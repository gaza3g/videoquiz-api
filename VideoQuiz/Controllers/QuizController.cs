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

        [Route("api/quiz/{quizId}")]
        public IHttpActionResult Get(int quizId)
        {
            var questions = this.GetAllQuestionsFromQuiz(quizId);

            var videoQuizRequest = new VideoQuizRequest
            {
                Id = quizId,
                Questions = questions,
                Title = "Hardcoded Title",
                VideoUrl = "https://videoquizstorage.blob.core.windows.net/asset-9e59093c-8fb3-4f17-b798-570d5998514c/video.mp4?sv=2012-02-12&sr=c&si=2fb819f8-081b-487b-970d-c3d24c91b61b&sig=QMjiVj9H3MlyUACk0qi2HZuWUfiI4ADp7z5rXeQZ2Vw%3D&st=2015-02-11T03%3A39%3A44Z&se=2017-02-10T03%3A39%3A44Z"
            };


            if (videoQuizRequest == null)
            {
                return this.NotFound();
            }

            return this.Ok(videoQuizRequest);

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
