using ModificationHousingFeesAdmin.Helpers;
using ModificationHousingFeesAdmin.Models;
using ModificationHousingFeesAdmin.ViewModel;
using ModificationHousingFeesAdmin.ws;
using PagedList;
using RefundFines.RestApiClients;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web; 
using System.Web.Configuration;
using System.Web.Mvc;
using AutoMapper;
using static ModificationHousingFeesAdmin.ViewModel.RequestModifyHousingFeesViewModel;
using System.Configuration;
using Newtonsoft.Json;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace ModificationHousingFeesAdmin.Controllers
{
    [ValidateInput(true)]
    public class HomeController : Controller
    {

        ModificationHousingFeesEntities _dbContext = new ModificationHousingFeesEntities();

        private static readonly log4net.ILog log
       = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public ActionResult Index()
        {
            log.Info("into index");

            ViewBag.StatusNEW = _dbContext.Request_Modify_DM_Housing_Fees.Where(x => x.Status == Stauts.PendingReview.ToString()).Count();
            ViewBag.StatusReviewed = _dbContext.Request_Modify_DM_Housing_Fees.Where(x => x.Status == Stauts.PendingDewa.ToString()).Count();
            
            ViewBag.StatusFinalAudit = ""; //_dbContext.Refund_Fine_Data.Where(x => x.Status == Refund_Fine_Data_ViewModel.Statuses.FinalAudit.ToString()).Count();
            log.Info(ViewBag.StatusNEW);
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }


        //the single action you don't want to be authorized.
        [HttpGet] //https://www.c-sharpcorner.com/article/simple-login-application-using-Asp-Net-mvc/
        public ActionResult Login()
        {
           
            Common.User = null;
            ViewBag.Message = "Your contact page.";
            return View(new UserDetails());
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult Login(UserDetails user)
        {
            // System.Diagnostics.Debugger.Launch();
            ViewBag.Message = "Your contact page.";
            var config = new MapperConfiguration(cfg =>
                     cfg.CreateMap<User, UserDetails>()
                 );
            string ipAddress = GetIP();

             IRestResponse<activeDirectoryUser> activeDirectoryUser = AuthenticateUser.GetUserInfoByUserNameAndPassword(user.USER_NAME, user.password);
            //  IRestResponse<activeDirectoryUser> activeDirectoryUser = AuthenticateUser.GetUserInfoByUserNameAndPassword(user.USER_NAME, "");

            //activeDirectoryUser activeDirectoryUser = AuthenticateUser.GetUserInfoByUserNameAndPassword(user.USER_NAME, user.password);

            if (activeDirectoryUser == null || activeDirectoryUser.Data == null)
            {
                log.Error("Your credentials is not valid as per active directory" + "  USER_NAME= " + user.USER_NAME + " &ipAddress=" + ipAddress);
                ModelState.AddModelError("CustomError", "Your credentials is not valid as per active directory USER_NAME=" + user.USER_NAME + " &ipAddress=" + ipAddress);
                return View(user);
            }

            if (activeDirectoryUser.Data.errorMessage != null && activeDirectoryUser.Data.errorMessage != "")
            {
                log.Error(activeDirectoryUser.Data.statusMessage + "  USER_NAME=" + user.USER_NAME + " &ipAddress=" + ipAddress);
                ModelState.AddModelError("CustomError", activeDirectoryUser.Data.statusMessage);
                return View(user);
            }

            if (activeDirectoryUser.Data.status != "Success")
            {
                ModelState.AddModelError("CustomError", "Your credentials is not valid as per active directory");
                log.Error(" Your credentials is not valid as per active directory  USER_NAME= " + user.USER_NAME + " &ipAddress=" + ipAddress);
                return View(user);
            }

            if (activeDirectoryUser.Data.mail == null || activeDirectoryUser.Data.mail == "")
            {
                log.Error(" Your Email is not defined in the active directory USER_NAME= " + user.USER_NAME + " &ipAddress=" + ipAddress);
                ModelState.AddModelError("CustomError", "Your Email is not defined in the active directory");
                return View(user);
            }

            var usrObject = _dbContext.Users.Where(x => x.EMAIL.ToUpper() == activeDirectoryUser.Data.mail.Trim().ToUpper()).FirstOrDefault();
            //var usrObject = _dbContext.Users.Where(x => x.EMAIL == "reach_psundaram@dm.gov.ae").FirstOrDefault();
            if (usrObject == null)
            {
                log.Error(" You are not authorized to access the application " + user.USER_NAME + " &ipAddress=" + ipAddress);
                ModelState.AddModelError("CustomError", "You are not authorized to access the application");
                return View(user);
            }
            else
            {
                log.Info(" login successfully  USER_NAME= " + user.USER_NAME + " & ipAddress=" + ipAddress);

                try
                {
                    //  Common.User  = usrObject;
                    //  var mapper = new Mapper(config);
                    UserDetails loggedinUser = new UserDetails();
                    if (usrObject != null)
                    {

                        if (usrObject.id > 0)
                        {
                            loggedinUser.id = usrObject.id;
                            loggedinUser.USER_NAME = usrObject.USER_NAME;
                            loggedinUser.NAME_ARABIC = usrObject.NAME_ARABIC;
                            loggedinUser.NAME_ENGLISH = usrObject.NAME_ENGLISH;
                            loggedinUser.EMAIL = usrObject.EMAIL;
                            loggedinUser.Has_Review_Role = usrObject.Has_Review_Role;
                            loggedinUser.Has_Manul_Update_Role_In_Dewa = usrObject.Has_Manul_Update_Role_In_Dewa;
                            loggedinUser.Created_By = usrObject.Created_By;
                            loggedinUser.Creation_Date = usrObject.Creation_Date;
                            loggedinUser.Updated_By = usrObject.Updated_By;
                            loggedinUser.Update_Date = usrObject.Update_Date;
                            loggedinUser.Has_Admin_Role = usrObject.Has_Admin_Role;
                            loggedinUser.Last_Login = usrObject.Last_Login;
                        }
                    }
                    Common.User = loggedinUser;
             
                    usrObject.Last_Login = DateTime.Now;

                    _dbContext.Entry(usrObject).State = EntityState.Modified;
                    _dbContext.SaveChanges();
          
                }
                catch(Exception ex)
                {
                    log.Error(ex.Message);
                }
                return RedirectToAction("Index");
            }

        }

        //https://www.youtube.com/watch?v=5omEuuIIFcg  (Pagination With ASP.NET MVC and Entity Framework, BootStrap)  //Install-Package PagedList.Mvc // Install-Package PagedList
        //https://www.c-sharpcorner.com/UploadFile/4d9083/creating-simple-grid-in-mvc-using-grid-mvc/
        [HttpGet]
        public ActionResult housingFeesData(bool reject = false, bool completed = false)
        {

            //System.Diagnostics.Debugger.Launch();

            List<RequestModifyHousingFeesViewModel> listOfRefundFines = null;
            var currentYear = DateTime.Now.Year;

            
            if (reject == false && completed == false)
            {
                listOfRefundFines = _dbContext.Request_Modify_DM_Housing_Fees
                    .Where(x => x.Status != Stauts.Rejected.ToString()
                    && x.Status != Stauts.Completed.ToString())
                    .Select(x => new RequestModifyHousingFeesViewModel
                    {
                        RequestorType = x.RequestorType,
                        AccountNumber = x.AccountNumber,
                        EmailAddress = x.EmailAddress,
                        MobileNumber = x.MobileNumber,
                        PremiseNumber = x.PremiseNumber,
                        Status = x.Status,
                        Id = x.Id,
                        approvedManualUpdateByUser = x.User.NAME_ENGLISH,
                        rejectedByUser = x.User3.NAME_ENGLISH,
                        lockedByUser = x.User2.NAME_ENGLISH,
                        approvedReviewerUser = x.User1.NAME_ENGLISH,
                        TransCount = _dbContext.Request_Modify_DM_Housing_Fees.Where(p => p.AccountNumber == x.AccountNumber).Count().ToString(),
                        Creation_Date = x.Creation_Date
                    
                    }).ToList();
            }

            PagedList<RequestModifyHousingFeesViewModel> model = null;
            if (listOfRefundFines != null && listOfRefundFines.Count() > 0)
            {
                model = new PagedList<RequestModifyHousingFeesViewModel>(listOfRefundFines, 1, listOfRefundFines.Count());
            }
            ViewBag.Message = "Refund Fine Data.";

            ViewBag.reject = reject;
            ViewBag.completed = completed;

            return View(model);
        }


        [HttpGet]
        public ActionResult housingFeesDataRejected(bool reject = false, bool completed = false)
        {
            List<RequestModifyHousingFeesViewModel> listOfRefundFines = null;
            var currentYear = DateTime.Now.Year;
            if (reject)
            { 
                listOfRefundFines = _dbContext.Request_Modify_DM_Housing_Fees
               .Where(x => x.Status == Stauts.Rejected.ToString() && x.Creation_Date.Year == currentYear)
               .Select(x => new RequestModifyHousingFeesViewModel
               {
                   RequestorType = x.RequestorType,
                   AccountNumber = x.AccountNumber,
                   EmailAddress = x.EmailAddress,
                   MobileNumber = x.MobileNumber,
                   PremiseNumber = x.PremiseNumber,
                   Status = x.Status,
                   Id = x.Id,
                   approvedManualUpdateByUser = x.User.NAME_ENGLISH,
                   rejectedByUser = x.User3.NAME_ENGLISH,
                   lockedByUser = x.User2.NAME_ENGLISH,
                   approvedReviewerUser = x.User1.NAME_ENGLISH,
                   TransCount = _dbContext.Request_Modify_DM_Housing_Fees.Where(p => p.AccountNumber == x.AccountNumber).Count().ToString(),
                   Creation_Date = x.Creation_Date,
                   Updated_Date = x.Updated_Date
               })
                .ToList();
            }

            PagedList<RequestModifyHousingFeesViewModel> model = null;
            if (listOfRefundFines != null && listOfRefundFines.Count() > 0)
            {
                model = new PagedList<RequestModifyHousingFeesViewModel>(listOfRefundFines, 1, listOfRefundFines.Count());
            }
            ViewBag.Message = "Rejected housing Fees Data.";

            ViewBag.reject = reject;
            ViewBag.completed = completed;

            return View(model);
        }


        //https://www.youtube.com/watch?v=5omEuuIIFcg  (Pagination With ASP.NET MVC and Entity Framework, BootStrap)  //Install-Package PagedList.Mvc // Install-Package PagedList
        //https://www.c-sharpcorner.com/UploadFile/4d9083/creating-simple-grid-in-mvc-using-grid-mvc/
        [HttpGet]
        public ActionResult housingFeesDataCompleted(bool reject = false, bool completed = false)
        {
            List<RequestModifyHousingFeesViewModel> listOfRefundFines = null;
            var currentYear = DateTime.Now.Year;

            if (completed)
            {
                listOfRefundFines = _dbContext.Request_Modify_DM_Housing_Fees
                .Where(x => x.Status == Stauts.Completed.ToString() && x.Creation_Date.Year == currentYear)
                .Select(x => new RequestModifyHousingFeesViewModel
                {
                    RequestorType = x.RequestorType,
                    AccountNumber = x.AccountNumber,
                    EmailAddress = x.EmailAddress,
                    MobileNumber = x.MobileNumber,
                    PremiseNumber = x.PremiseNumber,
                    Status = x.Status,
                    Id = x.Id,
                    approvedManualUpdateByUser = x.User.NAME_ENGLISH,
                    rejectedByUser = x.User3.NAME_ENGLISH,
                    lockedByUser = x.User2.NAME_ENGLISH,
                    approvedReviewerUser = x.User1.NAME_ENGLISH,
                    TransCount = _dbContext.Request_Modify_DM_Housing_Fees.Where(p => p.AccountNumber == x.AccountNumber).Count().ToString(),
                    Creation_Date = x.Creation_Date,
                    Updated_Date = x.Updated_Date
                })
                 .ToList();
            }

            PagedList<RequestModifyHousingFeesViewModel> model = null;
            if (listOfRefundFines != null && listOfRefundFines.Count() > 0)
            {
                model = new PagedList<RequestModifyHousingFeesViewModel>(listOfRefundFines, 1, listOfRefundFines.Count());
            }

            ViewBag.Message = "Completed housing Fees Data.";

            ViewBag.reject = reject;
            ViewBag.completed = completed;

            return View(model);
        }




        //https://www.youtube.com/watch?v=5omEuuIIFcg  (Pagination With ASP.NET MVC and Entity Framework, BootStrap)  //Install-Package PagedList.Mvc // Install-Package PagedList
        //https://www.c-sharpcorner.com/UploadFile/4d9083/creating-simple-grid-in-mvc-using-grid-mvc/
        [HttpPost]
        public ActionResult ReportsP(string fromdate, string todate, int theStatus)
        {
            //System.Diagnostics.Debugger.Launch();
            List<RequestModifyHousingFeesViewModel> listOfRefundFines = null;

            listOfRefundFines = _dbContext.Request_Modify_DM_Housing_Fees
            .Select(x => new RequestModifyHousingFeesViewModel
            {
                RequestorType = x.RequestorType,
                AccountNumber = x.AccountNumber,
                EmailAddress = x.EmailAddress,
                MobileNumber = x.MobileNumber,
                PremiseNumber = x.PremiseNumber,
                Status = x.Status,
                Id = x.Id
            }).ToList();



            if (!string.IsNullOrEmpty(fromdate) && fromdate!="0")
            {
                string[] datelist = fromdate.ToString().Split('-');
                DateTime frmdt = new DateTime(int.Parse(datelist[0]), int.Parse(datelist[1]), int.Parse(datelist[2]));
                

                listOfRefundFines = listOfRefundFines
           .Where(x => x.Creation_Date >= frmdt)
           .Select(x => new RequestModifyHousingFeesViewModel
           {
               RequestorType = x.RequestorType,
               AccountNumber = x.AccountNumber,
               EmailAddress = x.EmailAddress,
               MobileNumber = x.MobileNumber,
               PremiseNumber = x.PremiseNumber,
               Status = x.Status,
               Id = x.Id
           }).ToList();
            }

            if (!string.IsNullOrEmpty(todate) && todate != "0")
            {
                string[] datelist = todate.ToString().Split('-');
                DateTime tomdt = new DateTime(int.Parse(datelist[0]), int.Parse(datelist[1]), int.Parse(datelist[2]));

                listOfRefundFines = listOfRefundFines
          .Where(x => x.Creation_Date < tomdt)
          .Select(x => new RequestModifyHousingFeesViewModel
          {
              RequestorType = x.RequestorType,
              AccountNumber = x.AccountNumber,
              EmailAddress = x.EmailAddress,
              MobileNumber = x.MobileNumber,
              PremiseNumber = x.PremiseNumber,
              Status = x.Status,
              Id = x.Id
          }).ToList();
            }

            if (!string.IsNullOrEmpty(theStatus.ToString()) && theStatus !=0 && theStatus.ToString() != "0")
            {
                string sts = Stauts.PendingReview.ToString();
                if (theStatus == 2)
                {
                    sts = Stauts.Completed.ToString();
                }
                if (theStatus == 3)
                {
                    sts = Stauts.Rejected.ToString();
                }

                listOfRefundFines = listOfRefundFines
         .Where(x => x.Status == sts)
         .Select(x => new RequestModifyHousingFeesViewModel
         {
             RequestorType = x.RequestorType,
             AccountNumber = x.AccountNumber,
             EmailAddress = x.EmailAddress,
             MobileNumber = x.MobileNumber,
             PremiseNumber = x.PremiseNumber,
             Status = x.Status,
             Id = x.Id
         }).ToList();
            }



            PagedList<RequestModifyHousingFeesViewModel> model = null;
            if (listOfRefundFines != null && listOfRefundFines.Count() > 0)
            {
                model = new PagedList<RequestModifyHousingFeesViewModel>(listOfRefundFines, 1, listOfRefundFines.Count());
                Session["listOfRefundFines"] = listOfRefundFines;
            }
            ViewBag.Message = "housing Fees Details.";

            return View("Reports", model);
        }

        [HttpGet]
        public ActionResult Reports()
        {

            List<RequestModifyHousingFeesViewModel> listOfRefundFines = null;
            var currentYear = DateTime.Now.Year;
            listOfRefundFines = _dbContext.Request_Modify_DM_Housing_Fees
            .Where(x => x.Creation_Date.Year == currentYear)
            .Select(x => new RequestModifyHousingFeesViewModel
            {
                RequestorType = x.RequestorType,
                AccountNumber = x.AccountNumber,
                EmailAddress = x.EmailAddress,
                MobileNumber = x.MobileNumber,
                PremiseNumber = x.PremiseNumber,
                Status = x.Status,
                Id = x.Id,
                approvedManualUpdateByUser = x.User.NAME_ENGLISH,
                rejectedByUser = x.User3.NAME_ENGLISH,
                lockedByUser = x.User2.NAME_ENGLISH,
                approvedReviewerUser = x.User1.NAME_ENGLISH,
                TransCount = _dbContext.Request_Modify_DM_Housing_Fees.Where(p => p.AccountNumber == x.AccountNumber).Count().ToString(),
                Creation_Date = x.Creation_Date
            }).ToList();


            PagedList<RequestModifyHousingFeesViewModel> model = null;
            if (listOfRefundFines != null && listOfRefundFines.Count() > 0)
            {
                model = new PagedList<RequestModifyHousingFeesViewModel>(listOfRefundFines, 1, listOfRefundFines.Count());
                Session["listOfRefundFines"] = listOfRefundFines;
                //Tools tool = new Tools();
                //tool.ExportToExcel(listOfRefundFines);
            }
            ViewBag.Message = "housing Fees Details.";

            return View("Reports",model);
        }

        [HttpPost]
        public ActionResult ReportsXls(List<RequestModifyHousingFeesViewModel> listOfRefundFines)
        {
            
            Tools tool = new Tools();
            tool.ExportToExcel((List<RequestModifyHousingFeesViewModel>)Session["listOfRefundFines"]);
            
            return View(listOfRefundFines);
        }

        [HttpGet]
        public ActionResult DReports()
        {
            
            return View("DReports");
        }


        [HttpGet]
        public ActionResult getHousingFeesRequestData(int pageId)
        {
            System.Diagnostics.Debugger.Launch();
            RequestModifyHousingFeesViewModel refundFineData = _dbContext.Request_Modify_DM_Housing_Fees.Where(x => x.Id == pageId)
                  .Select(x => new RequestModifyHousingFeesViewModel
                  {
                          Id = x.Id,
                          RequestorType  = x.RequestorType,
                          AccountNumber  = x.AccountNumber,
                          NumberOfRooms  = x.NumberOfRooms,
                          EmailAddress   = x.EmailAddress,
                          PropertyType   = x.PropertyType,
                          PremiseNumber  = x.PremiseNumber,
                          MobileNumber = x.MobileNumber,
                          Remarks  = x.Remarks,
                          MakaniNumber = x.MakaniNumber,
                          PlotNumber = x.PlotNumber,
                          StreetNumber = x.StreetNumber,
                          StreetName = x.StreetName,
                          BuildingNumber = x.BuildingNumber,
                          OfficeNumber = x.OfficeNumber,
                          PointOfSample = x.PointOfSample,
                          EjariNumber=x.EjariNumber,
                          EjariRegisteredBy=x.EjariRegisteredBy,
                          OwnerName=x.OwnerName,
                          ContractAddress=x.ContractAddress,
                          ContractEndDate=x.ContractEndDate,
                          HousingFees=x.HousingFees,
                          Employee_Comment = x.Employee_Comment,
                          Status = x.Status,
                          Locked_By_User_Id = x.Locked_By_User_Id,
                          RejectionReason = x.RejectionReason ,
                          approvedReviewerUser =  x.User.NAME_ENGLISH,
                          rejectedByUser = x.User1.NAME_ENGLISH,
                          lockedByUser = x.User2.NAME_ENGLISH,
                          approvedManualUpdateByUser = x.User3.NAME_ENGLISH ,
                          PropertyId = x.PropertyId,
                          AreaId = x.AreaId,
                          communityPlaceId = x.communityPlaceId,
                          Area = x.Area,
                          Community_Places = x.Community_Places,
                          Property_Types = x.Property_Types
                  }).ToList().FirstOrDefault();


            ViewBag.Message = "Modify DM Housing Fees";
            return View(refundFineData);
        }

        [HttpPost]
        [ActionName("approveReviewStep")]
        //[ValidateAntiForgeryToken()]
        public ActionResult getHousingFeesRequestData(RequestModifyHousingFeesViewModel requestModifyHousingFeesdata)
        {
            
            var requestModifyHousingdataModel = _dbContext.Request_Modify_DM_Housing_Fees.Where(x => x.Id == requestModifyHousingFeesdata.Id).FirstOrDefault();
 
            requestModifyHousingdataModel.Status = Stauts.PendingDewa.ToString();
            requestModifyHousingdataModel.UpdatedBy = Common.User.USER_NAME;
            requestModifyHousingdataModel.Updated_Date = DateTime.Now;
            requestModifyHousingdataModel.Approved_Review_By_User_Id = Common.User.id;
            requestModifyHousingdataModel.Employee_Comment = requestModifyHousingFeesdata.Employee_Comment;
            //refundFinedataModel.Remarks = refundFineData.Remarks;
            _dbContext.Entry(requestModifyHousingdataModel).State = EntityState.Modified;
            _dbContext.SaveChanges();
            requestModifyHousingFeesdata.Status = Stauts.PendingDewa.ToString();
            requestModifyHousingFeesdata.Property_Types = requestModifyHousingdataModel.Property_Types;
            requestModifyHousingFeesdata.Area = requestModifyHousingdataModel.Area;
            requestModifyHousingFeesdata.Community_Places = requestModifyHousingdataModel.Community_Places;
            requestModifyHousingFeesdata.Remarks = requestModifyHousingdataModel.Remarks;

            try
            {
               
                EmailService emailService = new EmailService();
                string message = "<html><body> <b> The request with ref Num :" + requestModifyHousingFeesdata.Id + " Assigned to you, please do Manual Update In Dewa." +
                                  "</br>" +
                                  "You can the access the Project through : " +
                                  // Request.Url.AbsoluteUri + "/Home/Login" +
                                  WebConfigurationManager.AppSettings["adminProjectUrl"].ToString() +
                                  " </b></html></body>";
                string subject = "Modify DM Housing Fees Service" + " ref Num:" + requestModifyHousingFeesdata.Id;
                //emailService.sendEmail(auditorObject.EMAIL,
                //                       message,
                //                       subject);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.ToString());
            }

            return View("getHousingFeesRequestData", requestModifyHousingFeesdata);
        }


        [HttpPost]
        [ActionName("approveManualUpdate")]
        //[ValidateAntiForgeryToken()]
        public ActionResult getHousingFeesRequestData(RequestModifyHousingFeesViewModel requestModifyHousingFeesdata, float id = 0)
        {
            //System.Diagnostics.Debugger.Launch();
            var requestModifyHousingdataModel = _dbContext.Request_Modify_DM_Housing_Fees.Where(x => x.Id == requestModifyHousingFeesdata.Id).FirstOrDefault();
            requestModifyHousingdataModel.Status = Stauts.Completed.ToString();
            //requestModifyHousingdataModel.Remarks = requestModifyHousingFeesdata.Remarks;
            requestModifyHousingdataModel.UpdatedBy = Common.User.USER_NAME;
            requestModifyHousingdataModel.Updated_Date = DateTime.Now;
            requestModifyHousingdataModel.Approved_Manual_Update_By_User_Id = Common.User.id;
            requestModifyHousingdataModel.Employee_Comment = requestModifyHousingFeesdata.Employee_Comment;
            _dbContext.Entry(requestModifyHousingdataModel).State = EntityState.Modified;
            _dbContext.SaveChanges();
            requestModifyHousingFeesdata.Status = Stauts.Completed.ToString();
            requestModifyHousingFeesdata.Property_Types = requestModifyHousingdataModel.Property_Types;
            requestModifyHousingFeesdata.Area = requestModifyHousingdataModel.Area;
            requestModifyHousingFeesdata.Community_Places = requestModifyHousingdataModel.Community_Places;


            requestModifyHousingFeesdata.Remarks = requestModifyHousingdataModel.Remarks;

            try
            {
                string XsEmpCom = "";
                string empcom = requestModifyHousingFeesdata.Employee_Comment;
                if (!string.IsNullOrEmpty(empcom))
                {
                    XsEmpCom = "<br /><br /><span style='color:darkred;'>Remarks : " + requestModifyHousingFeesdata.Employee_Comment + "</span>";
                }
                
                EmailService emailService = new EmailService();
                SMSService smsService = new SMSService();
                string message = "<html><body><b> Congratulations your Modification Housing Fees Request Completed successfully with ref Num :" + requestModifyHousingFeesdata.Id + ",The Amount will be updated In DEWA" + " </b>"+ XsEmpCom + "</html></body>";
                string subject = "Modification Housing Fees Service" + " ref Num:" + requestModifyHousingFeesdata.Id;
                emailService.sendEmail(requestModifyHousingFeesdata.EmailAddress,
                                       message,
                                       subject);

                smsService.sendSMS("Congratulations your Modification Housing Fees Request Completed successfully with ref Num :"
                                    + requestModifyHousingFeesdata.Id + ", The Amount will be updated In DEWA, Kindly Check your mail for more details."
                                    , requestModifyHousingFeesdata.MobileNumber);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.ToString());
            }

            return View("getHousingFeesRequestData", requestModifyHousingFeesdata);    
        }

        [HttpPost]
        [ActionName("lockRecord")]
        //[ValidateAntiForgeryToken()]
        public ActionResult getHousingFeesRequestData(RequestModifyHousingFeesViewModel requestModifyHousingFeesdata, int id = 0)
        {
           
            var requestModifyHousingdataModel = _dbContext.Request_Modify_DM_Housing_Fees.Where(x => x.Id == requestModifyHousingFeesdata.Id).FirstOrDefault();

            //requestModifyHousingdataModel.Remarks = requestModifyHousingFeesdata.Remarks;


            //requestModifyHousingdataModel.Employee_Comment = requestModifyHousingFeesdata.Employee_Comment;

            requestModifyHousingdataModel.UpdatedBy = Common.User.USER_NAME;
            requestModifyHousingdataModel.Updated_Date = DateTime.Now;
            requestModifyHousingdataModel.Locked_By_User_Id = Common.User.id;

            _dbContext.Entry(requestModifyHousingdataModel).State = EntityState.Modified;
            _dbContext.SaveChanges();
            requestModifyHousingFeesdata.Locked_By_User_Id = Common.User.id;
            requestModifyHousingFeesdata.Property_Types = requestModifyHousingdataModel.Property_Types;
            requestModifyHousingFeesdata.Area = requestModifyHousingdataModel.Area;
            requestModifyHousingFeesdata.Community_Places = requestModifyHousingdataModel.Community_Places;



            requestModifyHousingFeesdata.Remarks = requestModifyHousingdataModel.Remarks;
             

            try
            {

            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.ToString());
            }

            return View("getHousingFeesRequestData", requestModifyHousingFeesdata);
        }


        [HttpPost]
        [ActionName("unlockRecord")]
        //[ValidateAntiForgeryToken()]
        public ActionResult getHousingFeesRequestData(RequestModifyHousingFeesViewModel requestModifyHousingFeesdata, double id = 0)
        {
           // System.Diagnostics.Debugger.Launch();
            var requestModifyHousingdataModel = _dbContext.Request_Modify_DM_Housing_Fees.Where(x => x.Id == requestModifyHousingFeesdata.Id).FirstOrDefault();

            //requestModifyHousingdataModel.Remarks = requestModifyHousingFeesdata.Remarks;
            requestModifyHousingdataModel.UpdatedBy = Common.User.USER_NAME;
            requestModifyHousingdataModel.Updated_Date = DateTime.Now;
            requestModifyHousingdataModel.Locked_By_User_Id = null;

            _dbContext.Entry(requestModifyHousingdataModel).State = EntityState.Modified;
            _dbContext.SaveChanges();
            requestModifyHousingFeesdata.Locked_By_User_Id = null;
            requestModifyHousingFeesdata.Property_Types = requestModifyHousingdataModel.Property_Types;
            requestModifyHousingFeesdata.Area = requestModifyHousingdataModel.Area;
            requestModifyHousingFeesdata.Community_Places = requestModifyHousingdataModel.Community_Places;



            requestModifyHousingFeesdata.Remarks = requestModifyHousingdataModel.Remarks;
            try
            {

            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.ToString());
            }

            return View("getHousingFeesRequestData", requestModifyHousingFeesdata);
        }


        [HttpPost]
        [ActionName("rejectRecord")]
        //[ValidateAntiForgeryToken()]
        public ActionResult getHousingFeesRequestData(RequestModifyHousingFeesViewModel requestModifyHousingFeesViewModel, bool reject)
        {
            var modifyHousingFeesModel = _dbContext.Request_Modify_DM_Housing_Fees.Where(x => x.Id == requestModifyHousingFeesViewModel.Id).FirstOrDefault();
            modifyHousingFeesModel.Status = Stauts.Rejected.ToString();
            modifyHousingFeesModel.UpdatedBy = Common.User.USER_NAME;
            modifyHousingFeesModel.Employee_Comment = requestModifyHousingFeesViewModel.Employee_Comment;
            modifyHousingFeesModel.Updated_Date = DateTime.Now;
            modifyHousingFeesModel.Rejected_By_User_Id = Common.User.id;
            modifyHousingFeesModel.RejectionReason = requestModifyHousingFeesViewModel.RejectionReason;
            _dbContext.Entry(modifyHousingFeesModel).State = EntityState.Modified;
            _dbContext.SaveChanges();

            try
            {
                string XsEmpCom = "";
                string empcom = requestModifyHousingFeesViewModel.Employee_Comment;
                if (!string.IsNullOrEmpty(empcom))
                {
                    XsEmpCom = "<br /><br /><span style='color:darkred;'>Remarks : " + requestModifyHousingFeesViewModel.Employee_Comment + "</span>";
                }

                EmailService emailService = new EmailService();
                string rejectionReason = "";
                if (!string.IsNullOrEmpty(requestModifyHousingFeesViewModel.RejectionReason))
                {
                    rejectionReason = "</br> rejection Reason: " + requestModifyHousingFeesViewModel.RejectionReason;
                }
                string message = "<html><body><b> Unfortunately your request with ref number  :" + requestModifyHousingFeesViewModel.Id + ", has been rejected." + rejectionReason + " </b> "+ XsEmpCom + "</html></body>";
                string subject = "Modification Housing Fees Service" + " ref Num:" + requestModifyHousingFeesViewModel.Id;
                emailService.sendEmail(requestModifyHousingFeesViewModel.EmailAddress,
                                       message,
                                       subject);

                SMSService smsService = new SMSService();
                smsService.sendSMS("Unfortunately Modification Housing Fees request with ref number " + requestModifyHousingFeesViewModel.Id + ", has been rejected, Kindly Check your mail for more details.",
                                    requestModifyHousingFeesViewModel.MobileNumber.ToString());
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.ToString());
            }


            return RedirectToAction("housingFeesData");
        }


        [HttpGet]
        public ActionResult getModificationHousingFeesImage(int pageId, Boolean isCopyOfDewaBill = false, Boolean isCopyOfEjariCertificate = false, Boolean isLetterFromOwnerMentioningMeters = false, Boolean isTitleDeed = false, Boolean isLayoutAttachmentBinary = false)
        {
            //System.Diagnostics.Debugger.Launch();
            RequestModifyHousingFeesViewModel refundFineData = null;
            string data = "";
            string mimeType = "";
            string filePath = "";
            string attachmentPath = WebConfigurationManager.AppSettings["AttachmentPath"].ToString();

            if (isCopyOfDewaBill)
            {
                refundFineData = _dbContext.Request_Modify_DM_Housing_Fees.Where(x => x.Id == pageId).Select(x => new RequestModifyHousingFeesViewModel { 
                    MimeTypeCopyOfDewaBill = x.MimeTypeCopyOfDewaBill, CopyOfDewaBill_FileName = x.CopyOfDewaBill_FileName }).ToList().FirstOrDefault();
                //data = "data:" + refundFineData.MimeTypeCopyOfDewaBill + ";base64," + Convert.ToBase64String(refundFineData.CopyOfDewaBill);
                //data = Convert.ToBase64String(refundFineData.CopyOfDewaBill);
                filePath = attachmentPath + refundFineData.CopyOfDewaBill_FileName;
                data = Convert.ToBase64String(GetBinaryFile(filePath));
                mimeType = refundFineData.MimeTypeCopyOfDewaBill;
            }

            if (isCopyOfEjariCertificate)
            {
                refundFineData = _dbContext.Request_Modify_DM_Housing_Fees.Where(x => x.Id == pageId).Select(x => new RequestModifyHousingFeesViewModel { MimeTypeCopyOfEjariCertificate = x.MimeTypeCopyOfEjariCertificate, CopyOfEjariCertificate_FileName = x.CopyOfEjariCertificate_FileName }).ToList().FirstOrDefault();
                //data = "data:" + refundFineData.MimeTypeCopyOfEjariCertificate + ";base64," + Convert.ToBase64String(refundFineData.CopyOfEjariCertificate);
                //data = Convert.ToBase64String(refundFineData.CopyOfEjariCertificate);
                filePath = attachmentPath + refundFineData.CopyOfEjariCertificate_FileName;
                data = Convert.ToBase64String(GetBinaryFile(filePath));
                mimeType = refundFineData.MimeTypeCopyOfEjariCertificate;
            }

            if (isLetterFromOwnerMentioningMeters)
            {
                refundFineData = _dbContext.Request_Modify_DM_Housing_Fees.Where(x => x.Id == pageId).Select(x => new RequestModifyHousingFeesViewModel { MimeTypeLetterFromOwnerMentioningMeters = x.MimeTypeLetterFromOwnerMentioningMeters, LetterFromOwnerMentioningMeters_FileName = x.LetterFromOwnerMentioningMeters_FileName }).ToList().FirstOrDefault();
                //data = "data:" + refundFineData.MimeTypeLetterFromOwnerMentioningMeters + ";base64," + Convert.ToBase64String(refundFineData.LetterFromOwnerMentioningMeters);
                //data = Convert.ToBase64String(refundFineData.LetterFromOwnerMentioningMeters);
                filePath = attachmentPath + refundFineData.LetterFromOwnerMentioningMeters_FileName;
                data = Convert.ToBase64String(GetBinaryFile(filePath));
                mimeType = refundFineData.MimeTypeLetterFromOwnerMentioningMeters;
            }

            if (isTitleDeed)
            {
                refundFineData = _dbContext.Request_Modify_DM_Housing_Fees.Where(x => x.Id == pageId).Select(x => new RequestModifyHousingFeesViewModel { MimeTypeTitleDeed = x.MimeTypeTitleDeed, TitleDeed_FileName = x.TitleDeed_FileName }).ToList().FirstOrDefault();
                //data = "data:" + refundFineData.MimeTypeTitleDeed + ";base64," + Convert.ToBase64String(refundFineData.TitleDeed);
                //data = Convert.ToBase64String(refundFineData.TitleDeed);
                filePath = attachmentPath + refundFineData.TitleDeed_FileName;
                data = Convert.ToBase64String(GetBinaryFile(filePath));
                mimeType = refundFineData.MimeTypeTitleDeed;
            }


            if (isLayoutAttachmentBinary)
            {
                refundFineData = _dbContext.Request_Modify_DM_Housing_Fees.Where(x => x.Id == pageId).Select(x => new RequestModifyHousingFeesViewModel { MimeTypeLayout = x.MimeTypeLayout, LayoutAttachment_FileName = x.LayoutAttachment_FileName}).ToList().FirstOrDefault();
                //data = "data:" + refundFineData.MimeTypeLayout + ";base64," + Convert.ToBase64String(refundFineData.LayoutAttachmentBinary);
                //data = Convert.ToBase64String(refundFineData.LayoutAttachmentBinary);
                filePath = attachmentPath + refundFineData.LayoutAttachment_FileName;
                data = Convert.ToBase64String(GetBinaryFile(filePath));
                mimeType = refundFineData.MimeTypeLayout;
            }

            ViewBag.Message = "Refund Fine Data.";
            ViewBag.mimeType = mimeType;
            ViewBag.data = data;
            ViewBag.filePath = filePath;
            return View();
        }

        public static string GetIP()
        {
            string ip = "";
            try
            {
                string hostnameip = System.Web.HttpContext.Current.Request.UserHostAddress.ToString() + " Client System: " + System.Web.HttpContext.Current.Request.UserAgent.ToString();
                ip = System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];

                if (string.IsNullOrEmpty(ip))
                {
                    ip = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                }
            }
            catch (Exception e)
            {
                ip = "";
            }
            return ip;
        }

        private byte[] GetBinaryFile(String filename)
        {
            byte[] bytes;
            if (!System.IO.File.Exists(filename))
            {
                throw new Exception("this file not exist " + filename);
            }
            using (FileStream file = new FileStream(filename, FileMode.Open, FileAccess.Read))
            {
                bytes = new byte[file.Length];
                file.Read(bytes, 0, (int)file.Length);
            }
            return bytes;
        }

        [HttpPost]
        public JsonResult getHousingFeesDetails(string accountNumber)
        {
            HousingFeesViewModel housingFeesViewModel = new HousingFeesViewModel();
           
                var url = ConfigurationManager.AppSettings["HousingFees_URL"];
                var APIKey = ConfigurationManager.AppSettings["HousingFees_APIKEY"];
                url = url + "?APIKey=" + APIKey;
                var json = JsonConvert.SerializeObject(new { MTO_HousingFee_Req = new { ContractAccount = accountNumber } });

                using (var client = new HttpClient())
                {


                    var data = new StringContent(json, Encoding.UTF8,
                                        "application/json");

                    //POST Method     
                    HttpResponseMessage response = client.PostAsync(url, data).Result;

                    if (response.IsSuccessStatusCode)
                    {
                        var jsonString = response.Content.ReadAsStringAsync().Result;

                        housingFeesViewModel = JsonConvert.DeserializeObject<HousingFeesViewModel>(jsonString);
                    }

                if (housingFeesViewModel?.MTO_HousingFee_Resp != null)
                {
                    if (housingFeesViewModel?.MTO_HousingFee_Resp.Item != null)
                    {
                        if (housingFeesViewModel.MTO_HousingFee_Resp.Item.Record != null)
                        { 
                            List<Record> records = housingFeesViewModel.MTO_HousingFee_Resp?.Item?.Record.OrderByDescending(c => c.Item.DueDate).ToList();

                        
                        housingFeesViewModel.MTO_HousingFee_Resp.Item.Record = records; }
                    }
                }
                  

                }
           
            return Json(housingFeesViewModel);

        }

        [HttpPost]
        public JsonResult getEjariDetails(string ejariNumber)
        {
            EjariDetailsViewModel ejariDetailsViewModel = new EjariDetailsViewModel();
           
               
                var url = ConfigurationManager.AppSettings["Ejari_URL"];
                var APIKey = ConfigurationManager.AppSettings["Ejari_APIKEY"];
                url = url + "?APIKey=" + APIKey;
                var json = JsonConvert.SerializeObject(new { contractNumber = ejariNumber });

                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("x-Gateway-APIKey", APIKey);
                    var data = new StringContent(json, Encoding.UTF8, "application/json");



                    //POST Method     
                    HttpResponseMessage response = client.PostAsync(url, data).Result;

                    if (response.IsSuccessStatusCode)
                    {
                        var jsonString = response.Content.ReadAsStringAsync().Result;

                        ejariDetailsViewModel = JsonConvert.DeserializeObject<EjariDetailsViewModel>(jsonString);
                    }
                }
           


            return Json(ejariDetailsViewModel);
        }
    }
}