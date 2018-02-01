using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SQL_Connection
{
    //this class provides the structure for what an answer will contain to then store in the session/db
    public class Answer
    {
        //the 'text-type' answer (textBox)
        public string text;
        //the question that the answer belongs to
        public int question_id;
        //the 'selected-type' answer (radio,checkBox)
        public int option_id;
    }
}