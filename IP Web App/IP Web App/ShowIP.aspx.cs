using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace IP_Web_App
{
    public partial class ShowIP : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //TODO set label text with your IP
        }

        //this method is accurate when working out client IPs but not reliable for collecting localhosts ip if selfhosting and testing on the same machine
        protected string GetIPAddress()
        {
            //get ip if through proxy
            //=======================
            System.Web.HttpContext context = System.Web.HttpContext.Current;
            string ipAddress = context.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];

            //if not null or not empty, then we can find the ip
            if (!string.IsNullOrEmpty(ipAddress))
            {
                //likely it will be a big string broken up with lots of commas, so we split into an array
                string[] proxyAddress = ipAddress.Split(',');
                if (proxyAddress.Length > 0)
                {
                    return proxyAddress[0]; //and return the first value in our array
                }
            }

            //if not proxy, get nice and easy ip
            //get ip from web http request
            //=======================
            ipAddress = context.Request.UserHostAddress; //if proxy, this ip would be the ip for the proxy server, 
                                                         //which is why we tested above
            if (ipAddress.Trim() != "::1")//check to make sure its not localhost
            {
                //if not, good, ip found
                return ipAddress;
            }
            else
            {
                //gotta get local ip address
                string stringHostName = System.Net.Dns.GetHostName();
                //get ip host entry by host name
                System.Net.IPHostEntry ipHostEntries = System.Net.Dns.GetHostEntry(stringHostName);
                //get ip address from the ip host entry address list
                try
                {
                    //if 2 slots in addresslist, ip will be in second slot(index 1)
                    ipAddress = ipHostEntries.AddressList[1].ToString();
                }
                catch (Exception)
                {
                    //if not, then try first slot
                    try
                    {
                        ipAddress = ipHostEntries.AddressList[0].ToString();
                    }
                    catch (Exception)
                    {
                        //failing that completely, go with local host ip
                        ipAddress = "127.0.0.1";
                    }
                }
            }

            return ipAddress;

        }
    }
}