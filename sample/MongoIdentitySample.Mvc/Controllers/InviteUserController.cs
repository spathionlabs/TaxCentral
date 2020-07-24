using Main.Mvc.Authorization;
using Main.Mvc.DBModels;
using Main.Mvc.DBModels.UserToken;
using Main.Mvc.HelperClasses;
using Main.Mvc.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoIdentitySample.Mvc.Models;
using System;
using System.Threading.Tasks;

namespace Main.Mvc.Controllers
{

    public class InviteUserController : Controller
    {
        // private readonly ILogger _logger;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<InviteUserController> _logger;

        public InviteUserController(ILogger<InviteUserController> logger, IHttpContextAccessor httpContextAccessor, UserManager<ApplicationUser> userManager)
        //public InviteUserController(IHttpContextAccessor httpContextAccessor)

        {
           _logger = logger;
            _httpContextAccessor = httpContextAccessor;
            _userManager = userManager;
        }

        [TaxProfessionalAuthorizeAttribute]
        public ActionResult InviteUsers()
        {

            return View();


        }




        public ActionResult UserCreation(string email, string type)
        {

            try
            {
                MongoDbRepository<UserToken> ut = new MongoDbRepository<UserToken>();

                var token = Guid.NewGuid().ToString();
                UserToken userToken = new UserToken();
                userToken.Token = token;
                userToken.UserType = Convert.ToInt32(type);
                userToken.Email = email;
                userToken.Created = false;
                ut.Insert(userToken);
                //  string autority = _httpContextAccessor.Http;
                string host = _httpContextAccessor.HttpContext.Request.Host.Value;

                //HttpContext.Current.Request.Url.Authority
                EmailTokenSending.email_AdminsendToken(email, token, Convert.ToInt32(type), host);

                return Json("Invite sent by Email to : " + email);
            }
            catch (Exception)
            {
                return Json("Some Error");

            }
        }

        public ActionResult InviteTrader()
        {
            return View();
        }
        [HttpPost]
        //[TaxProfessionalsAuthorizeAttribute]
        public async Task<ActionResult> InviteTrader(string email)
        {
            try
            {
                MongoDbRepository<UserToken> ut = new MongoDbRepository<UserToken>();

                 var token = Guid.NewGuid().ToString();
                UserToken account = new UserToken();
                account.Token = token;
                account.UserType = (int)UserType.Trader;
                account.Email = email;
               // account.TaxProfessionalId = (await _userManager.FindByNameAsync(User.Identity.Name.ToString())).Id;
                // BuyerId =?null:(ObjectId)new ObjectId(buyerid),
                account.Created = false;
                //if (!(buyerid == "0"))
                //{
                //    account.BuyerId = (ObjectId)new ObjectId(buyerid);
                //}


                //acount.Token = token;
                //acount.UserType = type;
                //acount.Email = email;
                // BuyerId =?null:(ObjectId)new ObjectId(buyerid),
                account.Created = false;

                ut.Insert(account);
                string host = _httpContextAccessor.HttpContext.Request.Host.Value;
                EmailTokenSending.email_TradersendToken(email, token, Convert.ToInt32((int)UserType.Trader), host);

               // WattsAppSending.email_InviteTrader(email, token);

                return View();
            }
            catch (Exception)
            {
                return View();

            }
        }

    }
}