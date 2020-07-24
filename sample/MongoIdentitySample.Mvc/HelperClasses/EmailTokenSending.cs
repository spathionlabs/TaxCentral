using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http.Extensions;

namespace Main.Mvc.HelperClasses
{
    public static class EmailTokenSending
    {

        public static void email_AdminsendToken(string toemail, string token, int usertype,string autority)
        {
            MailMessage mail = new MailMessage();
            //SmtpClient SmtpServer = new SmtpClient("mail.adfi.ae");
            //mail.IsBodyHtml = true;
            //SmtpServer.Port = 26;
            //SmtpServer.Credentials = new System.Net.NetworkCredential("info@adfi.ae", "Aex@#!$2019@");
            //SmtpServer.EnableSsl = true;
            //mail.From = new MailAddress("info@adfi.ae");
            //mail.To.Add(toemail);
            //mail.Subject = "New User SignUp";
            // string authority = "http://" + autority+ "/Password/ChangeAdminPassword?id=" + gid;
            var path = "http://" + autority + "/assets/img/logo2.png";
            string authority = "http://" + autority + "/Account/Register";
            // var path = autority;
            /// string body = "<div align=\"center\"><img src =" + path+ " width=\"150\" height=\"150\"> </div>" + "Welcome "+ Enum.GetName(typeof(UserType), usertype)+",<br/>Please create a user credentials: token :"+token + "<br/><br/><a href =\"" + authority + "\">" + "  Click here to Sign Up</a>";
            string body = "<div align=\"center\"><img src =" + path + " width=\"150\" height=\"150\"> </div>" + "Welcome to Spathion!<br /><br />Please proceed under following credentials to signup. <br/> Invitation Token:" + token +"<br /><br /><br /><a href =\'" + authority + "\'>" + "  Click here to Sign Up</a>";
           // var url = HttpC.Request.GetEncodedUrl();
           // HttpWebRequest

            //mail.Body = body;
            //SmtpServer.Send(mail);

            var fromAddress = new MailAddress("spathiontest@gmail.com", "From Name");
            var toAddress = new MailAddress("to@mailinator.com", "To Name");
            const string fromPassword = "pachikkara";
            const string subject = "Subject";
            ///const string body = "Body";

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
                Body = body,
                IsBodyHtml =true
            })
            {
                smtp.Send(message);
            }

        }

        public static void email_TradersendToken(string toemail, string token, int usertype, string autority)
        {
            MailMessage mail = new MailMessage();
            //SmtpClient SmtpServer = new SmtpClient("mail.adfi.ae");
            //mail.IsBodyHtml = true;
            //SmtpServer.Port = 26;
            //SmtpServer.Credentials = new System.Net.NetworkCredential("info@adfi.ae", "Aex@#!$2019@");
            //SmtpServer.EnableSsl = true;
            //mail.From = new MailAddress("info@adfi.ae");
            //mail.To.Add(toemail);
            //mail.Subject = "New User SignUp";
            // string authority = "http://" + autority+ "/Password/ChangeAdminPassword?id=" + gid;
            var path = "http://" + autority + "/assets/img/logo2.png";
            string authority = "http://" + autority + "/Account/RegisterTrader";
            // var path = autority;
            /// string body = "<div align=\"center\"><img src =" + path+ " width=\"150\" height=\"150\"> </div>" + "Welcome "+ Enum.GetName(typeof(UserType), usertype)+",<br/>Please create a user credentials: token :"+token + "<br/><br/><a href =\"" + authority + "\">" + "  Click here to Sign Up</a>";
            string body = "<div align=\"center\"><img src =" + path + " width=\"150\" height=\"150\"> </div>" + "Welcome to Spathion!<br /><br />Please proceed under following credentials to signup as Trader. <br/> Invitation Token:" + token + "<br /><br /><br /><a href =\'" + authority + "\'>" + "  Click here to Sign Up</a>";
            // var url = HttpC.Request.GetEncodedUrl();
            // HttpWebRequest

            //mail.Body = body;
            //SmtpServer.Send(mail);

            var fromAddress = new MailAddress("spathiontest@gmail.com", "From Name");
            var toAddress = new MailAddress("to@mailinator.com", "To Name");
            const string fromPassword = "pachikkara";
            const string subject = "Subject";
            ///const string body = "Body";

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
                Body = body,
                IsBodyHtml = true
            })
            {
                smtp.Send(message);
            }

        }



    }
}
