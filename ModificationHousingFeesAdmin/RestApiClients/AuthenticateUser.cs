using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Configuration;

namespace RefundFines.RestApiClients
{
    public class AuthenticateUser
    {
        static HttpClient client = new HttpClient();

        public static IRestResponse<activeDirectoryUser> GetUserInfoByUserNameAndPassword(string username, string password)
        {
            var activeDirectoryURL = WebConfigurationManager.AppSettings["activeDirectory"].ToString();
            //var client = new RestClient("http://bexml10.dm.gov.ae:38081/DMIntegrationWebServices/resources/activeDirectoryService/authenticateUser");
            var client = new RestClient(activeDirectoryURL);
            var request = new RestRequest(Method.GET);
            request.AddHeader("username", username.Trim());
            request.AddHeader("password", password);
            IRestResponse<activeDirectoryUser> response = client.Execute<activeDirectoryUser>(request);

            return response;
        }


        //public static activeDirectoryUser GetUserInfoByUserNameAndPassword(string username, string password)
        //{




        //    return new activeDirectoryUser
        //    {
        //        status = "Success",
        //        employeeId = "21",
        //        mail = "reach_psundaram@dm.gov.ae",
        //    };
        //}

        internal static IRestResponse<activeDirectoryUser> GetUserInfoByUserNameAndPassword(string uSER_NAME, object pASSWORD)
        {
            throw new NotImplementedException();
        }
    }

    
}