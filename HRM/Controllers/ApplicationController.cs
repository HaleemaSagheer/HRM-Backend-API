using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using HRM.Models;

namespace HRM.Controllers
{
    public class ApplicationController : ApiController
    {
        HRMEntities2 db = new HRMEntities2();
        // GET api/<controller>
        [HttpGet]
        public HttpResponseMessage GetAllApplicationByJob(int j_id)
        {
            try
            {
                var allApplications = db.Applies.Where(x => x.job_id == j_id).ToList();
                return Request.CreateResponse(HttpStatusCode.OK, allApplications);
            }
            catch (Exception exp)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, exp);
            }
        }

        [HttpGet]
        public HttpResponseMessage GetAllApplicationByUserId(int user_id)
        {
            try
            {
                var allApplications = db.Applies.Where(x => x.user_id == user_id).ToList();
                return Request.CreateResponse(HttpStatusCode.OK, allApplications);
            }
            catch (Exception exp)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, exp);
            }
        }
        [HttpPost]
        public HttpResponseMessage AddApply(Apply a)
        {
            try
            {
                db.Applies.Add(a);
                db.SaveChanges();
                return Request.CreateResponse(HttpStatusCode.InternalServerError, "Applied for job Sucessfully");
            }
            catch (Exception exp)
            {

                return Request.CreateResponse(HttpStatusCode.InternalServerError, exp.Message);
            }
        }

        [HttpPost]
        public HttpResponseMessage AddApplyEducation(ApplyEducation Edu)
        {
            try
            {
                
                db.ApplyEducations.Add(Edu);
                db.SaveChanges();
                return Request.CreateResponse(HttpStatusCode.InternalServerError, "Education Added  Sucessfully");
            }
            catch (Exception exp)
            {

                return Request.CreateResponse(HttpStatusCode.InternalServerError, exp.Message);
            }
        }
        [HttpPost]
        public HttpResponseMessage AddApplyExperience(ApplyExperience ex)
        {
            try
            {
                db.ApplyExperiences.Add(ex);
                db.SaveChanges();
                return Request.CreateResponse(HttpStatusCode.InternalServerError, "Experience  Added Sucessfully");
            }
            catch (Exception exp)
            {

                return Request.CreateResponse(HttpStatusCode.InternalServerError, exp.Message);
            }
        }

        
    }
}