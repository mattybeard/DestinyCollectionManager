using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace BungieWebClient
{
    static class Utilities
    {
        public static string ToJson(this object input)
        {
            try
            {
                return JsonConvert.SerializeObject(input);
            }
            catch (Exception ex)
            {
                SendErrorAlert(ex);
                return "";
            }
        }

        public static T FromJson<T>(this string input)
        {
            if (typeof(T) == typeof(string))
                return (T)(input as object);
            try
            {
                return JsonConvert.DeserializeObject<T>(input);
            }
            catch (Exception ex)
            {
                SendErrorAlert(ex);
                //log
            }
            return default(T);
        }

        private static void SendErrorAlert(Exception exception)
        {
            MailMessage msg = new MailMessage();

            msg.From = new MailAddress("mattybeard@gmail.com");
            msg.To.Add("mattybeard@gmail.com");
            msg.Subject = "GO Exception";
            msg.Body = exception.ToString();
            SmtpClient client = new SmtpClient();
            client.UseDefaultCredentials = true;
            client.Host = "smtp.gmail.com";
            client.Port = 587;
            client.EnableSsl = true;
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.Credentials = new NetworkCredential("mattybeard@gmail.com", "Baxter2242");
            client.Timeout = 20000;
            try
            {
                client.Send(msg);
            }
            catch (Exception ex)
            {
            }
            finally
            {
                msg.Dispose();
            }
        }
    }    
}
