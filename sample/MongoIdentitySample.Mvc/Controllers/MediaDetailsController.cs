using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Castle.Core.Logging;
using Main.Mvc.DBModels;
using Main.Mvc.Models;
//using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MongoIdentitySample.Mvc.Models;
using MongoIdentitySample.Mvc.Services;

namespace Main.Mvc.Controllers
{
    public class MediaDetailsController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IEmailSender _emailSender;
        private readonly ISmsSender _smsSender;
        private readonly ILogger _logger;
        public MediaDetailsController(
          UserManager<ApplicationUser> userManager,
          SignInManager<ApplicationUser> signInManager,
          IEmailSender emailSender,
          ISmsSender smsSender
            )
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
            _smsSender = smsSender;
           
        }

        public IActionResult Index(string id)
        {
            return View();
        }

        public async Task<ActionResult> Tip(string id)
        {
            MongoDbRepository<MediaTip> mdr = new MongoDbRepository<MediaTip>();
            MediaTip mt = new MediaTip();
            mt.MediaDetailsId = Guid.Parse(id);
            mt.CreatedBy = (await _userManager.FindByNameAsync(User.Identity.Name.ToString())).Id;
            mt.CreatedOn = DateTime.Now;
            mdr.Insert(mt);
            return Json("Sucess");
        }

       // [HttpPost]
        //public JsonResult GetAllUsers()
        //{
        //   // var users = (from a in _userManager.Users
        //   //              select new { a.FirstName, a.LastName });
        //    var users = _userManager.Users.Select(s => new { FirstName = s.FirstName, LastName = s.LastName }).ToList();

        //    return Json(users);
                     

        //}
        class Users
        {
            public string FirstName { get; set; }
            public string LastName { get; set; }
        }
        public IActionResult CreateMedia()
        {
            return View();
        }
        public async Task<ActionResult> DisplayMedia()
        {
            MongoDbRepository<MediaDetails> mdr = new MongoDbRepository<MediaDetails>();
            MongoDbRepository<MediaTip> mt = new MongoDbRepository<MediaTip>();
            var list = mdr.GetAll();
            int count = 0;
            List<DisplayMedia> ldm =new  List<DisplayMedia>();
            foreach (var ls in list)
            {
               
                //list[count] = ls.id.ToString();
                if (ls.youtubeURL != null)
                    list[count].youtubeURL = ls.youtubeURL.Replace(".com/watch?v=", ".com/embed/");

                if (ls.FileUploadedPath != null)

                    list[count].FileUploadedPath = ls.FileUploadedPath.Replace(" ", "_");
               
                DisplayMedia dm = new DisplayMedia();
                dm.youtubeURL = list[count].youtubeURL;
                dm.ImageURL = list[count].ImageURL;
                dm.FileUploadedPath = list[count].FileUploadedPath;
                dm.UploadType = list[count].UploadType;
                dm.CreatedBy = list[count].CreatedBy;
                dm.CreatedOn = list[count].CreatedOn;
                dm.Id= list[count].Id;
                try
                {
                    var createdby = (await _userManager.FindByNameAsync(User.Identity.Name.ToString())).Id;
                    dm.TipSaved = (mt.GetAll().Where(x => x.CreatedBy == createdby && x.MediaDetailsId == dm.Id).Any());
                }
                catch(Exception)
                {//When user is not logged in
                    dm.TipSaved = false;
                }
                
                ldm.Add(dm);
                count++;
            }
            return View(ldm);
             
        }
        // [HttpGet("AsJson")]
        [HttpPost]
        public async Task<ActionResult> CreateMediaURL(string url, int type)
        {
            if (!checkedUrl(url))
            {
                return Json("Invalid Url");
            }
            if (URlExist(url))
            {
                return Json("URl Does Not Exist");
            }
            if (type == 0 && !ISYoutube(url))
            {
                return Json("Invalid youtube Video");
            }
            MongoDbRepository<MediaDetails> mdr = new MongoDbRepository<MediaDetails>();
            MediaDetails md = new MediaDetails();
            md.CreatedOn = DateTime.Now;
            if (type == 0)
            {
                if (url.Contains(".be"))
                {
                    try
                    {
                        HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);
                        request.Method = "HEAD";
                        using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                        {
                            //Console.WriteLine("Does this resolve to youtube?: {0}", response.ResponseUri.ToString().Contains("youtube.com") ? "Yes" : "No");
                            url = response.ResponseUri.ToString().Replace("&feature=youtu.be", "");

                        }
                    }
                    catch (Exception)
                    {

                    }
                }
                md.youtubeURL = url;
            }
            if (type == 1)
                md.ImageURL = url;
            md.UploadType = (Int16)type;
            md.CreatedBy = (await _userManager.FindByNameAsync(User.Identity.Name.ToString())).Id; 

            mdr.Insert(md);
            return Json("Url Saved");

        }

        [HttpPost("FileUpload")]
        public async Task<ActionResult> Index(List<IFormFile> files)
        {
            long size = files.Sum(f => f.Length);
            var filePath = Path.GetTempFileName();
            if (HttpContext.Request.Form.Files[0] != null)
            {
                var allFiles = HttpContext.Request.Form.Files;
                foreach (var file in allFiles)
                {

                    MongoDbRepository<MediaDetails> mdr = new MongoDbRepository<MediaDetails>();
                    MediaDetails md = new MediaDetails();
                    md.CreatedOn = DateTime.Now;
                    md.FileUploadedPath = String.Empty;
                    md.UploadType = (Int16)UploadType.FileUpload;
                    md.CreatedBy = Guid.NewGuid();
                    string fileName = file.FileName.Replace(' ', '_');
                    string guid = Guid.NewGuid().ToString();
                    ///filePath = Path.GetFullPath("/Uploads/") + Guid.NewGuid().ToString() + fileName;
                    filePath = Directory.GetCurrentDirectory().ToString() + "\\wwwroot\\Uploads\\" + guid + fileName;
                    var displayURL = Request.GetDisplayUrl();
                    displayURL = displayURL.Replace("/FileUpload", "/") + "Uploads/" + guid + fileName;
                    // Request.GetDisplayUrl() remove /FileUpload
                    // string fileURL = string.Format("{0}://{1}", Request.Url.Scheme, Request.Url.Authority)
                    //   string fileURL = (Request.Url.AbsoluteUri.Substring(0, Request.Url.AbsoluteUri.IndexOf(Request.Url.AbsolutePath)) + "/Uploads/" + newguid + fileName).Replace(' ', '_');
                    filePath = Directory.GetCurrentDirectory().Replace("/FileUpload", "/") + "\\wwwroot\\Uploads\\" + guid + fileName;
                    md.FileUploadedPath = displayURL;
                    mdr.Insert(md);

                    //  HttpPostedFileBase file = Request.Files[i]; //Uploaded file
                    //Use the following properties to get file's name, size and MIMEType
                    long fileSize = file.Length;

                    string mimeType = file.ContentType;
                    // Path.map
                    // filePath = Server.MapPath("~/Uploads/" + id) + new Guid + fileName.Replace(' ', '_');
                    //  filePath = Path.GetFullPath("~/Uploads/") + new Guid().ToString() + fileName;
                    //string filePath = Server.MapPath("~/Uploads/" + id) + new Guid().ToString() + fileName;
                    System.IO.Stream fileContent = file.OpenReadStream();

                    //To save file, use SaveAs method
                    //file.SaveAs(filePath); //File will be saved in application root

                    using (FileStream fs = new FileStream(filePath, FileMode.CreateNew, FileAccess.Write, FileShare.Write))
                    {
                        //file.CopyTo(fs);
                        // file.SaveAs(filePath);
                        file.CopyTo(fs);
                    }
                }

            }
            var filePaths = new List<string>();


            return RedirectToAction("CreateMedia", "MediaDetails");
        }
        private bool checkedUrl(string url)
        {
            try
            {
                if (!Uri.IsWellFormedUriString(url, UriKind.Absolute))
                    return false;
                else return true;
            }
            catch (Exception)
            {
                return false;
            }

        }
        private bool URlExist(string url)
        {
            try
            {
                HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
                //Setting the Request method HEAD, you can also use GET too.
                request.Method = "HEAD";
                //Getting the Web Response.
                HttpWebResponse response = request.GetResponse() as HttpWebResponse;
                //Returns TRUE if the Status code == 200
                response.Close();
                return (response.StatusCode == HttpStatusCode.OK);
            }
            catch (Exception)
            {
                return false;
            }
        }
        private bool ISYoutube(string url)
        {

            //You could perform a HEAD request:
            try
            {
                HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);
                request.Method = "HEAD";
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    //Console.WriteLine("Does this resolve to youtube?: {0}", response.ResponseUri.ToString().Contains("youtube.com") ? "Yes" : "No");
                    if (response.ResponseUri.ToString().Contains("youtube.com"))
                        return true;
                    else
                        return false;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}