using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SQL_Connection
{
    public partial class Login : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void loginButton_Click(object sender, EventArgs e)
        {
            //check if username has a value
            //TODO check username/password against the actual DB
            if (userNameTextBox.Text.Length > 0)
            {
                //set username in session
                SessionHelper.setUserName(userNameTextBox.Text);
                //go to question page
                Response.Redirect("Question.aspx");
            }
        }
    }
}