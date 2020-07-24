using AspNetCore.Identity.MongoDbCore.Models;

namespace MongoIdentitySample.Mvc.Models
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    public class ApplicationUser : MongoIdentityUser
    {
        public string EntityName { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }

        public string PINCode { get; set; }
        public string GstId { get; set; }

    }
}
