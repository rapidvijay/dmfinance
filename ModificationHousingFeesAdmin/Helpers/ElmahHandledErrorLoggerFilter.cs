using Elmah;
using ModificationHousingFeesAdmin.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AdminRefundFines.Helpers
{
    public class ElmahHandledErrorLoggerFilter : IExceptionFilter
    {
        private static ErrorFilterConfiguration _config;

        public void OnException(ExceptionContext context)
        {
            //if (context.ExceptionHandled) //The unhandled ones will be picked by the elmah module
            //{
            //    var e = context.Exception;
            //    var context2 = context.HttpContext.ApplicationInstance.Context;
            //    //TODO: Add additional variables to context.HttpContext.Request.ServerVariables for both handled and unhandled exceptions
            //    if ((context2 == null) || (!_RaiseErrorSignal(e, context2) && !_IsFiltered(e, context2)))
            //    {
            //        _LogException(e, context2);
            //    }
            //}
            if (!context.ExceptionHandled)
            {


                HousingFeesExceptionLog logger = new HousingFeesExceptionLog()
                {
                    ExceptionMsg = context.RouteData.Values["controller"].ToString(),
                    ExceptionType = $"{"Admin"}{"|"}{context.GetType().Name.ToString()}",
                    ExceptionSource = $"{context.Exception.Message}{"|"} { context.Exception.StackTrace}",
                    ExceptionURL = context.HttpContext.Request.Url?.ToString(),

                    Logdate = DateTime.Now
                };
                ModificationHousingFeesEntities _dbContext = new ModificationHousingFeesEntities();

                _dbContext.HousingFeesExceptionLogs.Add(logger);
                _dbContext.SaveChanges();

                context.ExceptionHandled = true;

            }
        }

        private static bool _IsFiltered(System.Exception e, System.Web.HttpContext context)
        {
            if (_config == null)
            {
                _config = (context.GetSection("elmah/errorFilter") as ErrorFilterConfiguration) ?? new ErrorFilterConfiguration();
            }
            var context2 = new ErrorFilterModule.AssertionHelperContext((System.Exception)e, context);
            return _config.Assertion.Test(context2);
        }

        private static void _LogException(System.Exception e, System.Web.HttpContext context)
        {
            ErrorLog.GetDefault((System.Web.HttpContext)context).Log(new Elmah.Error((System.Exception)e, (System.Web.HttpContext)context));
        }


        private static bool _RaiseErrorSignal(System.Exception e, System.Web.HttpContext context)
        {
            var signal = ErrorSignal.FromContext((System.Web.HttpContext)context);
            if (signal == null)
            {
                return false;
            }
            signal.Raise((System.Exception)e, (System.Web.HttpContext)context);
            return true;
        }
    }
    

  
}