using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrainTravelAgency.Services;

namespace TrainTravelAgency.Fakes
{
    public class FakeDistanceCalculationService : IDistanceCalculationService
    {
        public double DistanceToReturn { get; set; }

        public double CalculateDistance(Guid source, Guid destination)
        {
            return DistanceToReturn;
        }
    }
}