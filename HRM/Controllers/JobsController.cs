using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Http;
using HRM.Models;

namespace HRM.Controllers
{
    public class JobsController : ApiController
    {
        HRMEntities2 db = new HRMEntities2();
        // GET api/<controller>
        [HttpPost]
        public  HttpResponseMessage AddNewJob(Job j)
        {
            try
            {
                db.Jobs.Add(j);
                db.SaveChanges();
                return Request.CreateResponse(HttpStatusCode.InternalServerError, "New job Added Sucessfully");
            }
            catch (Exception exp)
            {

                return Request.CreateResponse(HttpStatusCode.InternalServerError, exp.Message);
            }
        }

        [HttpPost]
        public HttpResponseMessage UpdateJob(Job job)
        {
            try
            {
                var isRecord = db.Jobs.Find(job.id);
                if(isRecord==null)
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound, "This job  does not exist");
                }
                else
                {
                    if(isRecord.is_active==true)
                    {
                        db.Entry(isRecord).CurrentValues.SetValues(job);
                        db.SaveChanges();
                        return Request.CreateResponse(HttpStatusCode.OK, " job Updated Sucessfully");
                    }
                    else
                    {
                        return Request.CreateResponse(HttpStatusCode.OK, "This job is not active now ");
                    }
                    
                }
            }
            catch (Exception exp)
            {

                return Request.CreateResponse(HttpStatusCode.InternalServerError, exp.Message);
            }
        }

        [HttpPost]
        public HttpResponseMessage ArchiveJob(int j_id)
        {
            try
            {
                var isRecord = db.Jobs.Find(j_id);
                if (isRecord == null)
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound, "This job  does not exist");
                }
                else
                {
                    isRecord.is_active = false;
                    db.Entry(isRecord).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                    return Request.CreateResponse(HttpStatusCode.OK, " Job Archived Sucessfully");
                }
            }
            catch (Exception exp)
            {

                return Request.CreateResponse(HttpStatusCode.InternalServerError, exp.Message);
            }
        }
        [HttpGet]
        public HttpResponseMessage GetAllPostedJobs()
        {
            try
            {
                var allJobs = db.Jobs.OrderBy(x => x.id).ToList();
                return Request.CreateResponse(HttpStatusCode.OK, allJobs);
            }
            catch (Exception exp)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, exp);
            }
        }

        [HttpGet]
        public HttpResponseMessage GetAllOpenJobs()
        {
            try
            {
                var openJobs = db.Jobs.Where(x => x.is_active == true).ToList();
                return Request.CreateResponse(HttpStatusCode.OK, openJobs);
            }
            catch (Exception exp)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, exp);
            }
        }
        [HttpGet]
        public HttpResponseMessage GetJobIdByCommitteeId(int c_id)
        {
            try
            {
                var job = db.CommitteeJobs.Where(x => x.committee_id==c_id).FirstOrDefault();
                if(job!=null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, job.job_id);
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.InternalServerError, 0);
                }
            }
            catch (Exception exp)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, exp);
            }
        }

        [HttpGet]
        public HttpResponseMessage GetJobById( int j_id)
        {
            try
            {
                var job = db.Jobs.Where(x => x.id == j_id).FirstOrDefault();
                return Request.CreateResponse(HttpStatusCode.OK, job);
            }
            catch (Exception exp)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, exp);
            }
        }

        [HttpGet]
        public HttpResponseMessage GetJobApplicants(int j_id)
        {
            try
            {
                var applicants = (from apply in db.Applies
                                  join user in db.Users on apply.user_id equals user.id
                                  where apply.job_id == j_id
                                  select new
                                  {
                                      user.id,
                                      user.name,
                                  }).ToList();

                var response = Request.CreateResponse(HttpStatusCode.OK, applicants);
                response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                return response;
            }
            catch (Exception exp)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, exp);
            }
        }
        //GIVE AN  AAPLICANTiD  AND ITWILL RETURN YOU ALL HIS INFO
        [HttpGet]
        public HttpResponseMessage GetApplicantDetails(int applicant_id)
        {
            try
            {
                var applicantDetails = (from user in db.Users
                                        where user.id == applicant_id
                                        select new
                                        {
                                            user.name,
                                            user.dob,
                                            user.address,
                                            user.mobile_num,
                                            user.email,
                                            user.gender,
                                            // Add other properties from the User table
                                            EducationalInformation = (from edu in db.ApplyEducations
                                                                      where edu.applicant_id == applicant_id
                                                                      select new
                                                                      {
                                                                          edu.degree,
                                                                          edu.institution,
                                                                          edu.date_from,
                                                                          edu.date_to,
                                                                      }).ToList(),
                                            ExperienceInformation = (from exp in db.ApplyExperiences
                                                                     where exp.user_id == applicant_id
                                                                     select new
                                                                     {
                                                                         exp.position,
                                                                         exp.from_date,
                                                                         exp.to_date,
                                                                         exp.organization,
                                                                         exp.is_current,
                                                                     }).ToList(),
                                        }).FirstOrDefault();

                var response = Request.CreateResponse(HttpStatusCode.OK, applicantDetails);
                response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                return response;
            }
            catch (Exception exp)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, exp);
            }
        }
        ///THIS FUNCTION
        [HttpGet]
        public IHttpActionResult GetApplicantCV(int applicantId)
        {
            try
            {
                var applicant = db.Users.SingleOrDefault(x => x.id == applicantId);

                if (applicant == null)
                {
                    return NotFound(); // Return 404 if applicant not found
                }

                var applyRecord = db.Applies.SingleOrDefault(a => a.user_id == applicantId);

                if (applyRecord == null || string.IsNullOrEmpty(applyRecord.cv_path))
                {
                    return NotFound(); // Return 404 if no CV path found
                }

                // Get the CV file path from the applyRecord
                var cvFilePath = applyRecord.cv_path;

                // Read the CV file content and return it as a response
                byte[] cvFileBytes = File.ReadAllBytes(cvFilePath);
                var response = new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new ByteArrayContent(cvFileBytes)
                };
                response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
                {
                    FileName = "applicant_cv.pdf" // Change the filename as needed
                };
                response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/pdf"); // Change the content type if needed

                return ResponseMessage(response);
            }
            catch (Exception exp)
            {
                return InternalServerError(exp);
            }
        }


        [HttpGet]

        public HttpResponseMessage GetAllUnAssignedJobs()

        {

            try

            {
                var notAssignedJobs = new List<Job>();

                foreach (var job in db.Jobs)

                {

                    if (db.CommitteeJobs.FirstOrDefault(cj => cj.job_id == job.id) == null && job.is_active == true)

                        notAssignedJobs.Add(job);

                }

                return Request.CreateResponse(HttpStatusCode.OK, notAssignedJobs);

            }

            catch (Exception exp)

            {

                return Request.CreateResponse(HttpStatusCode.InternalServerError, exp.Message);

            }







        }
        [HttpGet]
        public HttpResponseMessage GetCommittees(int u_id)
        {
            try
            {
                var committee = db.CommitteeMembers.Where(x => x.user_id == u_id).ToList();
                return Request.CreateResponse(HttpStatusCode.OK, committee);
            }
            catch (Exception exp)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, exp);
            }
        }


        //[HttpPost]
        //public HttpResponseMessage UploadFile(int userId,int jobId)
        //{
        //    HttpResponseMessage result = null;
        //    var httpRequest = HttpContext.Current.Request;
        //    if (httpRequest.Files.Count > 0)
        //    {
        //        var docfiles = new List<string>();
        //        foreach (string file in httpRequest.Files)
        //        {
        //            var postedFile = httpRequest.Files[file];
        //            var filename = DateTime.Now.ToFileTime().ToString() + ".pdf";
        //            var filePath = HttpContext.Current.Server.MapPath("~/App_Data/CVs/" + filename);
        //            postedFile.SaveAs(filePath);
        //            docfiles.Add(filePath);
        //            db.Applies.Add(new Apply {apply_date = DateTime.Now, cv_path = filename, status="Applied", user_id=userId, job_id=jobId });
        //            db.SaveChanges();
        //        }
        //        result = Request.CreateResponse(HttpStatusCode.Created, docfiles);
        //    }
        //    else
        //    {
        //        result = Request.CreateResponse(HttpStatusCode.BadRequest);
        //    }
        //    return result;
        //}
        [HttpPost]
        public HttpResponseMessage UploadFile(int userId, int jobId)
        {
            HttpResponseMessage result = null;
            var httpRequest = HttpContext.Current.Request;

            // Check if any files were uploaded in the request
            if (httpRequest.Files.Count > 0)
            {
                var docfiles = new List<string>();

                // Loop through each uploaded file
                foreach (string file in httpRequest.Files)
                {
                    var postedFile = httpRequest.Files[file];

                    // Generate a unique filename using the current timestamp
                    var filename = DateTime.Now.ToFileTime().ToString() + ".pdf";

                    // Specify the destination path for saving the file
                    var filePath = HttpContext.Current.Server.MapPath("~/App_Data/CVs/" + filename);

                    // Save the uploaded file to the specified path
                    postedFile.SaveAs(filePath);

                    // Keep track of the saved file paths
                    docfiles.Add(filePath);

                    // Add an entry to the database with information about the application
                    db.Applies.Add(new Apply
                    {
                        apply_date = DateTime.Now,
                        cv_path = filename,
                        status = "Applied",
                        user_id = userId,
                        job_id = jobId
                    });

                    // Save changes to the database
                    db.SaveChanges();
                }

                // Create a response indicating successful creation of files
                result = Request.CreateResponse(HttpStatusCode.Created, docfiles);
            }
            else
            {
                // If no files were uploaded, create a response indicating bad request
                result = Request.CreateResponse(HttpStatusCode.BadRequest);
            }

            // Return the appropriate response
            return result;
        }

    }
}