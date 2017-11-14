using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SQL_Connection
{
    public partial class LifeCycle : System.Web.UI.Page
    {
        protected void Page_PreInit(object sender, EventArgs e)
        {
            Response.Write("Page Pre Initiate <br/>");
        }

        protected void Page_Init(object sender, EventArgs e)
        {
            Response.Write("Page Initiate <br/>");
        }

        protected void Page_InitComplete(object sender, EventArgs e)
        {
            Response.Write("Page Initiate Complete <br/>");
        }

        protected void Page_PreLoad(object sender, EventArgs e)
        {
            Response.Write("Page Pre Load <br/>");
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            //If have dynamically added html intems, rebuild them here each time
            //If viewState exists (stuff clicked on, typed into, filled in etc), those values are copie into dynamic objects at the end of Page_Load
            Response.Write("Page Load <br/>");

            //If want to do something on page first visit and not on refresh
            if (!IsPostBack)
            {
                Response.Write("Page First Load <br/>");
            }
            else
            {
                Response.Write("Page Reloaded <br/>");
            }
        }

        protected void Page_LoadComplete(object sender, EventArgs e)
        {
            Response.Write("Page Load Complete <br/>");
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            Response.Write("Page Pre Render<br/>");
        }

        protected void Page_PreRenderComplete(object sender, EventArgs e)
        {
            Response.Write("Page Pre Render Complete<br/>");
        }

        

        protected void Button1_Click(object sender, EventArgs e)
        {
            Label1.Text = "You've pressed a button<br/>";
        }
    }
}