using Castle.Core.Logging;
using Main.Mvc.Authorization;
using Main.Mvc.DBModels;
using Main.Mvc.HelperClasses;
using Main.Mvc.Models;
using Main.Mvc.Models.MailInvoice;
//using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MongoIdentitySample.Mvc.Models;
using MongoIdentitySample.Mvc.Services;
using System;
using System.Linq;
using System.Threading.Tasks;
namespace Main.Mvc.Controllers
{
    public class ViewInvoiceController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IEmailSender _emailSender;
        private readonly ISmsSender _smsSender;
        private readonly ILogger _logger;
        public ViewInvoiceController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IEmailSender emailSender, ISmsSender smsSender)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
            _smsSender = smsSender;
        }

        // GET: ViewInvoice
        [TraderAuthorizeAttribute]
        public async Task<IActionResult> ReceivedInvoiceTrader()
        {
           await CheckReceiveMail.CheckMailAsync();
            await CheckIMAPMail.CheckMailAsync();
            MongoDbRepository<MailInvoice> mi = new MongoDbRepository<MailInvoice>();
            var mailInvoice = mi.SearchFor(x => x.mailInvoiceCreationType == MailInvoiceCreationType.Received);
            return View(mailInvoice);

        }

        public async Task<IActionResult> UploadedInvoiceTrader()
        {
           await CheckReceiveMail.CheckMailAsync();
            // CheckReceiveMail.CheckMail();
            MongoDbRepository<MailInvoice> mi = new MongoDbRepository<MailInvoice>();
            var mailInvoice = mi.SearchFor(x => (x.FromEmail == User.Identity.Name.ToString() || x.ToEmail == User.Identity.Name.ToString()) && x.mailInvoiceCreationType == MailInvoiceCreationType.Uploaded);
            return View(mailInvoice);

        }
        [TraderAuthorizeAttribute]
        public async Task<IActionResult> SendInvoiceTrader()
        {
           await CheckReceiveMail.CheckMailAsync();
            // CheckReceiveMail.CheckMail();
            MongoDbRepository<MailInvoice> mi = new MongoDbRepository<MailInvoice>();
            var mailInvoice = mi.SearchFor(x => (x.FromEmail == User.Identity.Name.ToString() || x.ToEmail == User.Identity.Name.ToString()) && (x.mailInvoiceCreationType == MailInvoiceCreationType.Created || x.mailInvoiceCreationType ==MailInvoiceCreationType.Uploaded));
            return View(mailInvoice);

        }
        public ActionResult GetMailInvoice(string id)
        {
            MongoDbRepository<MailInvoice> mi = new MongoDbRepository<MailInvoice>();
            var mailInvoice = mi.GetById(new Guid(id));
            return Json(mailInvoice);

        }
        [RegulatorAuthorizeAttribute]
        public ActionResult Regulator()
        {
            MongoDbRepository<MailInvoice> mi = new MongoDbRepository<MailInvoice>();
            var mailInvoice = mi.GetAll().ToList<MailInvoice>();
            return View(mailInvoice);
        }

        [TaxProfessionalAuthorizeAttribute]
        public ActionResult TaxPractitioner()
        {
            MongoDbRepository<MailInvoice> mi = new MongoDbRepository<MailInvoice>();
            var mailInvoice = mi.GetAll().ToList<MailInvoice>();
            return View(mailInvoice);

            //MongoDbRepository<UserDetails> ud = new MongoDbRepository<UserDetails>();
            //MongoDbRepository<MailInvoice> mi = new MongoDbRepository<MailInvoice>();
            //var mailInvoicefromemail = mi.SearchFor(x => x.FromEmail.Length == 0).ToList<MailInvoice>();
            //var taxProfessionalids = await _userManager.FindByNameAsync(User.Identity.Name.ToString());
            //var mailInvoicetoemail = from invoice in mi.GetAll().ToList<MailInvoice>()
            //                         join userdetails in ud.SearchFor(x => x.TaxProfessionalId == (Guid?)taxProfessionalids.Id).ToList<UserDetails>() on invoice.ToEmail equals userdetails.Email
            //                         select invoice;

            //return View(mailInvoicetoemail.Union(mailInvoicefromemail).Distinct<MailInvoice>());
        }

    }
}
