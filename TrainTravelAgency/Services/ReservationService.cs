using System;
using TrainTravelAgency.Exceptions;
using TrainTravelAgency.Models;
using TrainTravelAgency.Services;

namespace TrainTravelAgency
{
    public class ReservationService : IReservationService
    {
        private readonly IUserService _userService;
        private readonly ILoggerService _loggerService;
        private readonly IDistanceCalculationService _distanceCalculationService;

        public ReservationService (IUserService userService, ILoggerService loggerService, IDistanceCalculationService distanceCalculationService)
        {
            _userService = userService;
            _loggerService = loggerService;
            _distanceCalculationService = distanceCalculationService;
        }

        public double CalculateTicketPriceForUser(double distance, TicketType ticketType, Guid userId)
        {
            try
            {
                User user = _userService.GetUserById(userId);
                int numberOfTickets = user.NumberOfTicketsPurchasedInTheLastMonth;

                if (ticketType.Equals(TicketType.FirstClass)) 
                {
                    if (distance > 1500 || numberOfTickets > 10) 
                    {
                        return distance * 0.06;
                    }
                    else
                    {
                       return distance * 0.1;
                    }
                    
                }
                else if (ticketType.Equals(TicketType.SecondClass)) 
                {
                    if(distance > 1000 && numberOfTickets >= 15) 
                    {
                        return distance * 0.04;

                    }
                    else
                    {
                        return distance * 0.05;
                    }
                }
                else
                {
                    return distance * 0.01;
                }
            }
            catch(ExternalServiceErrorException ex)
            {
                _loggerService.LogError(ex.Message);
                throw ex;
            }
   
        }

        public double GetDistanceBetweenCities(Guid cityFromId, Guid cityToId)
        {
            double distanceInMiles =_distanceCalculationService.CalculateDistance(cityFromId, cityToId);
            return distanceInMiles * 1.060;
        }

        public TicketType? RecommendTicketType(SeatType seatType, double luggageWeight, bool beverage, int travelHour)
        {

            if (beverage)
            {
                if (seatType.Equals(SeatType.Regular))
                {
                    return null;
                }
                return TicketType.FirstClass;
            }

            if (luggageWeight > 30)
            {
                if(travelHour > 2 && travelHour < 5)
                {
                    return null;
                }
                return TicketType.SecondClass;
            }

            return TicketType.Economic;
        }

        
    }
}
