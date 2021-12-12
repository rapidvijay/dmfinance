using ModificationHousingFeesAdmin.smsServiceReference;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ModificationHousingFeesAdmin.ws
{
    public class SMSService
    {

        public String sendSMS(string message, string mobile)
        {
            string result = "0";

            try
            {
                DmSmsBrokerServicePTClient client = new DmSmsBrokerServicePTClient();
                DmSmsBrokerRequestMessage request = new DmSmsBrokerRequestMessage();
                request.Message = message;
                request.Recipients = new string[1];
                request.Recipients[0] = mobile;
                DmSmsBrokerPesponseMessage smsBrokerPesponseMessage = client.sendMultipleSMS(request);
                result = smsBrokerPesponseMessage.SMSAcknowledgement.SMSResponseList[0].StatusCode.Message;
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