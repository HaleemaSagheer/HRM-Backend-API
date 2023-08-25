using System;
using System.Collections.Generic;
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

        public HttpResponseMessage Assign()
        {
            try
            {
                var util = new Utility();
                util.AssignApplicantsForShortlisting(db);
                return Request.CreateResponse(HttpStatusCode.OK, "Done");
            }
            catch (Exception exp)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, exp);
            }
        }
    }
}