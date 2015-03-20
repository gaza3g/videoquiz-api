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
        public IHttpActionResult Get(int quizId)
        {
            var db_questions = this.GetAllQuestionsFromQuiz(quizId);

            List<QuestionContainer> questions = new List<QuestionContainer>();

            int time = 3;

            foreach(Question q in db_questions)
            {
                // TODO: linq query to only grab questions whose options is > 0
                // instead of checking each and every question.
                if (q.Options.Count() == 0) 
                    continue;

                questions.Add(new QuestionContainer { time = time, question = new List<Question>{q} });

                time += 3;

            }


            if (questions == null)
            {
                return this.NotFound();
            }

            return this.Ok(questions);

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
