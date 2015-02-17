using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GeekQuiz.Models
{
    public class Question
    {
        public int QuestionId { get; set; }
        public int TypeId {get; set;}
        public string Title { get; set; }
    }
}