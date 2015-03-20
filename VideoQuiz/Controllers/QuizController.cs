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
            string fake_data = @"
            
            [{'time':3,'items':[{'id':3244,'question':'What is color of sky','recordsResponse':true,'correctAnswer':'','type':'single','options':[{'optionid':11631,'name':'red'},{'optionid':11632,'name':'green'},{'optionid':11633,'name':'blue'},{'optionid':11634,'name':'orange'}]}]},{'time':6,'items':[{'id':3245,'question':'Which 2 are mamals','recordsResponse':true,'correctAnswer':'','type':'single','options':[{'optionid':11635,'name':'Cat'},{'optionid':11636,'name':'Tree'},{'optionid':11637,'name':'Dog'},{'optionid':11638,'name':'Grass'}]}]},{'time':9,'items':[{'id':3246,'question':'Once in a blue moon implies that sometimes the moon is blue','recordsResponse':true,'correctAnswer':'','type':'single','options':[{'optionid':11639,'name':'True'},{'optionid':11640,'name':'false'}]}]},{'time':12,'items':[{'id':3248,'question':'What is color of sky','recordsResponse':true,'correctAnswer':'','type':'single','options':[{'optionid':11641,'name':'red'},{'optionid':11642,'name':'green'},{'optionid':11643,'name':'blue'},{'optionid':11644,'name':'orange'}]}]},{'time':15,'items':[{'id':3249,'question':'Which 2 are mamals','recordsResponse':true,'correctAnswer':'','type':'single','options':[{'optionid':11645,'name':'Cat'},{'optionid':11646,'name':'Tree'},{'optionid':11647,'name':'Dog'},{'optionid':11648,'name':'Grass'}]}]},{'time':18,'items':[{'id':3250,'question':'Once in a blue moon implies that sometimes the moon is blue','recordsResponse':true,'correctAnswer':'','type':'single','options':[{'optionid':11649,'name':'True'},{'optionid':11650,'name':'false'}]}]},{'time':21,'items':[{'id':3252,'question':'<p>What is color of sky</p>','recordsResponse':true,'correctAnswer':'','type':'single','options':[{'optionid':11652,'name':'<p>red</p>'},{'optionid':11651,'name':'<p>green</p>'},{'optionid':11654,'name':'<p>blue</p>'},{'optionid':11653,'name':'<p>orange</p>'}]}]},{'time':24,'items':[{'id':3253,'question':'<p>Which 2 are mamals</p>','recordsResponse':true,'correctAnswer':'','type':'single','options':[{'optionid':11656,'name':'<p>Cat</p>'},{'optionid':11655,'name':'<p>Tree</p>'},{'optionid':11658,'name':'<p>Dog</p>'},{'optionid':11657,'name':'<p>Grass</p>'}]}]},{'time':27,'items':[{'id':3254,'question':'<p>Once in a blue moon implies that sometimes the moon is blue</p>','recordsResponse':true,'correctAnswer':'','type':'single','options':[{'optionid':11659,'name':'True'},{'optionid':11660,'name':'False'}]}]},{'time':30,'items':[{'id':3255,'question':'What is color of sky','recordsResponse':true,'correctAnswer':'','type':'single','options':[{'optionid':11661,'name':'red'},{'optionid':11662,'name':'green'},{'optionid':11663,'name':'blue'},{'optionid':11664,'name':'orange'}]}]},{'time':33,'items':[{'id':3256,'question':'Which 2 are mamals','recordsResponse':true,'correctAnswer':'','type':'single','options':[{'optionid':11665,'name':'Cat'},{'optionid':11666,'name':'Tree'},{'optionid':11667,'name':'Dog'},{'optionid':11668,'name':'Grass'}]}]},{'time':36,'items':[{'id':3257,'question':'Once in a blue moon implies that sometimes the moon is blue','recordsResponse':true,'correctAnswer':'','type':'single','options':[{'optionid':11669,'name':'True'},{'optionid':11670,'name':'false'}]}]},{'time':39,'items':[{'id':3258,'question':'What is color of sky','recordsResponse':true,'correctAnswer':'','type':'single','options':[{'optionid':11671,'name':'red'},{'optionid':11672,'name':'green'},{'optionid':11673,'name':'blue'},{'optionid':11674,'name':'orange'}]}]},{'time':42,'items':[{'id':3259,'question':'Which 2 are mamals','recordsResponse':true,'correctAnswer':'','type':'single','options':[{'optionid':11675,'name':'Cat'},{'optionid':11676,'name':'Tree'},{'optionid':11677,'name':'Dog'},{'optionid':11678,'name':'Grass'}]}]},{'time':45,'items':[{'id':3260,'question':'Once in a blue moon implies that sometimes the moon is blue','recordsResponse':true,'correctAnswer':'','type':'single','options':[{'optionid':11679,'name':'True'},{'optionid':11680,'name':'false'}]}]}]

            ";

            JObject json = JObject.Parse(fake_data);
            return this.Ok(json);

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
