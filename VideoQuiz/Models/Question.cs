using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VideoQuiz.Models
{
    public class Question
    {
        public int QuestionId { get; set; }
        public int TypeId {get; set;}
        public string Title { get; set; }
        public IEnumerable<QuestionOption> Options { get; set; }
    }
}