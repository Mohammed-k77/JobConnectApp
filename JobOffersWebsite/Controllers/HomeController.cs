using JobOffersWebsite.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;

namespace JobOffersWebsite.Controllers
{
    public class HomeController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public ActionResult Index()
        {
            return View(db.Categories.ToList());
        }

        public ActionResult Details(int JobId)
        {
            var job = db.Jobs.Find(JobId);
            if(job == null)
            {
                return HttpNotFound();  
            }
            Session["JobId"] = JobId;
            return View(job);
        }

        [Authorize]
        [HttpGet]
        public ActionResult Apply()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Apply(string Message)
        {
            var UserId = User.Identity.GetUserId();
            var JobId = (int)Session["JobId"];

            var check = db.ApplyForJobs.Where(a => a.JobId == JobId && a.UserId==UserId).ToList();

            if(check.Count < 1)
            {
                var job = new ApplyForJob();
                job.UserId = UserId;
                job.JobId = JobId;
                job.Message = Message;
                job.Date = DateTime.Now;

                db.ApplyForJobs.Add(job);
                db.SaveChanges();
                ViewBag.Result = "تمت عملية الطلب بنجاح";
            }

            else
            {
                ViewBag.Result = "المعذرة لقد سبق وتقدمت الى هذه الوظيفة ";  
            }
           

            return View();
        }

        [Authorize]
        public ActionResult GetJobsByUser()
        {
            var UserId = User.Identity.GetUserId();
            var Jobs = db.ApplyForJobs.Where( a => a.UserId == UserId );

            return View(Jobs.ToList());  
        }

        [Authorize]
        public ActionResult DetailsOfJob(int id)
        {
            var job = db.ApplyForJobs.Find(id);

            if (job == null)
            {
                return HttpNotFound();
            }

            return View(job);
        }

        [Authorize]
        public ActionResult GetJobsByPublisher()
        {
            var UserId = User.Identity.GetUserId();

            var Jobs = from app in db.ApplyForJobs
                       join job in db.Jobs on app.JobId equals job.Id
                       where job.User.Id == UserId
                       select app;

            var groubed = from j in Jobs
                          group j by j.job.JobTitle
                          into gr
                          select new JobsViewModel
                          {
                              JobTitle = gr.Key,
                              Items = gr
                          };


            return View(groubed.ToList());
        }


        public ActionResult Edit(int id)
        {
            var job = db.ApplyForJobs.Find(id);
            if (job == null)
            {
                return HttpNotFound();
            }
            return View(job);
        }

        [HttpPost]
        public ActionResult Edit(ApplyForJob job)
        {
            if (ModelState.IsValid)
            {
                job.Date = DateTime.Now;
                db.Entry(job).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("GetJobsByUser");
            }           
            return View(job);
        }

        public ActionResult Delete(int id)
        {
            var job = db.ApplyForJobs.Find(id);
            if (job == null)
            {
                return HttpNotFound();
            }
            return View(job);
        }

        // POST: Roles/Delete/5
        [HttpPost]
        public ActionResult Delete(ApplyForJob job)
        {
                // TODO: Add delete logic here
                var myjob = db.ApplyForJobs.Find(job.Id);
                db.ApplyForJobs.Remove(myjob);
                db.SaveChanges();
                return RedirectToAction("GetJobsByUser");
        }



        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        [HttpGet]
        public ActionResult Contact()
        {

            return View();
        }

        [HttpPost]
        public ActionResult Contact(ContactModel contact)
        {
            var mail = new MailMessage();
            var LoginInfo = new NetworkCredential("mk1872022@gmail.com","701074479");
            mail.From = new MailAddress(contact.Email);
            mail.To.Add(new MailAddress("mk1872022@gmail.com"));
            mail.Subject = contact.Subject;
            mail.IsBodyHtml = true; 

            string body = "أسم المرسل :" + contact.Name+"<br>"+
                            "بريد المرسل :" + contact.Email + "<br>" +
                            "عنوان الرسالة:" + contact.Subject + "<br>" +
                            "نص الرسالة :<b>" + contact.Message + "</b>" ;
            mail.Body = body;

            var smtpClient =new SmtpClient("smtp.gmail.com",Convert.ToInt32(587));   
            smtpClient.EnableSsl = true;
            smtpClient.Credentials = LoginInfo;
            smtpClient.Send(mail);  

            return RedirectToAction("Index");
        }

        public ActionResult Search()
        {
            return View();  
        }

        [HttpPost]
        public ActionResult Search(string searchName)
        {
            var result = db.Jobs.Where(j => j.JobTitle.Contains(searchName)
            || j.JobContent.Contains(searchName)
            || j.Category.CategoryName.Contains(searchName)
            || j.Category.CategoryDescription.Contains(searchName)).ToList();  

            return View(result);
        }
    }
    
}