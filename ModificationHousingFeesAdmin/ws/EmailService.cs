using ModificationHousingFeesAdmin.emailServiceReference;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Web;
using System.Web.Configuration;

namespace ModificationHousingFeesAdmin.ws
{
    public class EmailService
    {

        public string sendEmail(string toEmail, string message, string subject)
        {
            string result = "0";
            try
            {
                //System.Diagnostics.Debugger.Launch();
                emailServiceClient client = new emailServiceClient();
                using (OperationContextScope scope = new OperationContextScope(client.InnerChannel))
                {
                    EmailRequestType req = new EmailRequestType();

                    req.fromEmail = WebConfigurationManager.AppSettings["fromEmail"].ToString();
                    req.toEmail = toEmail;
                    req.subject = subject;
                    req.message = message;

                    commonDTO commonDTO = client.sendEmail(req);
                    result = commonDTO.status;
                    Console.WriteLine("############start sendEmail#####################");
                    Console.WriteLine("error" + commonDTO.errorMessage);
                    Console.WriteLine("#############End sendEmail######################");
                }
            }
            catch (Exception ex)
            {
                result = ex.Message;
                Console.WriteLine(ex.ToString());
            }
            return result;
        }



    }
}