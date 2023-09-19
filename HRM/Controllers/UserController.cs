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
    public class UserController : ApiController
    {
        HRMEntities2 db = new HRMEntities2();
        [HttpPost]
        public HttpResponseMessage Login(String email,string password) 
        {
            try
            {

                var util = new Utility();
                //var util = new Utility();
                util.AssignApplicantsForShortlisting(db);

                User login = db.Users.FirstOrDefault(x => x.email == email && x.password == password);

                if (login != null)
                {

                    return Request.CreateResponse(HttpStatusCode.OK,new {  Message=login.name + "  Login Successfully",login});
                    // return Request.CreateResponse(HttpStatusCode.OK,login);
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.Unauthorized, "Invalid email or password");
                }
            }
            catch (Exception exp)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, exp.Message);
            }
        }

        [HttpPost]
        public HttpResponseMessage UpdateUser()
        {
            try
            {
                var form = HttpContext.Current.Request.Form;

                // Parsing userId
                int userId;
                if (!int.TryParse(form["id"], out userId))
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, "Invalid userId");
                }

                // Extracting user data from the form
                string name = form["name"];
                string cnic = form["cnic"];
                string mobile_num = form["mobile_num"];
                string dob = form["dob"];
                string gender = form["gender"];
                string address = form["address"];
                string password = form["password"];
                string email = form["email"];

                // Extracting uploaded image file
                var file = HttpContext.Current.Request.Files["Image"];

                // Current date and time
                DateTime dt = DateTime.Now;

                // Creating a directory for image storage
                string path = HttpContext.Current.Server.MapPath("~/Images");
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                string fileData = null;
                if (file != null && file.ContentLength > 0)
                {
                    // Generating a unique filename using timestamp and original filename
                    fileData = $"{dt:yyyy_MM_dd_HH_mm_ss_fff}_{file.FileName}";

                    // Saving the uploaded file to the specified path
                    file.SaveAs(Path.Combine(path, fileData));
                }

                // Parsing date of birth
                DateTime date = DateTime.ParseExact(dob, "MM/dd/yyyy", CultureInfo.InvariantCulture);

                // Finding the user in the database
                User u = db.Users.Find(userId);
                if (u != null)
                {
                    // Updating user data
                    u.name = name;
                    u.cnic = cnic;
                    u.email = email;
                    u.mobile_num = mobile_num;
                    u.cnic = cnic;
                    u.dob = date;
                    u.gender = gender;
                    u.address = address;
                    u.image = fileData ?? fileData; // If fileData is null, keep the existing Image value
                    db.SaveChanges(); // Save changes to the database

                    return Request.CreateResponse(HttpStatusCode.OK, "User data updated successfully");
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound, "User not found");
                }
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, $"Error: {ex.Message}");
            }
        }


        //[HttpPost]
        //public HttpResponseMessage SignUp(User usr)
        //{

        //    try
        //    {
        //        var usrRecord = db.Users.FirstOrDefault(x => x.email == usr.email);

        //        if (usrRecord == null)
        //        {

        //            var file = HttpContext.Current.Request.Files.Count > 0 ?
        //              HttpContext.Current.Request.Files[0] : null;

        //            if (file != null && file.ContentLength > 0)
        //            {
        //                var fileName = Path.GetFileName(file.FileName);

        //                var path = Path.Combine(
        //                    HttpContext.Current.Server.MapPath("~/App_Data/uploads"),
        //                    fileName
        //                );

        //                usr.image = fileName;
        //                file.SaveAs(path);
        //            }


        //            db.Users.Add(usr);
        //            return Request.CreateResponse(HttpStatusCode.OK, "Successfully added");
        //        }
        //        else
        //        {
        //            return Request.CreateResponse(HttpStatusCode.BadRequest, "User already exists");
        //        }
        //    }
        //    catch (Exception exp)
        //    {

        //        return Request.CreateResponse(HttpStatusCode.InternalServerError, exp.Message);
        //    }

        //}

        [HttpPost]
        public HttpResponseMessage Signup( User u)
        {
            try
            {
                if (db.Users.Any(b => b.email == u.email))
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, "Email already exists");
                }
                //Insert Into User Table
                var users = db.Users.Add(u);
                db.SaveChanges();
                return Request.CreateResponse(HttpStatusCode.OK, u.name + "  is  added");
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [HttpGet]
        public HttpResponseMessage GetAllEmployees()
        {
            try
            {
                var employees = db.Users?.Where(x => x.role == "Employee").OrderBy(x => x.name).ToList();
                return Request.CreateResponse(HttpStatusCode.OK, employees);
            }
            catch (Exception exp)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, exp);
            }
        }
        //TO GET ALL THE EMPLOYEES WHO ARE NOT IN THE MEMBER OF COMITTEE 
        [HttpGet]
        public HttpResponseMessage GetAllactivatedEmployee(int e_id)
        {
            try
            {
                var employees = db.Users?.Where(x => x.role == "Employee"&& x.id!=e_id).OrderBy(x => x.name).ToList();
                return Request.CreateResponse(HttpStatusCode.OK, employees);
            }
            catch (Exception exp)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, exp);
            }
        }

        [HttpGet]
        public HttpResponseMessage GetAllHeads()
        {
            try
            {
                var employees = db.Users?.Where(x => x.role == "HOD").OrderBy(x => x.name).ToList();
                return Request.CreateResponse(HttpStatusCode.OK, employees);
            }
            catch (Exception exp)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, exp);
            }
        }

        [HttpGet]
        public HttpResponseMessage Assign()
        {
            try
            {
                //var util = new Utility();
                var util = new Utility();
                util.AssignApplicantsForShortlisting(db);
                return Request.CreateResponse(HttpStatusCode.OK, "Done");
            }
            catch (Exception exp)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, exp);
            }
        }

        [HttpPost]
        public HttpResponseMessage DeactivateMember(int c_id, int u_id)

        {
            try

            {

                var isRecord = db.CommitteeMembers.Where(x => x.committee_id == c_id && x.user_id == u_id).FirstOrDefault();

                if (isRecord == null)

                {

                    return Request.CreateResponse(HttpStatusCode.NotFound, "This user   does not exist in the commitee");

                }

                else

                {

                    if(isRecord.is_activated == false)
                    {
                        isRecord.is_activated = true;
                        db.Entry(isRecord).State = System.Data.Entity.EntityState.Modified;

                        db.SaveChanges();

                        return Request.CreateResponse(HttpStatusCode.OK, " the user is Activated again Sucessfully");
                    }
                    else
                    {
                        isRecord.is_activated = false;
                        db.Entry(isRecord).State = System.Data.Entity.EntityState.Modified;

                        db.SaveChanges();

                        return Request.CreateResponse(HttpStatusCode.OK, " the user is deactivated Sucessfully");

                    }
                    

                   

                }

            }

            catch (Exception exp)

            {



                return Request.CreateResponse(HttpStatusCode.InternalServerError, exp.Message);

            }

        }
       
    }
}