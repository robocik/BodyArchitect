using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Orchard.Security;

namespace Orchard.BodyArchitect.Models
{
    public class BACreateUserParams : CreateUserParams
    {
        public BACreateUserParams(string username, string password, string email, string passwordQuestion, string passwordAnswer, bool isApproved, int countryId) : base(username, password, email, passwordQuestion, passwordAnswer, isApproved) {
            CountryId = countryId;
        }

        public int CountryId { get; private set; }
    }
}