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
using GeekQuiz.Models;

namespace VideoQuiz.Controllers
{
    [Authorize]
    public class TriviaController : ApiController
    {
        private TriviaContext db = new TriviaContext();

        protected override void Dispose(bool disposing)
        {
            if(disposing)
            {
                this.db.Dispose();
            }

            base.Dispose(disposing);
        }


        [Route ("api/question/{questionId}")]
        [ResponseType(typeof(List<QuestionOption>))]
        public IHttpActionResult GetQuestionOption(int questionId)
        {
            var userid = User.Identity.Name;

            var questionOptions = this.GetOptionsFromQuestion(questionId);

            if (questionOptions == null)
            {
                return this.NotFound();
            }

            return this.Ok(questionOptions);
        }


        [Route ("api/quiz/{quizId}")]
        [ResponseType(typeof(List<Question>))]
        public IHttpActionResult Get(int quizId)
        {
            var userid = User.Identity.Name;

            var questions = this.GetAllQuestionsFromQuiz(quizId);

            if (questions == null)
            {
                return this.NotFound();
            }

            return this.Ok(questions);
        }

        // GET api/Trivia
        [ResponseType(typeof(TriviaQuestion))]
        public async Task<IHttpActionResult> Get()
        {
            var userid = User.Identity.Name;

            TriviaQuestion nextQuestion = await this.NextQuestionAsync(userid);

            if (nextQuestion == null)
            {
                return this.NotFound();
            }

            return this.Ok(nextQuestion);
        }

        // POST api/Trivia
        [ResponseType(typeof(TriviaAnswer))]
        public async Task<IHttpActionResult> Post(TriviaAnswer answer)
        {
            if(!ModelState.IsValid)
            {
                return this.BadRequest(this.ModelState);
            }

            answer.UserId = User.Identity.Name;

            var isCorrect = await this.StoreAsync(answer);
            return this.Ok<bool>(isCorrect);
        }

        private List<Question> GetAllQuestionsFromQuiz(int quizId)
        {
            List<Question> questionList = new List<Question>();

            using (VideoQuiz.Models.DBEntityContainer test = new VideoQuiz.Models.DBEntityContainer())
            {
                var result = test.QZ_GetAllSectionsQuestionsForVideoQuiz(quizId).ToList();

                foreach (var item in result.ToList())
                {
                    questionList.Add(new Question { QuestionId = item.ID, 
                                                    TypeId = item.TypeID, 
                                                    Title = item.Question });
                }

                return questionList;
            }
        }


        private List<QuestionOption> GetOptionsFromQuestion(int questionId)
        {
            List<QuestionOption> optionList = new List<QuestionOption>();

            using (VideoQuiz.Models.DBEntityContainer test = new VideoQuiz.Models.DBEntityContainer())
            {
                var result = test.QZ_GetAnswer_MCH(questionId).ToList();

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



        private async Task<TriviaQuestion> GetQuestionAsync(int questionId)
        {
            return await this.db.TriviaQuestions.FindAsync(CancellationToken.None, questionId);
        }

        private async Task<TriviaQuestion> NextQuestionAsync(string userId)
        {
            var lastQuestionId = await this.db.TriviaAnswers
                .Where(a => a.UserId == userId)
                .GroupBy(a => a.QuestionId)
                .Select(g => new { QuestionId = g.Key, Count = g.Count() })
                .OrderByDescending(q => new { q.Count, QuestionId = q.QuestionId })
                .Select(q => q.QuestionId)
                .FirstOrDefaultAsync();

            var questionsCount = await this.db.TriviaQuestions.CountAsync();

            var nextQuestionId = (lastQuestionId % questionsCount) + 1;

            return await this.db.TriviaQuestions.FindAsync(CancellationToken.None, nextQuestionId);
        }

        private async Task<bool> StoreAsync(TriviaAnswer answer)
        {
            this.db.TriviaAnswers.Add(answer);

            await this.db.SaveChangesAsync();

            var selectedOption = await this.db.TriviaOptions.FirstOrDefaultAsync(o => o.Id == answer.OptionId
                && o.QuestionId == answer.QuestionId);

            return selectedOption.IsCorrect;
        }
    }
}
