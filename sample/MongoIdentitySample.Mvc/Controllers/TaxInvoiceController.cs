using AspNetCore.Identity.MongoDbCore.Models;
using Main.Mvc.DBModels;
using Main.Mvc.DBModels.UserToken;
using Main.Mvc.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using MongoIdentitySample.Mvc.Models;
using MongoIdentitySample.Mvc.Models.AccountViewModels;
using MongoIdentitySample.Mvc.Services;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Main.Mvc.DBModels.TaxInvoice;
using System.Net;
using System.Net.Mail;
using Newtonsoft.Json;
using System;
using Main.Mvc.HelperClasses;
using System.Collections.Generic;
using Nancy.Json;
using System.IO;
using Main.Mvc.Models.MailInvoice;
using System.Net.Mime;
using Ipfs.Api;
using Main.Mvc.Authorization;

namespace Main.Mvc.Controllers
{
    public class TaxInvoiceController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IEmailSender _emailSender;
        private readonly ISmsSender _smsSender;
        private readonly ILogger _logger;
        private readonly RoleManager<MongoIdentityRole> _roleManager;


        public TaxInvoiceController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IEmailSender emailSender,
            ISmsSender smsSender,
            ILoggerFactory loggerFactory,
            RoleManager<MongoIdentityRole> roleManager
   )
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
            _smsSender = smsSender;
            _logger = loggerFactory.CreateLogger<TaxInvoiceController>();
            _roleManager = roleManager;
        }

        public IActionResult Index()
        {
            return View();
        }

        [TraderAuthorizeAttribute]
        public ActionResult UploadInvoice()
        {
            return View();
        }

        [HttpPost]
        [TraderAuthorizeAttribute]
        public async Task<ActionResult> UploadInvoice(List<Microsoft.AspNetCore.Http.IFormFile> file, string toemail)
        {
            try
            {
                var formFile = file[0];
                if (formFile.Length > 0)
                {
                    var filePath = Path.GetTempFileName();

                    using (var stream = System.IO.File.Create(filePath))
                    {
                        await formFile.CopyToAsync(stream);

                    }
                    StreamReader streamReader = new StreamReader(filePath);// Server.MapPath("~/FileUpload/" + Path.GetFileName(file.FileName)));
                    string data = streamReader.ReadToEnd();

                    InvoiceJSON invoiceJSON = JsonConvert.DeserializeObject<InvoiceJSON>(data);
                    IpfsClient ipfs = new IpfsClient("https://ipfs.infura.io:5001");
                    var result = ipfs.FileSystem.AddFileAsync(filePath).Result;
                    var url = "https://ipfs.io/ipfs/" + result.Id.Hash.ToString() + "?filename=" + formFile.FileName.Replace(" ", "%20");


                    MailInvoice mailInvoice = new MailInvoice();
                    mailInvoice.ToEmail = toemail;
                    mailInvoice.invoiceJSON = invoiceJSON;
                    mailInvoice.CreatedOn = DateTime.Now;
                    mailInvoice.InvoiceStatus = MailInvoiceStatus.Created;
                    mailInvoice.FromEmail = User.Identity.Name.ToString();
                    mailInvoice.mailInvoiceCreationType = MailInvoiceCreationType.Uploaded;
                    mailInvoice.HashUrl = url;
                    //mailInvoice.Hash = Hash;
                    MongoDbRepository<MailInvoice> mi = new MongoDbRepository<MailInvoice>();
                    mi.Insert(mailInvoice);
                    //  var a = collection.Save(mailInvoice);
                    var id = mailInvoice.Id.ToString();
                    //string json = JsonConvert.SerializeObject(invoicejson, Formatting.Indented);
                    var menstream = formFile.OpenReadStream();// new MemoryStream(Encoding.Default.GetBytes(json));
                    var fromAddress = new MailAddress("spathiontest@gmail.com", "From Name");
                    var toAddress = new MailAddress(toemail, "To Name");

                    const string fromPassword = "pachikkara";
                    const string subject = "Invoice";
                    ///const string body = "Body";
                    Attachment attachment = new Attachment(menstream, new ContentType("application/json"));


                    attachment.ContentDisposition.FileName = "Invoice.json";

                    // attachment.ContentType= "application/json";

                    //mailMessage.Attachments.Add(attachment);
                    string authorityAccept = "http://" + HttpContext.Request.Host.Value + "/TaxInvoice/Accept?id=" + id;
                    string authorityReject = "http://" + HttpContext.Request.Host.Value + "/TaxInvoice/Reject?id=" + id;
                    //string a1= "<html>You have received an Invoice. Please do check the invoice and conform <a href=\'";


                    var html = "<html> You have received an Invoice. Please do check the invoice and conform<br /><a href='" + authorityAccept + "' style= \"background-color: #90EE90\" > Accept </a>&nbsp &nbsp<a href='" + authorityReject + "' style= \"background-color: #FFFF00\" > Reject </a></ html >";


                    var smtp = new SmtpClient
                    {
                        Host = "smtp.gmail.com",
                        Port = 587,
                        EnableSsl = true,
                        DeliveryMethod = SmtpDeliveryMethod.Network,
                        UseDefaultCredentials = false,
                        Credentials = new NetworkCredential(fromAddress.Address, fromPassword)
                    };
                    using (var message = new MailMessage(fromAddress, toAddress)
                    {
                        Subject = subject,
                        Body = html,
                        IsBodyHtml = true
                    })

                    {
                        message.Attachments.Add(attachment);
                        smtp.Send(message);
                    }


                }
                //ViewBag.Message = "File Uploaded Successfully!!";
                return RedirectToAction("UploadInvoice");
            }
            catch
            {
                // ViewBag.Message = "File upload failed!!";
                return RedirectToAction("UploadInvoice");
            }
        }

        [TraderAuthorizeAttribute]
        public async Task<ActionResult> Create()
        {
            // var id = SignInManager.UserManager.Users.FirstOrDefault().Id;
             ApplicationUser loadedUserDetails = (await _userManager.FindByNameAsync(User.Identity.Name.ToString()));
            CreateTaxInvoice cti = new CreateTaxInvoice();
            cti.GSTIDSeller = loadedUserDetails.GstId;
            cti.NameoFSeller = loadedUserDetails.EntityName;
            cti.DateIssue = DateTime.Now.ToShortDateString();
            cti.SerialNumber = Guid.NewGuid().ToString();

            return View(cti);
        }
        [HttpPost]
        [TraderAuthorizeAttribute]
        public async Task<ActionResult> Create(string itemarray, string NameBuyer, string GSTIDBuyer, string RegisteredMobileNoBuyer, string TotalAmountHidden, string GSTAmountHidden, string DateIssue, string SerialNumber, string NameOfSeller, string GSTIdSeller, string PONO, string emailBuyer)
        {
            int count = 0;
            try
            {


                List<InvoiceItemDetail> itemList = new List<InvoiceItemDetail>();
                JavaScriptSerializer js = new JavaScriptSerializer();
                string[] itemsarray = js.Deserialize<string[]>(js.Deserialize<string>(JsonConvert.SerializeObject(itemarray)));
                int totalpurchaseItems = itemsarray.Length;
                count++;
                if (itemarray != null)
                {
                    for (int i = 0; i < totalpurchaseItems / 5; i++)
                    {
                        InvoiceItemDetail item = new InvoiceItemDetail();
                        item.Product = itemsarray[(i * 5) + 0];
                        item.GST = Convert.ToDouble(itemsarray[(i * 5) + 1]);
                        item.Quantity = Convert.ToDouble(itemsarray[(i * 5) + 2]);
                        item.Rate = Convert.ToDouble(itemsarray[(i * 5) + 3]);
                        item.Amount = Convert.ToDouble(itemsarray[(i * 5) + 4]);
                        itemList.Add(item);
                        //item.Amount = Convert.ToInt32(itemsarray[(i * 7) + 5]);
                    }
                }
                count++;
                CreateTaxInvoice taxInvoice = new CreateTaxInvoice();
                taxInvoice.DateIssue = DateIssue;
                taxInvoice.SerialNumber = SerialNumber;
                taxInvoice.NameoFSeller = NameOfSeller;//,string GSTIdSeller
                taxInvoice.GSTIDSeller = GSTIdSeller;
                taxInvoice.NameBuyer = NameBuyer;
                taxInvoice.EmailBuyer = emailBuyer;
                taxInvoice.GSTIdBuyer = GSTIDBuyer;
                taxInvoice.RegisteNoBuyer = RegisteredMobileNoBuyer;
                taxInvoice.itemDetails = itemList;
                taxInvoice.MerchantId = (await _userManager.FindByNameAsync(User.Identity.Name.ToString())).Id;
                taxInvoice.TotalAmount = Convert.ToDouble(TotalAmountHidden);
                taxInvoice.GSTAmount = Convert.ToDouble(GSTAmountHidden);
                taxInvoice.TaxStatus = (int)TaxInvoiceStatus.Created;
                taxInvoice.CreatedOn = DateTime.Now;
                taxInvoice.PONO = PONO;
                MongoDbRepository<CreateTaxInvoice> mdr = new MongoDbRepository<CreateTaxInvoice>();
                mdr.Insert(taxInvoice);
                var path = HttpContext.Request.Host.Value;
                ApplicationUser loadedUserDetails = (await _userManager.FindByNameAsync(User.Identity.Name.ToString()));

                await CreateInvoice.CreateInvoiceWithMailSending(taxInvoice, User.Identity.Name.ToString(), path,loadedUserDetails);
                count++;

            }

            //CreateTaxInvoice cti = new CreateTaxInvoice();
            catch (Exception e)
            {
                var smtp = new SmtpClient
                {
                    Host = "smtp.gmail.com",
                    Port = 587,
                    EnableSsl = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential("spathiontest@gmail.com", "pachikkara")
                };

                using (var message = new MailMessage("spathiontest@gmail.com", "punnoosepj@outlook.com")
                {
                    Subject = "main Loop",
                    Body = count.ToString()
                })
                {
                    smtp.Send(message);
                }
            }
            return RedirectToAction("Create");
        }

        public ActionResult validateca(string id)
        {
            MongoDbRepository<MailInvoice> mi = new MongoDbRepository<MailInvoice>();
            var mailInvoice = mi.GetById(new Guid(id));
            mailInvoice.taxpractitionerstatus = TaxPractitionerStatus.Validated;
            mailInvoice.TaxPractioner = User.Identity.Name.ToString();
            mi.Update(mailInvoice);
            return RedirectToAction("TaxPractitioner", "ViewInvoice");

        }

        public ActionResult rejectca(string id)
        {
            MongoDbRepository<MailInvoice> mi = new MongoDbRepository<MailInvoice>();
            var mailInvoice = mi.GetById(new Guid(id));
            mailInvoice.taxpractitionerstatus = TaxPractitionerStatus.Rejected;
            mailInvoice.TaxPractioner = User.Identity.Name.ToString();
            mi.Update(mailInvoice);
            return RedirectToAction("TaxPractitioner", "ViewInvoice");


        }
        public ActionResult Accept(string id)
        {
            MongoDbRepository<MailInvoice> mi = new MongoDbRepository<MailInvoice>();
            var mailInvoice = mi.GetById(new Guid(id));
            mailInvoice.InvoiceStatus = MailInvoiceStatus.Accepted;
            mi.Update(mailInvoice);
            return View();
        }


        public ActionResult Reject(string id)
        {
            MongoDbRepository<MailInvoice> mi = new MongoDbRepository<MailInvoice>();
            var mailInvoice = mi.GetById(new Guid(id));
            mailInvoice.InvoiceStatus = MailInvoiceStatus.Rejected;
            mi.Update(mailInvoice);
            return View();
        }


    }
}