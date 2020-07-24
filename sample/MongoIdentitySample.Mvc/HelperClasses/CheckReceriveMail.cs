using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using EAGetMail;
using Main.Mvc.Models;
using Main.Mvc.Models.MailInvoice;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using Newtonsoft.Json;
using Ipfs.Api;
using System.Text;
using Microsoft.AspNetCore.Http;

namespace Main.Mvc.HelperClasses
{
    public static class CheckReceiveMail
    {
        public static async System.Threading.Tasks.Task CheckMailAsync()//string toemail, string username, string password)
        {
            try
            {
                MailServer oServer = new MailServer("imap.gmail.com",
                                "spathiontest@gmail.com",
                                "pachikkara",
                                EAGetMail.ServerProtocol.Imap4);

                // Enable SSL connection.
                oServer.SSLConnection = true;

                // Set 993 SSL port
                oServer.Port = 993;

                MailClient oClient = new MailClient("TryIt");
                oClient.Connect(oServer);
                MongoDbRepository<MailInvoice> mi = new MongoDbRepository<MailInvoice>();// retrieve unread/new email only
                oClient.GetMailInfosParam.Reset();
                oClient.GetMailInfosParam.GetMailInfosOptions = GetMailInfosOptionType.NewOnly;
                //oClient.GetMailInfosParam.GetMailInfosOptions = GetMailInfosOptionType.;

                MailInfo[] infos = oClient.GetMailInfos();
                Console.WriteLine("Total {0} unread email(s)\r\n", infos.Length);
                for (int i = 0; i < infos.Length; i++)
                {
                    MailInfo info = infos[i];
                    Console.WriteLine("Index: {0}; Size: {1}; UIDL: {2}",
                        info.Index, info.Size, info.UIDL);

                    // Receive email from IMAP4 server
                    Mail oMail = oClient.GetMail(info);
                    if (oMail.Attachments.Length > 0)
                    {
                        foreach (var attachment in oMail.Attachments)
                        {
                            if (attachment.ContentType == "application/json")
                            {
                                var stream = new MemoryStream(attachment.Content);
                                IFormFile file = new FormFile(stream, 0, attachment.Content.Length, "name", "Invoice.json");

                               // var filePath = Path.GetTempFileName();
                                var filePath = Path.GetTempFileName();

                                using (var streamfile = System.IO.File.Create(filePath))
                                {
                                    await file.CopyToAsync(streamfile);

                                }
                                //File.WriteAllBytes(filePath, attachment.Content);

                                //using (var streamcreate = System.IO.File.Create(filePath))
                                //{

                                //}

                                //StreamReader streamReader = new StreamReader(new MemoryStream(attachment.Content));// Server.MapPath("~/FileUpload/" + Path.GetFileName(file.FileName)));
                               // string data = streamReader.ReadToEnd();
                                IpfsClient ipfs = new IpfsClient("https://ipfs.infura.io:5001");
                                var result = ipfs.FileSystem.AddFileAsync(filePath).Result;
                                var url = "https://ipfs.io/ipfs/" + result.Id.Hash.ToString() + "?filename=" + "Invoice.json";

                                using (StreamReader r = new StreamReader(new MemoryStream(attachment.Content)))
                                {
                                    var json = r.ReadToEnd();
                                    var invoiceJSON = JsonConvert.DeserializeObject<InvoiceJSON>(json);
                                    MailInvoice mailInvoice = new MailInvoice();
                                    mailInvoice.ToEmail = oMail.From.Address;
                                    mailInvoice.invoiceJSON = invoiceJSON;
                                    mailInvoice.CreatedOn = DateTime.Now;
                                    mailInvoice.InvoiceStatus = MailInvoiceStatus.Created;
                                    mailInvoice.IsSend = false;
                                    mailInvoice.HashUrl = url;
                                    mailInvoice.mailInvoiceCreationType = MailInvoiceCreationType.Received;
                                    mi.Insert(mailInvoice);
                                }
                            }

                        }
                    }

                    Console.WriteLine("From: {0}", oMail.From.ToString());
                    Console.WriteLine("Subject: {0}\r\n", oMail.Subject);
                    if (!info.Read)
                    {
                        oClient.MarkAsRead(info, true);
                    }
                }

                // Quit and expunge emails marked as deleted from IMAP4 server.
                oClient.Quit();
                Console.WriteLine("Completed!");
            }
            catch (Exception ep)
            {
                Console.WriteLine(ep.Message);
            }

        }
    }
}
