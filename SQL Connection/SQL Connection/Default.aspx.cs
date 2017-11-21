using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;


namespace SQL_Connection
{
    public partial class _Default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //get our test connection string from the web config
            string connectionString = ConfigurationManager.ConnectionStrings["testConnection"].ConnectionString;
            SqlConnection connection = new SqlConnection();

            connection.ConnectionString = connectionString;
            connection.Open(); //open connection using connectionString

            //setup basic sql command
            SqlCommand command = new SqlCommand("SELECT * FROM TestQuestion", connection);

            //execute command
            SqlDataReader reader = command.ExecuteReader();

            DataTable dt = new DataTable(); //can hold any datatypes

            //setup the columns
            dt.Columns.Add("questionId", typeof(Int32));
            dt.Columns.Add("text", typeof(String));
            dt.Columns.Add("questionType", typeof(Int32));
            dt.Columns.Add("nextQuestion", typeof(Int32));

            //reads 1 row at a time from our sql set of results
            while (reader.Read())
            {
                //generate an empty row for our table
                DataRow row = dt.NewRow();
                //fill in row from this row of results
                row["questionId"] = reader["questionId"];
                row["text"] = reader["text"];
                row["questionType"] = reader["questionType"];
                row["nextQuestion"] = reader["nextQuestion"];
                //add this row to our data table
                dt.Rows.Add(row);
            }
            //show results in gridview
            QuestionGridView.DataSource = dt;
            QuestionGridView.DataBind();

            connection.Close();
        }
    }
}
