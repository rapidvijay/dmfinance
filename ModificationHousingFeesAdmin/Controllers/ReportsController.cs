using Microsoft.ApplicationBlocks.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Configuration;
using System.Web.Http;

namespace ModificationHousingFeesAdmin.Controllers
{
    public class ReportsController : ApiController
    {
       
        [HttpPost]
        [ActionName("GetReport")]
        public HttpResponseMessage GetReport(RModel.ReportModel inputs)
        {
            string Mode = WebConfigurationManager.AppSettings["SystemMode"].ToString();
            string conStr = WebConfigurationManager.AppSettings["DB" + Mode.ToString().Trim()].ToString();

           

            object sts = DBNull.Value;
            object frm = DBNull.Value;
            object to = DBNull.Value;


            if(inputs.Status != 0)
            {
                if(inputs.Status == 1)
                {
                    sts = "Pending";
                }

                if (inputs.Status == 2)
                {
                    sts = "Completed";
                }

                if (inputs.Status == 3)
                {
                    sts = "Rejected";
                }
            }

            if(!string.IsNullOrEmpty(inputs.FromDate))
            {
                frm = inputs.FromDate;
            }

            if (!string.IsNullOrEmpty(inputs.ToDate))
            {
                to = inputs.ToDate;
            }

            DataTable ItemsList = new DataTable();
            try
            {        
                DataSet result = new DataSet();
                string q = @" select 
   Id  [ReferenceNo],
   RequestorType  [Requestor],
   AccountNumber  [AccountNo],
   PremiseNumber  [PremiseNo],
   MobileNumber  [Mobile],
   [Status],
   OwnerName [Owner]
 from [ModificationHousingFees].[dbo].[Request_Modify_DM_Housing_Fees]

 where 
 (@sts is null or Status like '%'+@sts+'%')
 and 
 (@frm is null or Creation_Date  > @frm)
 and 
 (@to is null or Creation_Date < dateadd(day,1, @to));";

                SqlParameter[] ParamsObject = {
                new SqlParameter("@sts", sts),
                new SqlParameter("@frm", frm),
                new SqlParameter("@to", to)
                  };

                result = SqlHelper.ExecuteDataset(conStr.ToString(), CommandType.Text, q, ParamsObject);

                if (result.Tables.Count > 0)
                {
                    if (result.Tables[0].Rows.Count > 0)
                    {
                        ItemsList = result.Tables[0];
                    }
                }

                if (ItemsList.Rows.Count > 0)
                {
                    return ControllerContext.Request
                            .CreateResponse(HttpStatusCode.OK, ItemsList);
                }
                else
                {
                    return ControllerContext.Request
                            .CreateResponse(HttpStatusCode.OK, "0");
                }
            }
            catch (Exception ex)
            {
                return ControllerContext.Request
                           .CreateResponse(HttpStatusCode.OK, ItemsList);
            }
        }
    }



   
}
