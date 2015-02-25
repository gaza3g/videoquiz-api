using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

namespace VideoQuiz.Models
{
    public class Question
    {
        [JsonProperty("id")]
        public int QuestionId { get; set; }

        [JsonIgnore]
        public int TypeId {get; set;}

        [JsonProperty("question")]
        public string Title { get; set; }

        [JsonProperty("recordsResponse")]
        public bool RecordsResponse { get; set; }

        [JsonProperty("correctAnswer")]
        public string CorrectAnswer { get; set; }

        public string Type { get; set; }

        public List<QuestionOption> Options { get; set; }

    }
}