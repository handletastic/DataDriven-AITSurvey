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

            //get question from DB
            //get connection string from webconfig
            string connectionString = ConfigurationManager.ConnectionStrings["testConnection"].ConnectionString;

            //build sql connectoin, use connection string, open(connect)
            SqlConnection connection = new SqlConnection();
            connection.ConnectionString = connectionString;
            connection.Open();

            //build sql command to get current question
            //joining two tables because TestQuestion table has a foreign key column for type, but I want to get the type name as a string which is on the TestQuestionType table
            SqlCommand command = new SqlCommand("SELECT * FROM TestQuestion, TestQuestionType WHERE TestQuestion.questionType = TestQuestionType.typeID AND TestQuestion.questionId = " + currentQuestion, connection);

            //run command, store results in SqlDataReader
            SqlDataReader reader = command.ExecuteReader();

            //loop through the rows of results
            while (reader.Read())
            {
                //get question text and type from this row of results
                string questionText = reader["text"].ToString(); // "text" is a column from the TestQuestion table
                string questionType = reader["typeName"].ToString(); // "typeName" is a column from the TestQuestionType table

                //using type, load up correct web user control (e.g: checkbox question)
                if (questionType.Equals("TextBox"))
                {
                    //load the control up
                    TextboxQuestionControl textboxControl = (TextboxQuestionControl)LoadControl("~/TextboxQuestionControl.ascx");
                    //set its ID
                    textboxControl.ID = "textboxQuestionControl";
                    //set its question text label
                    textboxControl.QuestionLabel.Text = questionText;

                    //add it to the placeholder on our webpage
                    questionPlaceHolder.Controls.Add(textboxControl);
                }
                else if (questionType.Equals("CheckBox"))
                {
                    //load the control up
                    CheckBoxQuestionControl checkBoxQuestion = (CheckBoxQuestionControl)LoadControl("~/CheckBoxQuestionControl.ascx");
                    //set its ID
                    checkBoxQuestion.ID = "checkBoxQuestion";
                    //set text label to question text
                    checkBoxQuestion.QuestionLabel.Text = questionText;

                    //we need to talk to the database to get all of the options for this question to display
                    //e.g: what gender? then options like male, female etc...
                    SqlCommand optionCommand = new SqlCommand(
                        "SELECT * FROM TestQuestionOption WHERE questionId = " + currentQuestion, connection);

                    //run command, get ready to read results
                    SqlDataReader optionReader = optionCommand.ExecuteReader();

                    //cycle through rows of options
                    while (optionReader.Read())
                    {
                        //for every row, we will build a list item representing it
                        //                           display member(for ui),          value member(for devs to store if selected)
                        ListItem item = new ListItem(optionReader["text"].ToString(), optionReader["optionId"].ToString());
                        //add item to our checkbox list
                        checkBoxQuestion.QuestionCheckBoxList.Items.Add(item);
                    }
                    //finally have all the checkbox list items populated, add our checkbox question control to the placeholder
                    questionPlaceHolder.Controls.Add(checkBoxQuestion);
                }
                //TODO else if radio
                //TODO else if dropdown
            }

            connection.Close();
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