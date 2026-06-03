using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrainTravelAgency.Services;

namespace TrainTravelAgency.Fakes
{
    public class FakeLoggerService : ILoggerService
    {
        public string LoggedMessage { get; private set; }

        public void LogError(string message)
        {
            LoggedMessage = message;
        }
    }
}