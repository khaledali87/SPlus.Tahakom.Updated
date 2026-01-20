
using Microsoft.SharePoint;
using Microsoft.SharePoint.Administration;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace SyncJobs.Helpers
{

    public class Email : SPJobDefinition
    {
        DataTable dtConfig = null;
        public static string key = "secret";
        public DataTable EmailConfigurationList(string SiteURL)
        {
            try
            {

                SPSecurity.RunWithElevatedPrivileges(delegate
                {
                    using (SPSite sps = new SPSite(SiteURL))
                    {
                        using (SPWeb spw = sps.OpenWeb())
                        {

                            SPList _list = spw.Lists["EmailConfiguration"];
                            dtConfig = _list.GetItems().GetDataTable();

                        }
                    }
                });

            }
            catch (Exception)
            {

                dtConfig = null;
            }

            return dtConfig;
        }

        public bool SendEMail(string SiteURL, string eNemailBody, string sentTo, string emailSubject, string link = "", List<Attachment> attachments = null, List<string> ccmail = null, List<string> bccmail = null, string SubjectParameter = "")
        {

            var dtConfig = EmailConfigurationList(SiteURL);

            bool result = false;

            try
            {
                if (dtConfig != null)
                {

                    SPSecurity.RunWithElevatedPrivileges(delegate ()
                    {
                        MailMessage message = new MailMessage();
                        // string strImageUrl = System.Web.HttpContext.Current.Server.MapPath("~/_layouts/15/images/logo.png");
                        message.Body += "";
                        //LinkedResource LinkedImage = new LinkedResource(strImageUrl);
                        //LinkedImage.ContentId = "PIC";
                        //LinkedImage.ContentType = new ContentType(MediaTypeNames.Image.Jpeg);

                        //message.Body = SPContext.Current.Web.Language == 1025
                        //      ? "<div dir='rtl' style=\"font: 15px Calibri, arial;\">"
                        //      : "<div dir='ltr' style=\"font: 15px Calibri, arial;\">";
                        //message.Body += "<table width=100%>";
                        //message.Body += "<tr><td><img src=cid:PIC></td></tr>";
                        //message.Body += "<tr><td>&nbsp;</td></tr>";
                        //message.Body += "<tr><td><font color='#000'>" + eNemailBody + "</font></td></tr>";
                        //message.Body += "<tr><td>&nbsp;</td></tr>";
                        //message.Body += "<tr><td><font color='#1569C7'>" + "Regards " +
                        //                "</font></td></tr>";
                        //message.Body += "<tr><td><font color='#1569C7'>" + enrecieverName +
                        //                "</font></td></tr>";
                        //message.Body += "</table></div>";
                        message.Body += eNemailBody;
                        AlternateView htmlView = AlternateView.CreateAlternateViewFromString(
                          message.Body,
                          null, "text/html");

                        //htmlView.LinkedResources.Add(LinkedImage);
                        //message.AlternateViews.Add(htmlView);

                        if (attachments != null)
                        {
                            foreach (var item in attachments)
                            {
                                message.Attachments.Add(item);
                            }
                        }



                        var fromMail = dtConfig.Rows[0]["SenderEmail"].ToString();
                        var SmtpServer = dtConfig.Rows[0]["SmtpServer"].ToString();
                        var port = int.Parse(dtConfig.Rows[0]["SmtpPort"].ToString());
                        var credentialsEmail = dtConfig.Rows[0]["SenderEmail"].ToString();
                        var credentialsPassword = dtConfig.Rows[0]["SenderPassword"].ToString();
                        if (!string.IsNullOrEmpty(fromMail) && !string.IsNullOrEmpty(SmtpServer) && !string.IsNullOrEmpty(port.ToString()) && !string.IsNullOrEmpty(credentialsEmail) && !string.IsNullOrEmpty(credentialsPassword))
                        {

                            using (MailMessage mm = new MailMessage(fromMail, sentTo))
                            {
                                mm.Subject = emailSubject;
                                mm.Body = eNemailBody;
                                mm.IsBodyHtml = true;
                                SmtpClient smtp = new SmtpClient();
                                smtp.Host = SmtpServer;
                                smtp.EnableSsl = true;
                                NetworkCredential NetworkCred = new NetworkCredential(credentialsEmail, Decrypt(credentialsPassword));
                                smtp.UseDefaultCredentials = true;
                                smtp.Credentials = NetworkCred;
                                smtp.Port = port;
                                smtp.Send(mm);
                                //System.Threading.Thread.Sleep(3000);
                            }
                            result = true;
                        }


                    });
                }





            }
            catch (SmtpException ex)
            {
                result = false;
                AddException(SiteURL, "Send Email", ex.Message);
            }
            return result;
        }
        public static string Decrypt(string cipherText)
        {
            cipherText = cipherText.Replace(" ", "+");
            byte[] cipherBytes = Convert.FromBase64String(cipherText);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(key, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(cipherBytes, 0, cipherBytes.Length);
                        cs.Close();
                    }
                    cipherText = Encoding.Unicode.GetString(ms.ToArray());
                }
            }
            return cipherText;
        }
        #region Log Area
        public void AddException(string SiteURL, string FunctionName, string Message)
        {
            SPSecurity.RunWithElevatedPrivileges(delegate ()
            {
                using (SPSite sps = new SPSite(SiteURL))
                {
                    using (SPWeb spw = sps.OpenWeb())
                    {
                        spw.AllowUnsafeUpdates = true;
                        SPList spl = spw.Lists.TryGetList("Exceptions");
                        SPListItem item = spl.Items.Add();
                        item["Title"] = "Exception";
                        item["Message"] = Message;
                        item["Controller"] = "Workflow Sync Job";
                        item["Function"] = FunctionName;
                        item.Update();
                        spw.AllowUnsafeUpdates = false;
                    }
                }
            });
        }

        #endregion
    }

}
