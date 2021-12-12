using ModificationHousingFeesAdmin.Models;
using ModificationHousingFeesAdmin.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ModificationHousingFeesAdmin
{
    public static class Common
    {
        public static UserDetails User
        {
            get
            {
                if (HttpContext.Current.Session["usrObject"] == null)
                    return null;
                else
                    return HttpContext.Current.Session["usrObject"] as UserDetails;
            }
            set
            {
                HttpContext.Current.Session["usrObject"] = value;
            }
        }
    }
}