using Main.Mvc.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Main.Mvc.DBModels.UserToken
{
    public class UserToken: EntityBase
    {
       public string Token { get; set; }
        public int UserType { get; set; }
        public string Email { get; set; }

        public Guid? TaxProfessionalId { get; set; }
        public bool Created { get; set; }
    }
}
