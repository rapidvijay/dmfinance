using AdminRefundFines.Helpers;
using ModificationHousingFeesAdmin.Helpers;
using System.Web;
using System.Web.Mvc;

namespace ModificationHousingFeesAdmin
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
            filters.Add(new AuthorizeFilterAttribute());
            filters.Add(new HandleError());
            filters.Add(new ElmahHandledErrorLoggerFilter());
        }
    }
}
