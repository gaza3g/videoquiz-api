using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

namespace VideoQuiz.Models
{
    /// <summary>
    /// List<Question> can be misleading since there will always be one item in that list 
    /// but we want the json to output in an array format e.g:
    ///		"item": [{
    ///		    "id": 3244,
    ///		    "question": "What is color of sky",
    ///		    "recordsResponse": true,
    ///		    "correctAnswer": "",
    ///		    "type": "single",
    ///		    "options": [{ ... }]
    ///		}]
    /// 
    /// If we don't add the question within the list, it will be returned in this format:
    /// "item": {
    /// }
    /// 
    /// Notice the square brackets.
    ///

    /// </summary>
    class QuestionContainer {
        public int time {get; set;}

        [JsonProperty("items")]
        public List<Question> question { get; set; }
    }
}