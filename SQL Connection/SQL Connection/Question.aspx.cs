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
        private static SqlConnection ConnectToSqlDb()
        {
            //get connection string from webconfig
            string connectionString = ConfigurationManager.ConnectionStrings["testConnection"].ConnectionString;

            //build sql connection, use connection string, open(connect)
            SqlConnection connection = new SqlConnection();

            //set the connection string 
            connection.ConnectionString = connectionString;
            connection.Open();
            return connection;
        }

        //returns the ID of the question that should be next
        private static int GetCurrentQuestionID()
        {
            int currentQuestionID;

            //create a followup question list and populate it with the current session's followup question list
            List<int> followUpQuestionsIDList = ((List<int>)HttpContext.Current.Session["followUpQuestionsIDList"]);

            //if there is a followup question in the list
            if (followUpQuestionsIDList.Count > 0)
            {
                //make the current question be the first row found in the followup question list
                currentQuestionID = followUpQuestionsIDList.First();
                //after that remove that entry from the list 
                followUpQuestionsIDList.Remove(0);
                //update the followup question list
                HttpContext.Current.Session["followUpQuestionsIDList"] = followUpQuestionsIDList;
            }

            //if there is no followup question, continue to the next question
            else if (HttpContext.Current.Session["questionID"] != null)
            {
                //we have a question number stored in the session, we should use that value for current question
                currentQuestionID = (int)HttpContext.Current.Session["questionID"];//session stores objects, cast to int
            }

            //and if we dont have followup nor a nextquestion, get the first value from db
            else
            {
                //create a temporary connection to the database in order to fetch the first question
                SqlConnection connection = ConnectToSqlDb();
                //create an sql command to fetch from the database the rows with the smallest ID value present in the db
                SqlCommand getFirstQuestionID = new SqlCommand("SELECT min(id) FROM questions", connection);
                //create a temporary object to store the data which will be fetched from running 'getFirstQuestionID' SQL command
                SqlDataReader reader = getFirstQuestionID.ExecuteReader();
                reader.Read();
                //set the currentQuestion to be the value that is in the 'reader' object's first column (0), which is the 'id' column from the database
                currentQuestionID = Int32.Parse(reader.GetValue(0).ToString());
                //close the connection
                connection.Close();
                //now store the previously set currentQuestion to the session, 
                //so that when we run this the second time it will run the if part of this statement
                HttpContext.Current.Session["questionID"] = currentQuestionID;
            }
            return currentQuestionID;
        }

        //returns the current list of answers for this session
        private static List<Answer> GetListOfAnswersFromSession()
        {
            //create the list that we will want to return at the end of the session
            List<Answer> answers = new List<Answer>();
            //check if there is a list of answers of the current session, and if there is
            if (HttpContext.Current.Session["sessionAnswers"] != null)
                //populate the list we created with the current session's list of answers
                answers = (List<Answer>)HttpContext.Current.Session["sessionAnswers"];
            //return the list of answers
            return answers;
        }

        //returns the follow up question id of an option_id that is given as a parameter
        private static Int32 GetFollowUpQuestionID(int _option_id)
        {
            //create connection
            SqlConnection connection = new SqlConnection();
            //create a command for selecting the follow_up_id value for the _option_id we are checking now
            SqlCommand getFollowUpID = new SqlCommand("SELECT follow_up_id FROM options WHERE id = " + _option_id, connection);
            //create a reader object to be able to populate with the data from the sqlcommand above
            SqlDataReader reader = getFollowUpID.ExecuteReader();

            //simply create it and assign a value 
            int followUpQuestionID = Int32.Parse(reader.GetValue(2).ToString());

            connection.Close();
            return followUpQuestionID;
        }

        //returns the current question text 
        private static string GetCurrentQuestionText(int _currentQuestionID)
        {
            //establish a connection in order to run the SqlCommand
            SqlConnection connection = ConnectToSqlDb();

            /*build sql command to get current question
             *joining two tables because the 'questions' table has a foreign key column for type,
             *but I want to get the question type name as a string which is on the 'question_types' table
             *so that I check and match it via a string in the code */
            SqlCommand getCurrentQuestionAndItsQuestionType = new SqlCommand("SELECT * FROM questions, question_types WHERE questions.type = question_types.id AND questions.id = " + _currentQuestionID, connection);

            //run the command previously built, store the results in the reader object
            SqlDataReader reader = getCurrentQuestionAndItsQuestionType.ExecuteReader();

            string questionText = "";

            //loop through the rows stored on the reader object
            while (reader.Read())
            {
                //get question text and type from this row of results
                questionText = reader["text"].ToString(); // "text" is a column from the TestQuestion table
            }
            connection.Close();

            return questionText;
        }

        //returns the current question type
        private static string GetCurrentQuestionType(int _currentQuestionID)
        {
            //establish a connection in order to run the SqlCommand
            SqlConnection connection = ConnectToSqlDb();

            /*build sql command to get current question
             *joining two tables because the 'questions' table has a foreign key column for type,
             *but I want to get the question type name as a string which is on the 'question_types' table
             *so that I check and match it via a string in the code */
            SqlCommand getCurrentQuestionAndItsQuestionType = new SqlCommand("SELECT * FROM questions, question_types WHERE questions.type = question_types.id AND questions.id = " + _currentQuestionID, connection);

            //run the command previously built, store the results in the reader object
            SqlDataReader reader = getCurrentQuestionAndItsQuestionType.ExecuteReader();

            string questionType = "";
            //loop through the rows stored on the reader object
            while (reader.Read())
            {
                //get question type from this row of results
                questionType = reader["type_name"].ToString(); // "typeName" is a column from the TestQuestionType table
            }
            connection.Close();

            return questionType;
        }

        //loads on the correct controller based on the current question 
        private void QuestionLoader(string _questionText, string _questionType, int _currentQuestionID)
        {
            #region TextBox
            if (_questionType.Equals("text"))
            {
                //load the control up
                TextBoxQuestionControl textBoxControl = (TextBoxQuestionControl)LoadControl("~/TextBoxQuestionControl.ascx");
                //set its ID
                textBoxControl.ID = "textBoxQuestionControl";
                //set its question text label
                textBoxControl.QuestionLabel.Text = _questionText;

                //add it to the placeholder on our webpage
                questionPlaceHolder.Controls.Add(textBoxControl);
            }
            #endregion

            #region CheckBox
            else if (_questionType.Equals("checkbox"))
            {
                //load the control up
                CheckBoxQuestionControl checkBoxQuestion = (CheckBoxQuestionControl)LoadControl("~/CheckBoxQuestionControl.ascx");
                //set its ID
                checkBoxQuestion.ID = "checkBoxQuestion";
                //set text label to question text
                checkBoxQuestion.QuestionLabel.Text = _questionText;

                //establish a connection in order to run the SqlCommand
                SqlConnection connection = ConnectToSqlDb();

                //we need to talk to the database to get all of the options for this question to display
                //e.g: what gender? then options like male, female etc...
                SqlCommand optionCommand = new SqlCommand(
                    "SELECT * FROM options WHERE question_id = " + _currentQuestionID, connection);

                //run command, get ready to read results
                SqlDataReader optionReader = optionCommand.ExecuteReader();

                //cycle through rows of options
                while (optionReader.Read())
                {
                    //for every row, we will build a list item representing it
                    //                           display member(for ui),          value member(for devs to store if selected)
                    ListItem item = new ListItem(optionReader["text"].ToString(), optionReader["id"].ToString());
                    //add item to our checkbox list
                    checkBoxQuestion.QuestionCheckBoxList.Items.Add(item);
                }
                //finally have all the checkbox list items populated, add our checkbox question control to the placeholder
                questionPlaceHolder.Controls.Add(checkBoxQuestion);

                connection.Close();
            }
            #endregion

            #region Radio
            else if (_questionType.Equals("radio"))
            {
                //load the control up
                RadioQuestionControl radioQuestion = (RadioQuestionControl)LoadControl("~/RadioQuestionControl.ascx");
                //set its ID
                radioQuestion.ID = "radioQuestion";
                //set text label to question text
                radioQuestion.QuestionLabel.Text = _questionText;

                //establish a connection in order to run the SqlCommand
                SqlConnection connection = ConnectToSqlDb();

                //we need to talk to the database to get all of the options for this question to display
                //e.g: what gender? then options like male, female etc...
                SqlCommand optionCommand = new SqlCommand(
                    "SELECT * FROM options WHERE question_id = " + _currentQuestionID, connection);

                //run command, get ready to read results
                SqlDataReader optionReader = optionCommand.ExecuteReader();

                //cycle through rows of options
                while (optionReader.Read())
                {
                    //for every row, we will build a list item representing it
                    //                           display member(for ui),          value member(for devs to store if selected)
                    ListItem item = new ListItem(optionReader["text"].ToString(), optionReader["id"].ToString());
                    //add item to our checkbox list
                    radioQuestion.QuestionRadioList.Items.Add(item);
                }
                //finally have all the checkbox list items populated, add our checkbox question control to the placeholder
                questionPlaceHolder.Controls.Add(radioQuestion);

                connection.Close();
            }
            #endregion
        }

        //initial page load
        protected void Page_Load(object sender, EventArgs e)
        {
            //if there isnt a followupquestions list then create an empty one
            if (HttpContext.Current.Session["followUpQuestionsIDList"] == null) {
                HttpContext.Current.Session["followUpQuestionsIDList"] = new List<int>();
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

            //set the currentQuestionID to start the survey
            int currentQuestionID = GetCurrentQuestionID();
            string currentQuestionText = GetCurrentQuestionText(currentQuestionID);
            string currentQuestionType = GetCurrentQuestionType(currentQuestionID);

            //using type, load up correct web user control
            QuestionLoader(currentQuestionText, currentQuestionType, currentQuestionID);
        }
        
        protected void nextButton_Click(object sender, EventArgs e)
        {
            //load list of answers that are being stored in this current session and
            //store it on a temporary session answer's list
            //while we wait to update it with the current answer
            //and then store the temporary session answer's list in the actual session's answer 
            List<Answer> tmpSessionAnswers = GetListOfAnswersFromSession();

            //checks for what question type we need to handle
            //finds what answer to store in the current session's answers list
            #region QuestionType Check, Store Answer in Session

                #region Radio Question

                //lets try to find a checkbox question control in the webpage from the placeholder
                RadioQuestionControl radioQuestion = (RadioQuestionControl)questionPlaceHolder.FindControl("radioQuestion");

                //if in fact there is a checkboxquestion
                if (radioQuestion != null)
                {
                    //for each selected item, add to bullet list
                    foreach (ListItem option in radioQuestion.QuestionRadioList.Items)
                    {
                        //is this option selected? if so do the following:
                        if (option.Selected)
                        {
                            //create a new answer object to store current answer
                            Answer answer = new Answer();
                            //store in answer the text that the user input in the textbox
                            answer.option_id = Int32.Parse(option.Value);
                            //store in answer the ID of the question
                            answer.question_id = GetCurrentQuestionID();
                            //store this answer in the temporary list of answers 
                            tmpSessionAnswers.Add(answer);
                            //update the current session 
                            HttpContext.Current.Session["sessionAnswers"] = tmpSessionAnswers;
                        }
                    }
                }
                #endregion

                #region Checkbox Question

                //lets try to find a checkbox question control in the webpage from the placeholder
                CheckBoxQuestionControl checkBoxQuestion = (CheckBoxQuestionControl)questionPlaceHolder.FindControl("checkBoxQuestion");
                //if in fact there is a checkboxquestion
                if (checkBoxQuestion != null)
                {
                    //for each selected item, add to bullet list
                    foreach (ListItem option in checkBoxQuestion.QuestionCheckBoxList.Items)
                    {
                        //is this option selected?
                        if (option.Selected)
                        {
                            //create a new answer object to store current answer
                            Answer answer = new Answer();
                            //store in answer the text that the user input in the textbox
                            answer.option_id = Int32.Parse(option.Value);
                            //store in answer the ID of the question
                            answer.question_id = GetCurrentQuestionID();
                            //store this answer in the temporary list of answers 
                            tmpSessionAnswers.Add(answer);
                            //update the current session 
                            HttpContext.Current.Session["sessionAnswers"] = tmpSessionAnswers;

                            //check if the option selected has followup
                            //open a connection to request data from the database
                        }
                    }
                }
                #endregion

                #region TextBox Question

                //create a textBoxQuestion object and go find 
                TextBoxQuestionControl textBoxQuestion = (TextBoxQuestionControl)questionPlaceHolder.FindControl("textBoxQuestionControl");
                if (textBoxQuestion != null)
                {
                    //create a new answer object to store current answer
                    Answer answer = new Answer();
                    //store in answer the text that the user input in the textbox
                    answer.text = textBoxQuestion.QuestionTextBox.Text;
                    //store in answer the ID of the question
                    answer.question_id = GetCurrentQuestionID();
                    //store this answer in the temporary list of answers 
                    tmpSessionAnswers.Add(answer);
                    //update the current session 
                    HttpContext.Current.Session["sessionAnswers"] = tmpSessionAnswers;

                    List<int> followUpQuestionIDList = (List<int>)HttpContext.Current.Session["followUpQuestionsIDList"];
                    //...
                    //ADD ids to the list
                    //...
                    HttpContext.Current.Session["followUpQuestionsIDList"] = followUpQuestionIDList;

                }
                #endregion

            #endregion

            int currentQuestionID = GetCurrentQuestionID();
            SqlConnection connection = ConnectToSqlDb();

            #region Follow Up Question
            //setup or load up a list of follow up questions we need to complete
            List<Int32> followUpQuestions = new List<int>();
            //if list of follow up questions exists then use it instead of the empty list
            if (HttpContext.Current.Session["followUpQuestions"] != null)
            {
                followUpQuestions = (List<Int32>)HttpContext.Current.Session["followUpQuestions"];
            }
            //FOR TESTING ONLY, faking adding follow up questions. AKA dont do it this in final
            if (currentQuestionID == 1) //hardcoded checks loses marks in assignment
            {
                //add your follow up question ids to the list
                followUpQuestions.Add(3);
                followUpQuestions.Add(4);
            }

            //Find out what the next question should be:
            //get current question from DB, there is a column on this table with a foreign key to the next question
            SqlCommand command = new SqlCommand("SELECT * FROM questions where id = " + currentQuestionID, connection);
            SqlDataReader reader = command.ExecuteReader();

            //loop through results. Should only be 1
            while (reader.Read())
            {
                //if at the end of the survey, next question foreign key column will be NULL, so check for nulls first
                //first, get index of nextQuestion column
                int nextQuestionColumnIndex = reader.GetOrdinal("next_question_id");
                //use this index to check if column is null
                if (reader.IsDBNull(nextQuestionColumnIndex))
                {
                    //end of Survey
                    //TODO redirect to registration page or something finalising the survey
                }
                else
                {
                    //not end of survey
                    int nextQuestionID = (int)reader["next_question_id"];
                    //set this as the current question in the session
                    HttpContext.Current.Session["questionID"] = nextQuestionID;

                    //IF THERE IS FOLLOW UP QUESTIONS THOUGH, DO THEM FIRST
                    if (followUpQuestions.Count > 0)
                    {
                        //set current question to first follow up question, then remove from follow up question list 
                        //so it doesn't repeat for next time
                        HttpContext.Current.Session["questionID"] = followUpQuestions[0];
                        followUpQuestions.RemoveAt(0);
                    }
                    //store the follow up questions list in session (just in case it changed)
                    HttpContext.Current.Session["followUpQuestions"] = followUpQuestions;

                    //redirect to same page to load up the next question as current question(aka, run pageLoad again)
                    Response.Redirect("Question.aspx");
                }
            }
            #endregion
            
            //close the connection
            connection.Close();
        }



    }
}