using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using System.Configuration;

namespace SQL_Connection
{
    public partial class Question : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!SessionHelper.IsLoggedIn())
            {
                Response.Redirect("Login.aspx");
                return; //make sure that the rest of method does not run
            }
            else
            {
                usernameLabel.Text = SessionHelper.getUserName();
            }

            //check session state to see what question we're up to
            int currentQuestion; //TODO for assignment, don't use default values, ask the DB for first question instead
            if (HttpContext.Current.Session["questionNumber"] != null)
            {
                // we have a question numebr stored in the session, we should use that value for current question
                currentQuestion = (int)HttpContext.Current.Session["questionNumber"];//session stores Objects, cast to int
            }
            else
            {
                //no value stored in session, set it to our first question number
                currentQuestion = 1;
                HttpContext.Current.Session["questionNumber"] = currentQuestion; //now store the previously set currentQuestion to the session, so that when we run this the second time it will run the if part of this statement
            }

            //check session state to see what question we're up to
            //get question from DB
            //IF question is checkBox, radio or dropdownList
                //get list of options from DB too
            //IF checkbox type of question
            CheckBoxQuestionControl checkBoxQuestion = (CheckBoxQuestionControl)LoadControl("~/CheckBoxQuestionControl.ascx");
            checkBoxQuestion.ID = "checkBoxQuestion";
            checkBoxQuestion.QuestionLabel.Text = "Select weapons you wish to buy:";
            //Get options/choices for this checkbox question from DB
            //add all choices as items to checkbox list
            ListItem item1 = new ListItem("AK47", "weap1");
            ListItem item2 = new ListItem("SMG", "weap2");
            ListItem item3 = new ListItem("Knife", "weap3");
            ListItem item4 = new ListItem("Nuke", "weap4");

            checkBoxQuestion.QuestionCheckBoxList.Items.Add(item1);
            checkBoxQuestion.QuestionCheckBoxList.Items.Add(item2);
            checkBoxQuestion.QuestionCheckBoxList.Items.Add(item3);
            checkBoxQuestion.QuestionCheckBoxList.Items.Add(item4);

            //ADD checkbox question control to the web page
            questionPlaceHolder.Controls.Add(checkBoxQuestion);

         }

        protected void nextButton_Click(object sender, EventArgs e)
        {
            //lets try to find checkb box question control in webpage
            CheckBoxQuestionControl checkBoxQuestion = (CheckBoxQuestionControl)questionPlaceHolder.FindControl("checkBoxQuestion");
            if (checkBoxQuestion != null)
            {
                //then its a checkbox question, lets process answers

                //empty list of shown answers in bullet list
                selectedAnswerBulletedList.Items.Clear();

                //for each selected item, add to bullet list
                foreach (ListItem option in checkBoxQuestion.QuestionCheckBoxList.Items)
                {
                    if (option.Selected)
                    {
                        //TODO add answer to session or DB

                        //for now we will use a local solution
                        selectedAnswerBulletedList.Items.Add(option);
                    }
                }
            }
        }
    }
}