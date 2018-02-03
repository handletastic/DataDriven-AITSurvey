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
        private int currentQuestionId;
        private List<int> followUpQuestionIdList;
        private List<int> selectedOptionsIdList;

        protected void Page_Load(object sender, EventArgs e)
        {
            this.followUpQuestionIdList = new List<int>();
            if (HttpContext.Current.Session["followUpQuestionIdList"] != null)
            {
                this.followUpQuestionIdList = (List<int>)HttpContext.Current.Session["followUpQuestionIdList"];
            }

            if (HttpContext.Current.Session["currentQuestionId"] != null)
            {
                this.currentQuestionId = Int32.Parse(HttpContext.Current.Session["currentQuestionId"].ToString());
            }
            else {
                this.currentQuestionId = GetCurrentQuestionID();
            }

            

            this.selectedOptionsIdList = new List<int>();
            if (HttpContext.Current.Session["selectedOptionsIdList"] != null)
            {
                this.followUpQuestionIdList = (List<int>)HttpContext.Current.Session["selectedOptionsIdList"];
            }
            
            //TODO - load a login page if user is not logged in already
            #region Login Page
            //if not logged-in go to login page
            /*if (!SessionHelper.IsLoggedIn())
            {
                Response.Redirect("Login.aspx");
                return; //make sure that the rest of method does not run
            }
            else
            {
                usernameLabel.Text = SessionHelper.getUserName();
            }
            */
            #endregion
            
            QuestionLoader(this.currentQuestionId);
        }

        private SqlConnection ConnectToSqlDb()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["testConnection"].ConnectionString;
            SqlConnection connection = new SqlConnection();
            connection.ConnectionString = connectionString;
            connection.Open();
            return connection;
        }

        //OK
        private void UpdateFollowUpListOnSession() {
            HttpContext.Current.Session["followUpQuestionIdList"] = this.followUpQuestionIdList;
        }

        //OK
        private int GetCurrentQuestionID()
        {
            int questionId = 0;

            if (this.followUpQuestionIdList.Count > 0)
            {
                questionId = this.followUpQuestionIdList.First();
                this.followUpQuestionIdList.RemoveAt(0);
                UpdateFollowUpListOnSession();
            }
            else if (currentQuestionId > 0)
            {
                SqlConnection connection = ConnectToSqlDb();
                SqlCommand command = new SqlCommand("SELECT * FROM questions where id = " + this.currentQuestionId, connection);
                SqlDataReader reader = command.ExecuteReader();

                reader.Read();
                currentQuestionId = (int)reader["next_question_id"];
                questionId = currentQuestionId;
                HttpContext.Current.Session["currentQuestionId"] = currentQuestionId;
                connection.Close();
            }
            else
            {
                SqlConnection connection = ConnectToSqlDb();
                SqlCommand getFirstQuestionID = new SqlCommand("SELECT min(id) FROM questions", connection);
                SqlDataReader reader = getFirstQuestionID.ExecuteReader();
                reader.Read();
                questionId = Int32.Parse(reader.GetValue(0).ToString());
                connection.Close();
                HttpContext.Current.Session["currentQuestionId"] = questionId;
            }
            return questionId;
        }

        //OK
        private List<Answer> GetListOfAnswersFromSession()
        {
            List<Answer> answers = new List<Answer>();
            if (HttpContext.Current.Session["sessionAnswers"] != null)
                answers = (List<Answer>)HttpContext.Current.Session["sessionAnswers"];
            return answers;
        }

        //OK
        private Int32 GetFollowUpQuestionID(int _option_id)
        {
            SqlConnection connection = ConnectToSqlDb();
            SqlCommand getFollowUpID = new SqlCommand("SELECT followup_question_id FROM options WHERE id = " + _option_id, connection);
            SqlDataReader reader = getFollowUpID.ExecuteReader();

            int followUpQuestionID = 0;

            if (reader.HasRows && reader.Read() && !reader.IsDBNull(0))
            {
                followUpQuestionID = Int32.Parse(reader.GetValue(0).ToString());
            }                

            connection.Close();
            return followUpQuestionID;
        }

        //OK
        private void CheckAndAddFollowUpQuestionToSession(int _option_id)
        {   
            int followUpFromOption = GetFollowUpQuestionID(_option_id);
            if (followUpFromOption > 0)
            {
                this.followUpQuestionIdList.Add(followUpFromOption);
                UpdateFollowUpListOnSession();
            }
        }

        private string GetCurrentQuestionText(int _currentQuestionID)
        {
            SqlConnection connection = ConnectToSqlDb();
            SqlCommand getCurrentQuestionAndItsQuestionType = new SqlCommand("SELECT * FROM questions, question_types WHERE questions.type = question_types.id AND questions.id = " + _currentQuestionID, connection);
            SqlDataReader reader = getCurrentQuestionAndItsQuestionType.ExecuteReader();

            string questionText = "";
            while (reader.Read())
            {
                questionText = reader["text"].ToString(); // "text" is a column from the TestQuestion table
            }

            connection.Close();

            return questionText;
        }

        //OK
        private string GetCurrentQuestionType(int _currentQuestionID)
        {
            SqlConnection connection = ConnectToSqlDb();
            SqlCommand getCurrentQuestionAndItsQuestionType = new SqlCommand("SELECT * FROM questions, question_types WHERE questions.type = question_types.id AND questions.id = " + _currentQuestionID, connection);
            SqlDataReader reader = getCurrentQuestionAndItsQuestionType.ExecuteReader();

            string questionType = "";
            while (reader.Read())
            {
                questionType = reader["type_name"].ToString();
            }
            connection.Close();

            return questionType;
        }

        //OK
        private void QuestionLoader(int _currentQuestionID)
        {
            questionPlaceHolder.Controls.Clear();

            string _questionText = GetCurrentQuestionText(_currentQuestionID);
            string _questionType = GetCurrentQuestionType(_currentQuestionID);

            #region TextBox
            if (_questionType.Equals("text"))
            {
                TextBoxQuestionControl textBoxControl = (TextBoxQuestionControl)LoadControl("~/TextBoxQuestionControl.ascx");
                textBoxControl.ID = "textBoxQuestionControl";
                textBoxControl.QuestionLabel.Text = _questionText;
                questionPlaceHolder.Controls.Add(textBoxControl);
            }
            #endregion

            #region CheckBox
            else if (_questionType.Equals("checkbox"))
            {
                CheckBoxQuestionControl checkBoxQuestion = (CheckBoxQuestionControl)LoadControl("~/CheckBoxQuestionControl.ascx");
                checkBoxQuestion.ID = "checkBoxQuestion";
                checkBoxQuestion.QuestionLabel.Text = _questionText;

                SqlConnection connection = ConnectToSqlDb();
                SqlCommand optionCommand = new SqlCommand(
                    "SELECT * FROM options WHERE question_id = " + _currentQuestionID, connection);
                SqlDataReader optionReader = optionCommand.ExecuteReader();

                while (optionReader.Read())
                {
                    ListItem item = new ListItem(optionReader["text"].ToString(), optionReader["id"].ToString());
                    checkBoxQuestion.QuestionCheckBoxList.Items.Add(item);
                }
                questionPlaceHolder.Controls.Add(checkBoxQuestion);

                connection.Close();
            }
            #endregion

            #region Radio
            else if (_questionType.Equals("radio"))
            {
                RadioQuestionControl radioQuestion = (RadioQuestionControl)LoadControl("~/RadioQuestionControl.ascx");
                radioQuestion.ID = "radioQuestion";
                radioQuestion.QuestionLabel.Text = _questionText;

                SqlConnection connection = ConnectToSqlDb();
                SqlCommand optionCommand = new SqlCommand(
                    "SELECT * FROM options WHERE question_id = " + _currentQuestionID, connection);
                SqlDataReader optionReader = optionCommand.ExecuteReader();

                while (optionReader.Read())
                {
                    ListItem item = new ListItem(optionReader["text"].ToString(), optionReader["id"].ToString());
                    radioQuestion.QuestionRadioList.Items.Add(item);
                }
                questionPlaceHolder.Controls.Add(radioQuestion);

                connection.Close();
            }
            #endregion
        }

        private void SubmitCurrentQuestionAnswersToSession()
        {

            List<Answer> _tmpSessionAnswers = GetListOfAnswersFromSession();
            #region Radio Question
            RadioQuestionControl radioQuestion = (RadioQuestionControl)questionPlaceHolder.FindControl("radioQuestion");
            if (radioQuestion != null)
            {
                foreach (ListItem option in radioQuestion.QuestionRadioList.Items)
                {
                    if (option.Selected)
                    {
                        Answer answer = new Answer();
                        answer.option_id = Int32.Parse(option.Value);
                        answer.question_id = this.currentQuestionId;
                        _tmpSessionAnswers.Add(answer);
                        selectedOptionsIdList.Add(answer.option_id);
                    }
                }
            }
            #endregion

            #region Checkbox Question
            CheckBoxQuestionControl checkBoxQuestion = (CheckBoxQuestionControl)questionPlaceHolder.FindControl("checkBoxQuestion");
            if (checkBoxQuestion != null)
            {
                foreach (ListItem option in checkBoxQuestion.QuestionCheckBoxList.Items)
                {
                    if (option.Selected)
                    {
                        Answer answer = new Answer();
                        answer.option_id = Int32.Parse(option.Value);
                        answer.question_id = this.currentQuestionId;
                        _tmpSessionAnswers.Add(answer);
                        selectedOptionsIdList.Add(answer.option_id);
                    }
                }
            }
            #endregion

            HttpContext.Current.Session["sessionAnswers"] = _tmpSessionAnswers;
            HttpContext.Current.Session["selectedOptionIdList"] = selectedOptionsIdList;

            #region TextBox Question
            TextBoxQuestionControl textBoxQuestion = (TextBoxQuestionControl)questionPlaceHolder.FindControl("textBoxQuestionControl");
            if (textBoxQuestion != null)
            {
                Answer answer = new Answer();
                answer.text = textBoxQuestion.QuestionTextBox.Text;
                answer.question_id = this.currentQuestionId;
                _tmpSessionAnswers.Add(answer);
                HttpContext.Current.Session["sessionAnswers"] = _tmpSessionAnswers;
            }
            #endregion

        }

        private void UpdateFollowUpQuestionIdList() {
            if (selectedOptionsIdList != null)
                foreach (int i in selectedOptionsIdList)
                    CheckAndAddFollowUpQuestionToSession(i);
        }

        protected void nextButton_Click(object sender, EventArgs e)
        {
            
            SubmitCurrentQuestionAnswersToSession();

            UpdateFollowUpQuestionIdList();

            this.currentQuestionId = GetCurrentQuestionID();
            HttpContext.Current.Session["currentQuestionId"] = this.currentQuestionId;

            QuestionLoader(this.currentQuestionId);

        }
    


    }
}