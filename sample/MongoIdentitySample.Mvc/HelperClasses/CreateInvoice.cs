using Main.Mvc.DBModels.TaxInvoice;
using Main.Mvc.Models;
using Main.Mvc.Models.MailInvoice;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using Microsoft.AspNetCore.Http;
using Ipfs.Api;
using MongoIdentitySample.Mvc.Models;


namespace Main.Mvc.HelperClasses
{
    public static class CreateInvoice
    {
        public static async Task  CreateInvoiceWithMailSending(CreateTaxInvoice invoice, string fromEmail,string path,ApplicationUser au)
        {
            int counter = 0;
            try
            {
               
                //var collectionmr = db.GetCollection<MerchantRegistration>("MerchantRegistration");
                // var mr = collectionmr.FindAll().ToList<MerchantRegistration>().FirstOrDefault();
                InvoiceJSON invoicejson = new InvoiceJSON();
                invoicejson.version = "1.0.1118";
                billLists[] billListarray = new billLists[1];
                billLists billlist = new billLists();
                billlist.userGstin = au.GstId;
                billlist.supplyType = "O";
                billlist.subSupplyType = 1;
                billlist.docType = "INV";
                billlist.docNo = "20";
                //Test
                billlist.docDate = invoice.DateIssue;
                billlist.transType = 1;
                billlist.fromGstin = invoice.GSTIDSeller;
                billlist.fromTrdName = invoice.NameoFSeller;
                //T2
                // billlist.fromAddr1 = mr.Address1;
                // billlist.fromAddr2 = mr.Address2;
                // billlist.fromPlace = mr.City;
                billlist.fromStateCode = 27;
                billlist.actualFromStateCode = 27;
                //T2
                billlist.toGstin = invoice.GSTIdBuyer;
                billlist.toTrdName = invoice.NameBuyer;
                billlist.toAddr1 = "1st floor, Jain Complex, opp.Bhoot Nath Mandir, Indra Nagar";
                billlist.toAddr2 = "";
                billlist.toPlace = "Pune";
                billlist.toStateCode = 27;
                billlist.actualToStateCode = 27;
                //Test
                billlist.totalValue = invoice.TotalAmount;// 121.00;
                billlist.cgstValue = invoice.GSTAmount / 2;
                billlist.sgstValue = invoice.GSTAmount / 2;
                billlist.igstValue = 0.00;
                billlist.cessValue = 0.00;
                billlist.TotNonAdvolVal = 0.00;
                billlist.OthValue = 0.0000;
                billlist.totInvValue = invoice.TotalAmount;
                billlist.transMode = 1;
                billlist.transDistance = 60;
                counter++;
                billlist.transporterName = "";
                billlist.transporterId = "";
                billlist.transDocNo = "";
                billlist.transDocDate = invoice.DateIssue;
                billlist.vehicleNo = "";
                billlist.vehicleType = "R";
                counter++;
                ItemLists[] itemlistarray = new ItemLists[invoice.itemDetails.Count];
                for (int count = 0; count < invoice.itemDetails.Count; count++)
                {
                    ItemLists itemlist = new ItemLists();
                    itemlist.itemNo = count + 1;
                    itemlist.productName = invoice.itemDetails[count].Product;
                    itemlist.productDesc = invoice.itemDetails[count].Product;
                    itemlist.quantity = invoice.itemDetails[count].Quantity;
                    //il.qtyUnit = invoice.itemDetails[count].Q;
                    itemlist.qtyUnit = "Qty";
                    itemlist.taxableAmount = invoice.itemDetails[count].Amount;// (invoice.itemDetails[count].GST / 100) * invoice.itemDetails[count].Amount;
                    itemlist.sgstRate = Convert.ToInt32(itemlist.taxableAmount / 2);
                    itemlist.cgstRate = Convert.ToInt32(itemlist.taxableAmount / 2);
                    itemlist.igstRate = 0;
                    itemlist.cessRate = 0;
                    itemlist.cessNonAdvol = 0;
                    itemlistarray[count] = itemlist;
                }

                billlist.itemList = itemlistarray;
                billListarray[0] = billlist;
                invoicejson.billLists = billListarray;
                string json = JsonConvert.SerializeObject(invoicejson, Formatting.Indented);
                var menstream = new MemoryStream(Encoding.Default.GetBytes(json));
                // var filePath = Path.GetTempFileName();
                //File.WriteAllBytes(filePath, menstream.ToArray());
                var stream = new MemoryStream(Encoding.Default.GetBytes(json));
                IFormFile file = new FormFile(stream, 0, (Encoding.Default.GetBytes(json)).Length, "name", "Invoice.json");

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

                //StreamReader streamReader = new StreamReader(new MemoryStream
              //  using (var stream = System.IO.File.Create(filePath))
                {
                    //File.WriteAllBytes(filePath, menstream.ToArray());

                    //await filePath.CopyToAsync(menstream);

                }
               StreamReader streamReader = new StreamReader(filePath);// Server.MapPath("~/FileUpload/" + Path.GetFileName(file.FileName)));
                string data = streamReader.ReadToEnd();

                //InvoiceJSON invoiceJSON = JsonConvert.DeserializeObject<InvoiceJSON>(data);
                IpfsClient ipfs = new IpfsClient("https://ipfs.infura.io:5001");
                var result = ipfs.FileSystem.AddFileAsync(filePath).Result;
                var url = "https://ipfs.io/ipfs/" + result.Id.Hash.ToString() + "?filename=" + "Invoice.json";

                MongoDbRepository<MailInvoice> mdr = new MongoDbRepository<MailInvoice>();

                MailInvoice mi = new MailInvoice();
                mi.invoiceJSON = invoicejson;
                mi.FromEmail = fromEmail;
                mi.ToEmail = invoice.EmailBuyer;
                mi.InvoiceStatus = MailInvoiceStatus.Created;
                mi.mailInvoiceCreationType = MailInvoiceCreationType.Created;
                mi.HashUrl = url;
                // mi.Hash = Hash;
                mdr.Insert(mi);
                var id = mi.Id.ToString();

                var fromAddress = new MailAddress("spathiontest@gmail.com", "From Name");
                var toAddress = new MailAddress(invoice.EmailBuyer, "To Name");
                counter++;
                const string fromPassword = "pachikkara";
                const string subject = "Invoice";
                ///const string body = "Body";
                Attachment attachment = new Attachment(menstream, new ContentType("application/json"));


                attachment.ContentDisposition.FileName = "Invoice.json";

                // attachment.ContentType= "application/json";

                //mailMessage.Attachments.Add(attachment);
                string authorityAccept = "http://" + path + "/TaxInvoice/Accept?id=" + id;
                string authorityReject = "http://" + path + "/TaxInvoice/Reject?id=" + id;
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
                // return invoicejson;
            }
            catch (Exception e)
            {
                
                // return invoicejson;

            }
        }
    }
}

