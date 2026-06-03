using NUnit.Framework;
using System;
using TrainTravelAgency;
using TrainTravelAgency.Exceptions;
using TrainTravelAgency.Fakes;
using TrainTravelAgency.Models;

namespace TrainTravelAgency.Test
{
    [TestFixture]
    internal class ReservationServiceTest
    {
        private FakeUserService fakeUserService;
        private FakeLoggerService fakeLoggerService;
        private FakeDistanceCalculationService fakeDistanceCalculationService;
        private ReservationService reservationService;

        [SetUp]
        public void SetUp()
        {
            fakeUserService = new FakeUserService();
            fakeLoggerService = new FakeLoggerService();
            fakeDistanceCalculationService = new FakeDistanceCalculationService();

            reservationService = new ReservationService(
                fakeUserService,
                fakeLoggerService,
                fakeDistanceCalculationService);
        }

        [Test] //ulazimo u if proveraavamo da li je jedan od uslova ispunjen i ako jeste racuna distance*0.06
        public void CalculateTicketPriceForUser_GreaterThen1500AndDontNeedToBeGreaterThen10_FirstClass()
        {
            fakeUserService.User = new User
            {
                NumberOfTicketsPurchasedInTheLastMonth = 5
            };
            double actual = reservationService.CalculateTicketPriceForUser(2500, TicketType.FirstClass, Guid.NewGuid());
            Assert.AreEqual(actual, 150);
        }

        [Test]//ExternalServiceErrorException, kada je servis nedostupan
        public void CalculateTicketPriceForUser_WhenUserServiceThrows_LogsErrorAndRethrows()
        {
            fakeUserService.ExceptionToThrow =
            new ExternalServiceErrorException("Service unavailable");
            Assert.Throws<ExternalServiceErrorException>(() =>
            {
                reservationService.CalculateTicketPriceForUser(500, TicketType.Economic, Guid.NewGuid());
            });
            Assert.AreEqual("Service unavailable", fakeLoggerService.LoggedMessage);
        }

        [TestCase(2500, TicketType.FirstClass, 5, 150)] // ako je tip karte firstclass, distanca vece od 1500 ili broj karata vece od 10, ispunjava firstclass. ispunjava samo jedan uslov, a to je da je distanca veca od 1500
        [TestCase(1000, TicketType.FirstClass, 7, 100)] // ako je tip karte firstclass, distanca vece od 1500 ili broj karata vece od 10, ispunjava firstclass, ne ispunjava ni jedan uslov i ulazi u else i racuna distancu * 0.1
        [TestCase(500, TicketType.FirstClass, 11, 30)] // ako je tip karte firstclass,distanca vece od 1500 ili broj karata veca od 10, ispunjava samo uslov da je vece od 10, i mnozi nam distancu sa 0.06, broj karata je veci od 10
        [TestCase(1200, TicketType.SecondClass, 15, 48)] // tip SecondClass, distanca veca od 1000 i broj karata >= 15, onda racuna distance*0.04
        [TestCase(900, TicketType.SecondClass, 10, 45)] // tip SecondClass, dist < 1000 i br krt < 15, distance*0,05 iz else
        [TestCase(1000, TicketType.Economic, 50, 10)] // tip nije ni ScndCLass ni FrstClass i onda zadajemo trecu preostalu, mnozimo distancu sa 0.01
        public void CalculateTicketPriceForUser_ReturnsExpectedPrice(double distance, TicketType ticketType, int numberOfTickets, double expected)
        {
            fakeUserService.User = new User
            {
                NumberOfTicketsPurchasedInTheLastMonth = numberOfTickets
            };
            double actual = reservationService.CalculateTicketPriceForUser(distance, ticketType, Guid.NewGuid());
            Assert.AreEqual(expected, actual);
        }

        [Test] // vraca nam izracunatu distancu, tj. zadajemo distancu za koju mislimo da je izmedju gradova, i on racuna tu distancu *1.060
        public void GetDistanceBetweenCities_ReturnsConvertedDistance()
        {
            fakeDistanceCalculationService.DistanceToReturn = 100;
            double actual = reservationService.GetDistanceBetweenCities(
                Guid.NewGuid(),
                Guid.NewGuid());
            Assert.AreEqual(106, actual);
        }

        [Test]
        public void RecommendTicketType_BeverageAndRegularSeat_ReturnsNull() //kada je seattype regular i bevarage true vraca nam null
        {
            TicketType? actual = reservationService.RecommendTicketType(SeatType.Regular, 15, true, 15);
            Assert.IsNull(actual);
        }

        [TestCase(SeatType.Table, 10, true, 10, TicketType.FirstClass)] // kada je beverage true i kada nije regular, lugguage nam nije bitan, vraca nam uvek firstclass
        [TestCase(SeatType.Regular, 40, false, 10, TicketType.SecondClass)] // kada je regular, beverage false ulazimo u if za luggageWeight i tu mora da nam bude vece od 30, i manje od 2h ili vece od 5h da bi nam vratilo SecondClass
        [TestCase(SeatType.Table, 40, false, 3, null)] // kada je table, kada je vece od 30 izmedju 2h i manje od 5h
        [TestCase(SeatType.Regular, 10, false, 10, TicketType.Economic)] // kada je manje od 30 i false beverage onda nam vraca Economic
        public void RecommendTicketType_ReturnsExpectedResult(SeatType seatType, double luggageWeight, bool beverage, int travelHour, TicketType? expected)
        {
            TicketType? actual = reservationService.RecommendTicketType(seatType, luggageWeight, beverage, travelHour);
            Assert.AreEqual(expected, actual);
        }
    }
}

