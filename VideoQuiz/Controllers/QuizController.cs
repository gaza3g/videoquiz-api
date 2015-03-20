using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Web.Http.Description;
using VideoQuiz.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text;

namespace VideoQuiz.Controllers
{
    /// <summary>
    /// Returns the questions that are associated with a particular quiz.
    /// </summary>
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class QuizController : ApiController
    {
        public class QuestionResponse
        {
            public string QuizId { get; set; }
            public string QuestionId { get; set; }
            public string OptionId { get; set; }
            public string OptionText { get; set; }
        }

        [Route("quiz/response")]
        [HttpPost, HttpOptions]
        public IHttpActionResult Post([FromBody] QuestionResponse response)
        {
            if (this.Request.Method == HttpMethod.Post)
            {
                // Do something with it, for e.g store in DB
                return this.Ok(response);
            }

            return this.Ok();
        }

        [Route("quiz/{quizId}")]
        [HttpGet, HttpOptions]
        public HttpResponseMessage Get(int quizId)
        {
            string fake_data = @"
                    [{
            \""time\"": 3,
            \""items\"": [{
                \""id\"": 3244,
                \""question\"": \""What is color of sky\"",
                \""recordsResponse\"": true,
                \""correctAnswer\"": \""\"",
                \""type\"": \""single\"",
                \""options\"": [{
                    \""optionid\"": 11631,
                    \""name\"": \""red\""
                }, {
                    \""optionid\"": 11632,
                    \""name\"": \""green\""
                }, {
                    \""optionid\"": 11633,
                    \""name\"": \""blue\""
                }, {
                    \""optionid\"": 11634,
                    \""name\"": \""orange\""
                }]
            }]
        }, {
            \""time\"": 6,
            \""items\"": [{
                \""id\"": 3245,
                \""question\"": \""Which 2 are mamals\"",
                \""recordsResponse\"": true,
                \""correctAnswer\"": \""\"",
                \""type\"": \""single\"",
                \""options\"": [{
                    \""optionid\"": 11635,
                    \""name\"": \""Cat\""
                }, {
                    \""optionid\"": 11636,
                    \""name\"": \""Tree\""
                }, {
                    \""optionid\"": 11637,
                    \""name\"": \""Dog\""
                }, {
                    \""optionid\"": 11638,
                    \""name\"": \""Grass\""
                }]
            }]

        }]
            ";

            var response = Request.CreateResponse(HttpStatusCode.OK);
            response.Content = new StringContent(fake_data.Replace("\\",""),Encoding.UTF8, "application/json");
            return response;

        }

        /// <summary>
        /// Call GetAllSectionsQuestionsForVideoQuiz stored proc and populate a list of Question
        /// objects with data from the rows returned.
        /// </summary>
        /// <param name="quizId"></param>
        /// <returns></returns>
        private List<Question> GetAllQuestionsFromQuiz(int quizId)
        {
            List<Question> questionList = new List<Question>();

            using (DBEntityContainer db = new DBEntityContainer())
            {
                var result = db.QZ_GetAllSectionsQuestionsForVideoQuiz(quizId).ToList();

                foreach (var item in result.ToList())
                {
                    var questionOptions = GetOptionsFromQuestion(item.ID);

                    questionList.Add(
                        new Question {
                            QuestionId = item.ID,
                            TypeId = item.TypeID,
                            Title = item.Question,
                            Options = questionOptions,
                            RecordsResponse = true,
                            CorrectAnswer = String.Empty,
                            Type = "single"
                        }
                    );
                }

                return questionList;
            }
        }


        /// <summary>
        /// Call the GetAnswer_MCH stored proc and populate the QuestionOption objects
        /// with the rows returned.
        /// </summary>
        /// <param name="questionId"></param>
        /// <returns></returns>
        private List<QuestionOption> GetOptionsFromQuestion(int questionId)
        {
            List<QuestionOption> optionList = new List<QuestionOption>();

            using (DBEntityContainer db = new DBEntityContainer())
            {
                var result = db.QZ_GetAnswer_MCH(questionId).ToList();

                foreach (var item in result.ToList())
                {
                    optionList.Add(new QuestionOption { Id = item.ID, 
                                                        QuestionId = item.QuestionID, 
                                                        Title = item.AnswerText,
                                                        Order = item.OptionOrder});
                }

                return optionList;
            }
        }



    }
}
