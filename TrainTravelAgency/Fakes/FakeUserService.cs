using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrainTravelAgency.Exceptions;
using TrainTravelAgency.Models;
using TrainTravelAgency.Services;

namespace TrainTravelAgency.Fakes
{
    public class FakeUserService : IUserService
    {
        public ExternalServiceErrorException ExceptionToThrow { get; set; }

        public User User { get; set; }

        public User GetUserById(Guid userId)
        {
            if (ExceptionToThrow != null)
            {
                throw ExceptionToThrow;
            }

            return User;
        }
    }
}