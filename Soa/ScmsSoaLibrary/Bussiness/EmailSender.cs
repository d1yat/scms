/*
 * Created By Indra
 * 20171231FM
 * 
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ScmsModel;
using ScmsSoaLibrary.Parser;
using ScmsSoaLibrary.Commons;
using System.Data.Common;
using ScmsSoaLibrary.Core.Threading;
using ScmsModel.Core;
using ScmsSoaLibraryInterface.Commons;


namespace ScmsSoaLibrary.Bussiness
{
    class EmailSender
    {

        public static int EmailParameter(ORMDataContext db, 
                                                        string EmailSender,
                                                        string TextSender,
                                                        string Received,
                                                        string CarbonCopy,
                                                        string EmailSubject,
                                                        string EmailHeader,
                                                        string EmailContent,
                                                        string Emailtable,
                                                        string EmailFooter)
        {


            System.Net.Mail.SmtpClient smtp = null;
            StringBuilder sb = new StringBuilder();


            string strEmailSender   = "scms.dophar@ams.co.id";
            string strTextSender    = TextSender;
            string strReceived      = Received;
            string strCarbonCopy    = CarbonCopy;
            string strSubject       = EmailSubject;
            string strEmailHeader   = EmailHeader;
            string strContent       = EmailContent;
            string strEmailFooter   = EmailFooter;

            if (Emailtable != "")
            {
                Emailtable = "<table style='font-family:Arial, Helvetica, sans-serif;' border=1>" + Emailtable + "</table>";
                strContent = strContent + Emailtable;
            }
            string SaveText = "";            

            try
            {
                using (System.Net.Mail.MailMessage mail = new System.Net.Mail.MailMessage())
                {
                    // send mail containing the file here

                    mail.From = new System.Net.Mail.MailAddress(strEmailSender, strTextSender);

                    mail.Subject = strSubject;

                    for (int Awal = 0; Awal < strReceived.Length; Awal++)
                    {
                        if (strReceived.Substring(Awal, 1) != ";")
                        {
                            SaveText += strReceived.Substring(Awal, 1);
                        }
                        else
                        {
                            mail.To.Add(SaveText);
                            SaveText = "";
                        }

                    }

                    for (int Awal = 0; Awal < strCarbonCopy.Length; Awal++)
                    {
                        if (strCarbonCopy.Substring(Awal, 1) != ";")
                        {
                            SaveText += strCarbonCopy.Substring(Awal, 1);
                        }
                        else
                        {
                            mail.CC.Add(SaveText);
                            SaveText = "";
                        }

                    }

                    sb.AppendLine(EmailHeader);
                    sb.AppendLine(strContent);
                    sb.AppendLine(strEmailFooter);

                    mail.IsBodyHtml = true;
                    mail.Body = "<html style='font-family:Arial, Helvetica, sans-serif;'><body>" + sb.ToString() + "</body></html>";
                    
                    smtp = new System.Net.Mail.SmtpClient("10.100.10.9", 25);

                    smtp.DeliveryMethod = System.Net.Mail.SmtpDeliveryMethod.Network;
                    smtp.UseDefaultCredentials = false;
                    smtp.Credentials = new System.Net.NetworkCredential("scms.dophar@ams.co.id", "scms");

                    smtp.Send(mail);

                    sb.Length = 0;
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
            }
            

            int isOk = 1;

            return isOk;
        }
    }
}
