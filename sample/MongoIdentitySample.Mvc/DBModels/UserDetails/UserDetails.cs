using Main.Mvc.Models;
using System;

namespace Main.Mvc.DBModels
{
    public class UserDetails : EntityBase
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string Phone { get; set; }
        //   public bool? IsSupplier { get; set; }
        // public ObjectId? BuyerId { get; set; }
        public int Usertype { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int? UserCreationStatus { get; set; }
        public string KycRejectedReason { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string Mobile { get; set; }
        public Guid? TaxProfessionalId { get; set; }
    }
    enum UserType
    {
        Trader, TaxProfessionals, TaxRegulator

    }
}
