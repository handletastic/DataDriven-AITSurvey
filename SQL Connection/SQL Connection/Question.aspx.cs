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
            CreateSessionID();

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

            if (currentQuestionId > 0)
                QuestionLoader(this.currentQuestionId);
            else
                ChangeNextButtonToSubmit();
        }


        private SqlConnection ConnectToSqlDb()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["testConnection"].ConnectionString;
            SqlConnection connection = new SqlConnection();
            connection.ConnectionString = connectionString;
            connection.Open();
            return connection;
        }


        //user registration functions
        private string GetIPAddress()
        {
            //get IP through PROXY
            //====================
            System.Web.HttpContext context = System.Web.HttpContext.Current;
            string ipAddress = context.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];

            //should break ipAddress down, but here is what it looks like:
            // return ipAddress;
            if (!string.IsNullOrEmpty(ipAddress))
            {
                string[] address = ipAddress.Split(',');
                if (address.Length != 0)
                {
                    return address[0];
                }
            }
            //if not proxy, get nice ip, give that back :(
            //ACROSS WEB HTTP REQUEST
            //=======================
            ipAddress = context.Request.UserHostAddress;//ServerVariables["REMOTE_ADDR"];

            if (ipAddress.Trim() == "::1")//ITS LOCAL(either lan or on same machine), CHECK LAN IP INSTEAD
            {
                //This is for Local(LAN) Connected ID Address
                string stringHostName = System.Net.Dns.GetHostName();
                //Get Ip Host Entry
                System.Net.IPHostEntry ipHostEntries = System.Net.Dns.GetHostEntry(stringHostName);
                //Get Ip Address From The Ip Host Entry Address List
                System.Net.IPAddress[] arrIpAddress = ipHostEntries.AddressList;

                try
                {
                    ipAddress = arrIpAddress[1].ToString();
                }
                catch
                {
                    try
                    {
                        ipAddress = arrIpAddress[0].ToString();
                    }
                    catch
                    {
                        try
                        {
                            arrIpAddress = System.Net.Dns.GetHostAddresses(stringHostName);
                            ipAddress = arrIpAddress[0].ToString();
                        }
                        catch
                        {
                            ipAddress = "127.0.0.1";
                        }
                    }
                }
            }
            return ipAddress;
        }
        private void CreateSessionID()
        {
            if (HttpContext.Current.Session["sessionId"] == null)
            {
                string respondentIpAddress = GetIPAddress();

                SqlConnection connection = ConnectToSqlDb();

                SqlCommand command = new SqlCommand("INSERT INTO sessions (date, ip, respondent_id) VALUES ('" + System.DateTime.Now + "', '" + respondentIpAddress + "', '" + 1 + "'); SELECT CAST(scope_identity() AS int)", connection);
                //SqlCommand command = new SqlCommand("INSERT INTO sessions (date, ip, respondent_id) VALUES ('" + System.DateTime.Now + "', '" + respondentIpAddress + "', '" + respondentid + "'); SELECT CAST(scope_identity() AS int)", connection);

                int newSessionId = (int)command.ExecuteScalar();
                HttpContext.Current.Session["sessionId"] = newSessionId;
            }
        }

        //interface functions
        private void ChangeNextButtonToSubmit()
        {
            nextButton.Text = "Submit";
            Label lb = new Label();
            lb.Text = "Thank you for completing the survey. </br > Click the 'Submit' button to submit your answers";
            questionPlaceHolder.Controls.Add(lb);
        }
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

        //get current question functions
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
        private string GetCurrentQuestionText(int _currentQuestionID)
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

        //answer functions
        private List<Answer> GetListOfAnswersFromSession()
        {
            List<Answer> answers = new List<Answer>();
            if (HttpContext.Current.Session["sessionAnswers"] != null)
                answers = (List<Answer>)HttpContext.Current.Session["sessionAnswers"];
            return answers;
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

            HttpContext.Current.Session["sessionAnswers"] = _tmpSessionAnswers;
            HttpContext.Current.Session["selectedOptionIdList"] = selectedOptionsIdList;
        }
        private void SubmitSessionAnswersToDatabase()
        {
            SqlConnection connection = ConnectToSqlDb();
            foreach (Answer a in (List<Answer>)HttpContext.Current.Session["sessionAnswers"])
            {
                SqlCommand insertCommand = new SqlCommand("INSERT INTO answers (text, question_id, session_id, option_id) VALUES ('" + a.text + "','" + a.question_id + "','" + (int)HttpContext.Current.Session["sessionID"] + "','" + a.option_id + "')", connection);
            }
            connection.Close();
        }

        //followup question functions
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
        private void UpdateFollowUpQuestionIdList() {
            if (selectedOptionsIdList != null)
                foreach (int i in selectedOptionsIdList)
                    CheckAndAddFollowUpQuestionToSession(i);
        }
        private void UpdateFollowUpListOnSession() {
            HttpContext.Current.Session["followUpQuestionIdList"] = this.followUpQuestionIdList;
        }
        private void CheckAndAddFollowUpQuestionToSession(int _option_id)
        {   
            int followUpFromOption = GetFollowUpQuestionID(_option_id);
            if (followUpFromOption > 0)
            {
                this.followUpQuestionIdList.Add(followUpFromOption);
                UpdateFollowUpListOnSession();
            }
        }


        protected void nextButton_Click(object sender, EventArgs e)
        {
            SubmitCurrentQuestionAnswersToSession();            

            UpdateFollowUpQuestionIdList();

            this.currentQuestionId = GetCurrentQuestionID();
            HttpContext.Current.Session["currentQuestionId"] = this.currentQuestionId;

            if (this.currentQuestionId == 0)
            {
                SubmitSessionAnswersToDatabase();
            }

            Response.Redirect("Question.aspx");
        }
    }
}