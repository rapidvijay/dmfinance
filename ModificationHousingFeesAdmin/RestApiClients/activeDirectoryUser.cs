using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RefundFines.RestApiClients
{
    public class activeDirectoryUser
    {
        public string status            { get; set; }
        public string statusMessage     { get; set; }
        public string  address          { get; set; }
        public string commonName        { get; set; }
        public string department        { get; set; }
        public string employeeId        { get; set; }
        public string givenName         { get; set; }
        public string mail              { get; set; }
        public string samAccountName    { get; set; }
        public string telephoneNumber   { get; set; }
        public string title             { get; set; }
        public string userPrincipal     { get; set; }

        public string errorMessage      { get; set; }
        public string exceptionCause    { get; set; }
  
    }   
}