using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VideoQuiz.Models
{
    public class VideoQuizRequest
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string VideoUrl { get; set; }
        public IEnumerable<Question> Questions { get; set; }
    }
}