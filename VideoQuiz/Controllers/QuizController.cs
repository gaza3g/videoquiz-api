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
using System.Web.Http.Description;
using VideoQuiz.Models;

namespace VideoQuiz.Controllers
{
    public class QuizController : ApiController
    {

        class resp_ques {
            public int time {get; set;}
            public List<Item> items { get; set; }
        }

        class Item {
            public int id { get; set; }
            public string type { get; set; }
            public bool recordsResponse { get; set; }
            public string question { get; set; }
            public List<Option> options { get; set; }
            public string correctAnswer { get; set; }
        }

        class Option {
            public string name {get; set;}
        }

        [Route("api/quiz/{quizId}")]
        public IHttpActionResult Get(int quizId)
        {
            var db_questions = this.GetAllQuestionsFromQuiz(quizId);

            List<resp_ques> questions = new List<resp_ques>();

            int time = 3;

            foreach(Question q in db_questions)
            {
                // TODO: linq query to only grab questions whose options is > 0
                if (q.Options.Count() == 0) 
                    continue;

                List<Option> options = new List<Option>();

                foreach (QuestionOption qo in q.Options)
                {
                    options.Add(new Option { name = qo.Title });
                }


                List<Item> items = new List<Item>();
                
                items.Add(new Item
                {
                    id = q.QuestionId,
                    type = "single",
                    recordsResponse = true,
                    question = q.Title,
                    options = options,
                    correctAnswer = String.Empty
                });


                questions.Add(new resp_ques { time = time, items = items });

                time += 3;

            }

            //var videoQuizRequest = new VideoQuizRequest
            //{
            //    time = 10
            //    Id = quizId,
            //    Questions = questions,
            //    Title = "Hardcoded Title",
            //    VideoUrl = "https://videoquizstorage.blob.core.windows.net/asset-9e59093c-8fb3-4f17-b798-570d5998514c/video.mp4?sv=2012-02-12&sr=c&si=2fb819f8-081b-487b-970d-c3d24c91b61b&sig=QMjiVj9H3MlyUACk0qi2HZuWUfiI4ADp7z5rXeQZ2Vw%3D&st=2015-02-11T03%3A39%3A44Z&se=2017-02-10T03%3A39%3A44Z"
            //};




            if (questions == null)
            {
                return this.NotFound();
            }

            return this.Ok(questions);

        }

        private List<Question> GetAllQuestionsFromQuiz(int quizId)
        {
            List<Question> questionList = new List<Question>();

            using (DBEntityContainer db = new DBEntityContainer())
            {
                var result = db.QZ_GetAllSectionsQuestionsForVideoQuiz(quizId).ToList();

                foreach (var item in result.ToList())
                {
                    var questionOptions = GetOptionsFromQuestion(item.ID);

                    questionList.Add(new Question { QuestionId = item.ID, 
                                                    TypeId = item.TypeID, 
                                                    Title = item.Question,
                                                    Options = questionOptions });
                }

                return questionList;
            }
        }


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
