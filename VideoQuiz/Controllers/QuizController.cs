using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Web.Http.Description;
using VideoQuiz.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Configuration;

namespace VideoQuiz.Controllers
{
    /// <summary>
    /// Returns the questions that are associated with a particular quiz.
    /// </summary>
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class QuizController : ApiController
    {

        [Route("{instance}/quiz/{quizId}/title")]
        [HttpGet, HttpOptions]
        public HttpResponseMessage GetQuizTitle(string instance, int quizId)
        {
            DBEntityContainer db = GetDB(instance);

            string title = db.QZQuiz
                          .Where(q => q.ID == quizId)
                          .Select(q => q.Title)
                          .SingleOrDefault();

            db.Dispose();

            if (title == null)
                throw new HttpResponseException(HttpStatusCode.NotFound);


            //return this.Ok(title.Replace("\"", String.Empty));
            var response = Request.CreateResponse(HttpStatusCode.OK);
            response.Content = new StringContent(title);
            return response;

        }

        /// <summary>
        /// Self explanatory. Form video URL by joining columns from the LOResource table.
        /// </summary>
        /// <param name="quizId"></param>
        /// <returns></returns>
        [Route("{instance}/quiz/{quizId}/video")]
        [HttpGet, HttpOptions]
        public HttpResponseMessage GetVideoUrl(string instance, int quizId)
        {
            int loId = GetLOID(instance, quizId);

            DBEntityContainer db = GetDB(instance);

            var lo = db.LOResource.Where(l => l.ID == loId).SingleOrDefault();
            Uri baseUri = new Uri(Request.RequestUri.AbsoluteUri.Replace(Request.RequestUri.PathAndQuery, String.Empty));

            string host = baseUri.Host;
            string type = lo.ContentType;
            string fuid = lo.FilePhysicalFolderFUID.ToString();
            string title = lo.FileName;

            string url = string.Format("http://{0}/EdulearnNETUpload/{1}/learningobject/{2}/{3}/{4}",
                            host,
                            instance,
                            type,
                            fuid,
                            title);

            db.Dispose();

            if (url == null)
                throw new HttpResponseException(HttpStatusCode.NotFound);

            var response = Request.CreateResponse(HttpStatusCode.OK);
            response.Content = new StringContent(url);
            return response;

        }

        [Route("{instance}/quiz/{quizId}/{puid}")]
        [HttpGet, HttpOptions]
        public IHttpActionResult Get(string instance, int quizId, string puid)
        {
            var db_questions = this.GetAllQuestionsFromQuiz(instance, quizId, puid);

            DBEntityContainer db = GetDB(instance);

            //var cuepoints = db.QZ_Video_GetQuePointsByQuizId(quizId).ToList();
            /*
             * Retrieve cuepoints info */
            var cuepoints = from v in db.QZVideoQuizQuePoint
                            where v.QuizID == quizId
                            orderby v.QuestionID
                            select new
                            {
                                Id = v.ID,
                                QuestionId = v.QuestionID,
                                QuePoint = v.QuePoint
                            };

            List<QuestionContainer> questions = new List<QuestionContainer>();


            foreach(Question q in db_questions)
            {
                // TODO: linq query to only grab questions whose options is > 0
                // instead of checking each and every question.
                if (q.Options.Count() == 0) 
                    continue;

                int cuepoint = cuepoints.Where(x => x.QuestionId == q.QuestionId).Select(x => x.QuePoint).SingleOrDefault();

                questions.Add(new QuestionContainer { time = cuepoint, question = new List<Question>{q} });

            }


            db.Dispose();

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
        private List<Question> GetAllQuestionsFromQuiz(string instance, int quizId, string puid)
        {
            List<Question> questionList = new List<Question>();

            using (DBEntityContainer db = GetDB(instance))
            {

                /* Get all questions in all sections for a quiz. */
                var result =
                            from s in db.QZSection
                            join q in db.QZQuestion
                                on s.ID equals q.SectionID
                            where s.QuizID == quizId
                            select new { 
                                        Id = q.ID, 
                                        TypeId = q.TypeID, 
                                        Question = q.Question 
                            };

                

                // Get attempt number
                var convertedPUID = Guid.Parse(puid);
                int attempt = db.QZResult
                                .Where(q => q.QuizID == quizId)
                                .Where(q => q.PUID == convertedPUID)
                                .Select(q => q.Attempt)
                                .DefaultIfEmpty(0)
                                .Max();

                attempt += 1;

                foreach (var item in result.ToList())
                {
                    var questionOptions = GetOptionsFromQuestion(instance, item.Id);

                    questionList.Add(
                        new Question {
                            QuestionId = item.Id,
                            TypeId = item.TypeId,
                            Title = item.Question,
                            Options = questionOptions,
                            RecordsResponse = true,
                            CorrectAnswer = String.Empty,
                            CurrentAttempt = attempt,
                            Type = "single"
                        }
                    );
                }

                return questionList;
            }
        }



        /// <summary>
        /// Retrieve LOID given a quizId
        /// </summary>
        /// <param name="quizId"></param>
        /// <returns></returns>
        private int GetLOID(string instance, int quizId)
        {
            DBEntityContainer db = GetDB(instance);

            int loId = db.QZVideoQuizAttachement.Where(q => q.QuizID == quizId).SingleOrDefault().LOID;

            db.Dispose();

            return loId;

        }



        /// <summary>
        /// Call the GetAnswer_MCH stored proc and populate the QuestionOption objects
        /// with the rows returned.
        /// </summary>
        /// <param name="questionId"></param>
        /// <returns></returns>
        private List<QuestionOption> GetOptionsFromQuestion(string instance, int questionId)
        {
            List<QuestionOption> optionList = new List<QuestionOption>();


            using (DBEntityContainer db = GetDB(instance))
            {

                /* Get all options for a particular question */
                var result = from m in db.QZAnswer_MCH
                             where m.QuestionID == questionId
                             orderby m.OptionOrder
                             select m;
                             

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


        private DBEntityContainer GetDB(string instance)
        {

            string databaseServer = Convert.ToString(ConfigurationManager.AppSettings["DatabaseServer"]);
            string databaseUsername = Convert.ToString(ConfigurationManager.AppSettings["DatabaseUsername"]);
            string databasePassword = Convert.ToString(ConfigurationManager.AppSettings["DatabasePassword"]);
            string databaseCatalog = "eduservice_" + instance;

            return DBEntityContainer.ConnectToDatabase(
                                                databaseServer,
                                                databaseCatalog,
                                                databaseUsername,
                                                databasePassword);
        }


    }

}
