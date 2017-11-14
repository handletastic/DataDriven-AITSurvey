using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SQL_Connection
{
    public class SessionHelper
    {
        public static void setUserName(string username)
        {
            //to set a session variable, set a value within its associative array
            //if array slot doesn't exist, it will make one for it
            HttpContext.Current.Session["UserName"] = username;
        }

        public static string getUserName()
        {
            if (HttpContext.Current.Session["UserName"] == null)
                return "";
            else
                return (string)HttpContext.Current.Session["UserName"];
        }

        public static bool IsLoggedIn()
        {
            string username = getUserName();
            if (username != null && username.Length > 0)
                return true;
            else
                return false;
        }

    }
}