using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using _744Project.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;

namespace _744Project.Controllers
{
    public class SecurityQuestionsController : Controller
    {
        private MasterModel db = new MasterModel();

        static string connectionString = Configuration.getConnectionString();
        SqlConnection connect = new SqlConnection(connectionString);
        //SqlConnection connect = Configuration.getConnectionString();
        static string g_id = "";
        static int g_q1, g_q2, g_q3, g_final_question;
        static int pageRefreshes = 0, g_numOfTries = 0;
        static LoginViewModel model;
        // GET: SecurityQuestions
        public ActionResult Index(string id)
        {
            
            if(User.Identity.IsAuthenticated == true)
            {

            }
            model = new LoginViewModel();
            if(!string.IsNullOrWhiteSpace(id))
                g_id = id;
            pageRefreshes++;
            //The below guarantees us to have three non-redundant different random questions:
            if (pageRefreshes == 1)
            {
                getThreeQuestions();
                model.SecuriyQuestion = getQuestion(g_id, g_q1);  //db.Questions.Find(g_q1).QuestionText;
                g_final_question = g_q1;
            }
            else if(pageRefreshes == 2 )
            {
                model.SecuriyQuestion = getQuestion(g_id, g_q2); //db.Questions.Find(g_q2).QuestionText;
                g_final_question = g_q2;
            }
            else if(pageRefreshes == 3)
            {
                model.SecuriyQuestion = getQuestion(g_id, g_q3);  //db.Questions.Find(g_q3).QuestionText;
                pageRefreshes = 0;
                g_final_question = g_q3;
            }
            
            
            //model.SecuriyQuestion = db.Questions.Find(g_q1).QuestionText;
            ViewBag.Security ="";
            //return View(db.SecurityQuestions.ToList());
            return View(model);
        }
        public void getThreeQuestions()
        {
            Random index = new Random();
            g_q1 = index.Next(1, 4);
            if(g_q1 == 3)
            {
                g_q2 = 1;
                g_q3 = 2;
            }
            else if(g_q1 == 2)
            {
                g_q2 = 3;
                g_q3 = 1;
            }
            else if(g_q1 == 1)
            {
                g_q2 = 2;
                g_q3 = 3;
            }
            
        }
        public string getQuestion(string g_id, int questionId)
        {
            string question = "";
            connect.Open();
            SqlCommand cmd = connect.CreateCommand();
            cmd.CommandText = "select questionText from questions where questionID = '"+questionId+"' ";
            question = cmd.ExecuteScalar().ToString();
            connect.Close();
            return question;
        }
        public Boolean userExists(string name)
        {
            Boolean exists = true;
            connect.Open();
            SqlCommand cmd = connect.CreateCommand();
            cmd.CommandText = "select count(*) from AspNetUsers where Email like '"+name+"' ";
            int count = Convert.ToInt32(cmd.ExecuteScalar());
            if(count == 0)
            {
                exists = false;
            }
            connect.Close();
            return exists;
        }



        public Boolean checkAnswer(int question, string answer, string id)
        {
            Boolean correct = true;
            connect.Open();
            SqlCommand cmd = connect.CreateCommand();
            cmd.CommandText = "select count(*) from SecurityQuestions where AspNetUserID like '"+id+"' and questionID = '"+question+"' and answer like '"+answer+"' ";
            int count = Convert.ToInt32(cmd.ExecuteScalar());
            if (count == 0)
                correct = false;
            connect.Close();
            return correct;
        }      
       
        public void lockUser(string id)
        {
            connect.Open();
            SqlCommand cmd = connect.CreateCommand();
            cmd.CommandText = "update AspNetUsers set numFailedAttempts = 4 where Id like '"+id+"' ";
            cmd.ExecuteScalar();
            connect.Close();
        }
        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }
        //POST: SecurityQuestions/Cancel
        [HttpPost]        
        public ActionResult Cancel()
        {
            logoutUser();            
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public ActionResult logoutUser()
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            return RedirectToAction("Index", "Home");
        }
        [HttpPost]
        public ActionResult Index(string answer, string name)
        {            
            g_numOfTries++;
            Boolean correct = checkAnswer(g_final_question, answer, g_id);
            string id = g_id;
            if (correct)
            {
                g_numOfTries = 0;
                return RedirectToAction("Index", "");
            }
            else
            {
                if (g_numOfTries > 2)
                {
                    g_numOfTries = 0;
                    ModelState.AddModelError("answer", "Input Error: You have entered the answer three times already.");
                    lockUser(id);
                    logoutUser();
                    //return View(model);
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError("answer", "Input Error: Please provide a correct answer.");
                    return View(model);
                    //return RedirectToAction("Index", "SecurityQuestions", new { id });
                }
            }
            //var answers = model.AnswerToSecurityQuestion;
            //var question = model.SecuriyQuestion;
            //var QuestionIDUser = (from ans in db.SecurityQuestions where ans.Answer == answers select ans).FirstOrDefault();

            //int QuestionID;
            //if (QuestionIDUser != null)
            //{
            //    QuestionID = QuestionIDUser.QuestionID;
            //}
            //else
            //{
            //    QuestionID = -1;
            //}
            //return RedirectToAction("Index");
        }

        // GET: SecurityQuestions/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SecurityQuestion securityQuestion = db.SecurityQuestions.Find(id);
            if (securityQuestion == null)
            {
                return HttpNotFound();
            }
            return View(securityQuestion);
        }

        // GET: SecurityQuestions/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: SecurityQuestions/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "SecurityQuestionsID,AspNetUserID,QuestionID,Answer")] SecurityQuestion securityQuestion)
        {
            if (ModelState.IsValid)
            {
                db.SecurityQuestions.Add(securityQuestion);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(securityQuestion);
        }

        // GET: SecurityQuestions/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SecurityQuestion securityQuestion = db.SecurityQuestions.Find(id);
            if (securityQuestion == null)
            {
                return HttpNotFound();
            }
            return View(securityQuestion);
        }

        // POST: SecurityQuestions/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "SecurityQuestionsID,AspNetUserID,QuestionID,Answer")] SecurityQuestion securityQuestion)
        {
            if (ModelState.IsValid)
            {
                db.Entry(securityQuestion).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(securityQuestion);
        }

        

        // GET: SecurityQuestions/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SecurityQuestion securityQuestion = db.SecurityQuestions.Find(id);
            if (securityQuestion == null)
            {
                return HttpNotFound();
            }
            return View(securityQuestion);
        }

        // POST: SecurityQuestions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            SecurityQuestion securityQuestion = db.SecurityQuestions.Find(id);
            db.SecurityQuestions.Remove(securityQuestion);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
