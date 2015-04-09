using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

namespace VideoQuiz.Models
{
    public class QuestionOption
    {
        [JsonProperty("optionid")]
        public int Id { get; set; }

        [JsonIgnore]
        public int QuestionId { get; set; }

        [JsonProperty("name")]
        public string Title { get; set; }

        public int? Order { get; set; }
    }
}