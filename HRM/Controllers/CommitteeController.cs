using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using HRM.Models;

namespace HRM.Controllers
{
    public class CommitteeController : ApiController
    {
        // GET api/<controller>
        HRMEntities2 db = new HRMEntities2();
        [HttpGet]
        public HttpResponseMessage  GetAllCommittees()
        {
            try
            {
                var comm = db.Committees.OrderBy(x => x.id).ToList();
                return Request.CreateResponse(HttpStatusCode.OK, comm);
            }
            catch (Exception exp)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, exp);
            }
        }

        //Get All that committeess in which you are head
        [HttpGet]
        public HttpResponseMessage GetAllHeadCommitteesByEmployeeeId(int user_id)
        {
            try
            {
                var comm = db.Committees.Where(x => x.user_id==user_id).ToList();
                return Request.CreateResponse(HttpStatusCode.OK, comm);
            }
            catch (Exception exp)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, exp);
            }
        }
        //Get All that committeess in which you are member
        [HttpGet]
        public HttpResponseMessage GetAllCommitteesByEmployeeId(int user_id)
        {
            try
            {
                var committees = (from cm in db.CommitteeMembers
                                  join c in db.Committees on cm.committee_id equals c.id
                                  where cm.user_id == user_id
                                  select new
                                  {
                                      CommitteeId = c.id,
                                      CommitteeTitle = c.title,
                                      UserId = cm.user_id
                                  }).ToList();

                return Request.CreateResponse(HttpStatusCode.OK, committees);
            }
            catch (Exception exp)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, exp);
            }
        }

        [HttpGet]
        public HttpResponseMessage GetSingleCommittee(int id)
        {
            try
            {
                var Scomm = db.Committees.Where(x => x.id == id).FirstOrDefault();
                return Request.CreateResponse(HttpStatusCode.OK, Scomm);
            }
            catch (Exception exp)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, exp);
            }
        }

        [HttpGet]
        public HttpResponseMessage GetCommitteeMembers(int id)
        {
            try
            {
                var members = db.CommitteeMembers.Where(x => x.committee_id == id).FirstOrDefault();
                return Request.CreateResponse(HttpStatusCode.OK, members);
            }
            catch (Exception exp)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, exp);
            }
        }

        [HttpGet]
        public HttpResponseMessage GetCommitteeHead(string title)
        {
            try
            {
                var head = db.Committees.Where(x => x.title == title).FirstOrDefault();
                return Request.CreateResponse(HttpStatusCode.OK, head);
            }
            catch (Exception exp)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, exp);
            }
        }

        [HttpGet]
        public HttpResponseMessage GetCommitteeRemarks(int a_id, int j_id)
        {
            try
            {
                var result = db.CommitteeRemarks
                    .Where(x => x.applicant_id == a_id && x.job_id == j_id)
                    .Join(db.Users,
                        remarks => remarks.committee_member_id,
                        committeeMember => committeeMember.id,
                        (remarks, committeeMember) => new
                        {
                            remarks.applicant_id,
                            remarks.job_id,
                            remarks.remark,
                            CommitteeMemberId = committeeMember.id, // Include the CommitteeMemberId
                    CommitteeMemberName = committeeMember.name
                        })
                    .ToList();

                return Request.CreateResponse(HttpStatusCode.OK, result);
            }
            catch (Exception exp)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, exp);
            }
        }



        [HttpGet]
        public HttpResponseMessage GetLastCommitteeId()
        {
            try
            {
                var lastCommittee = db.Committees.OrderByDescending(x => x.id).FirstOrDefault();

                if (lastCommittee != null)
                {
                    int lastCommitteeId = lastCommittee.id;
                    return Request.CreateResponse(HttpStatusCode.OK, lastCommitteeId);
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound, "No committees found.");
                }
            }
            catch (Exception exp)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, exp);
            }
        }

        [HttpPost]
        public HttpResponseMessage AddNewCommittee(Committee c)
        {
            try
            {
                db.Committees.Add(c);
                db.SaveChanges();
                return Request.CreateResponse(HttpStatusCode.OK, c.title + "  " + "Commmitee Added Successfully");
            }
            catch (Exception exp)
            {

                return Request.CreateResponse(HttpStatusCode.InternalServerError, exp.Message);
            }
        }

        //#################################################

        //[HttpPost]
        //public HttpResponseMessage EditCommittee(Committee c)
        //{
        //    try
        //    {
        //        db.Committees.Add(c);
        //        db.SaveChanges();
        //        return Request.CreateResponse(HttpStatusCode.OK, c.title + "  " + "Commmitee Added Successfully");
        //    }
        //    catch (Exception exp)
        //    {

        //        return Request.CreateResponse(HttpStatusCode.InternalServerError, exp.Message);
        //    }
        //}
        //################################################
        [HttpPost]
        public HttpResponseMessage UpdateCommmittee(Committee c)
        {
            try
            {
                var isRecord = db.Committees.Find(c.id);
                if (isRecord == null)
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound, "This committee does not exist ");
                }
                else
                {
                    db.Entry(isRecord).CurrentValues.SetValues(c);
                    db.SaveChanges();
                    return Request.CreateResponse(HttpStatusCode.OK, "Committee Updated Sucessfully");
                }
            }
            catch (Exception exp)
            {

                return Request.CreateResponse(HttpStatusCode.InternalServerError, exp.Message);
            }
        }
        [HttpDelete]
        public HttpResponseMessage DeleteCommittee(int c_id)
        {
            try
            {
                var IsRecord = db.Committees.Find(c_id);
                if(IsRecord==null)
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound, "Committee does not exist");
                }
                else
                {
                    db.Entry(IsRecord).State = System.Data.Entity.EntityState.Deleted;
                    db.SaveChanges();
                    return Request.CreateResponse(HttpStatusCode.OK, "Committee  Deleted Successfully");
                }
            }
            catch (Exception exp)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, exp.Message);
            }
        }
        [HttpPost]
        public HttpResponseMessage AddCommitteeMember(CommitteeMember member)
        {
            try
            {

                var existingMember = db.CommitteeMembers.FirstOrDefault(c => c.committee_id == member.committee_id && c.user_id == member.user_id);
                if (existingMember != null)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, "User already exists as member in  the Committee");
                }
                else
                {
                    db.CommitteeMembers.Add(member);
                    db.SaveChanges();
                    return Request.CreateResponse(HttpStatusCode.OK, "Employee with  UserId" + "  " + member.user_id + "  " + " is Added  to Committtee" + " " + member.committee_id + " " +
                        " as Member");
                }
            }
            catch (Exception exp)
            {

                return Request.CreateResponse(HttpStatusCode.InternalServerError, exp.Message);
            }
        }

        //############################//
        //############################//

        // Herefor this function i created a new class   named as  CommitteeMembersRequest  to passs multiple userid against the 
        // one committee id 
        [HttpPost]
        public HttpResponseMessage AddCommitteeMembers(CommitteeMembersRequest request)
        {
            try
            {
                var committeeId = request.committee_id;
                List<string> successMessages = new List<string>();
                List<string> errorMessages = new List<string>();

                foreach (var userId in request.user_ids)
                {
                    var existingMember = db.CommitteeMembers.FirstOrDefault(c => c.committee_id == committeeId && c.user_id == userId);
                    if (existingMember != null)
                    {
                        errorMessages.Add("User with UserId " + userId + " already exists as a member in Committee " + committeeId);
                    }
                    else
                    {
                        var newMember = new CommitteeMember
                        {
                            committee_id = committeeId,
                            user_id = userId
                        };
                        db.CommitteeMembers.Add(newMember);
                        successMessages.Add("User with UserId " + userId + " added to Committee " + committeeId + " as a member.");
                    }
                }

                db.SaveChanges();

                if (successMessages.Count > 0)
                {
                    var successMessage = string.Join("\n", successMessages);
                    if (errorMessages.Count > 0)
                    {
                        var errorMessage = string.Join("\n", errorMessages);
                        return Request.CreateResponse(HttpStatusCode.OK, successMessage + "\n\n" + errorMessage);
                    }
                    else
                    {
                        return Request.CreateResponse(HttpStatusCode.OK, successMessage);
                    }
                }
                else
                {
                    var errorMessage = string.Join("\n", errorMessages);
                    return Request.CreateResponse(HttpStatusCode.BadRequest, errorMessage);
                }
            }
            catch (Exception exp)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, exp.Message);
            }
        }


        //############################//
        //############################//

        [HttpPost]
        public HttpResponseMessage UpdateCommitteeMember(CommitteeMember member)
        {
            try
            {
                var IsREcord = db.CommitteeMembers.Find(member.committee_id);
                if(IsREcord==null)
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound, "Committee does not exist ");
                }
                else
                {
                    var existingMember = db.CommitteeMembers.FirstOrDefault(c => c.committee_id == member.committee_id && c.user_id == member.user_id);
                    if (existingMember != null)
                    {
                        return Request.CreateResponse(HttpStatusCode.BadRequest,"User already exists as member in  the Committee"); 
                    }
                    else
                    {
                        IsREcord.user_id = member.user_id;
                        db.Entry(IsREcord).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();
                        return Request.CreateResponse(HttpStatusCode.OK, " Updated Sucessfully ");
                    }
                }
            }
            catch (Exception exp)
            {

                return Request.CreateResponse(HttpStatusCode.InternalServerError, exp.Message);
            }
        }
        [HttpPost]
        public HttpResponseMessage DeleteCommitteeMember(CommitteeMember member)
        {
            try
            {
                var IsREcord = db.CommitteeMembers.Find(member.committee_id);
                if (IsREcord == null)
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound, "Committee does not exist ");
                }
                else
                {
                    db.Entry(IsREcord).State = System.Data.Entity.EntityState.Deleted;
                    db.SaveChanges();
                    return Request.CreateResponse(HttpStatusCode.OK, member.user_id + "    Deleted Sucessfully  from  committee");
                }
            }
            catch (Exception exp)
            {

                return Request.CreateResponse(HttpStatusCode.InternalServerError, exp.Message);
            }
        }

        [HttpPost]
        public HttpResponseMessage AddCommitteeRemarks( CommitteeRemark remarks)
        {
            try
            {
                db.CommitteeRemarks.Add(remarks);
                db.SaveChanges();
                return Request.CreateResponse(HttpStatusCode.OK, "Commitee Member with Id    "+ remarks.committee_member_id+
                    "Added his remarks for Applicant with Id   "+remarks.applicant_id);
            }
            catch (Exception exp)
            {

                return Request.CreateResponse(HttpStatusCode.InternalServerError, exp.Message);
            }
        }
        [HttpPost]
        public HttpResponseMessage ChangeApplicantStatus(int a_id,int j_id)
        {
            try
            {
                var isRecord = db.Applies.Where(x=>x.user_id==a_id &&x.job_id==j_id).FirstOrDefault();
                if (isRecord == null)
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound, "This  applicant does not exist");
                }
                else
                {
                    isRecord.status = "Interview";
                    db.Entry(isRecord).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                    return Request.CreateResponse(HttpStatusCode.OK, " Interview completed  Sucessfully");
                }
            }
            catch (Exception exp)
            {

                return Request.CreateResponse(HttpStatusCode.InternalServerError, exp.Message);
            }
        }

        //////////////////////////////////////// INCOMPLETE///////////////////////////////////////////
        [HttpPost]
        public HttpResponseMessage UpdateCommitteeRemarks(CommitteeRemark remarks)
        {
            try
            {

                var IsRecord = db.CommitteeRemarks.FirstOrDefault(x => x.committee_member_id == remarks.committee_member_id &&
                x.applicant_id == remarks.applicant_id && x.job_id == remarks.job_id);
                if (IsRecord == null)
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound, "Does not exist any record against this applicant ");
                }
                else
                {
                    IsRecord.remark = remarks.remark;
                    db.Entry(IsRecord).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                    return Request.CreateResponse(HttpStatusCode.OK, "Remarks Updated Sucessfully");
                }

            }
            catch (Exception exp)
            {

                return Request.CreateResponse(HttpStatusCode.InternalServerError, exp.Message);
            }
        }
    
        [HttpPost]
        public HttpResponseMessage LinkCommitteeToJob(CommitteeJob c)
        {
            try
            {
                bool isJobAssigned = db.CommitteeJobs.Any(x => x.job_id == c.job_id);
                if (isJobAssigned)
                {
                    //  If  a Job is already assigned, return a message
                    return Request.CreateResponse(HttpStatusCode.BadRequest, "This job is already assigned to a committee.");
                }
                db.CommitteeJobs.Add(c);
                db.SaveChanges();
                return Request.CreateResponse(HttpStatusCode.OK,"This Committee  is asgined  for job id "+c.job_id);
            }
            catch (Exception exp)
            {

                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, exp.Message);
            }
        }





        //GET COMMITTEE DETAILS

        [HttpGet]
        public HttpResponseMessage GetCommitteeDetail(int committeeId)
        {
            try
            {
                var committeeDetail = new CommitteeDetail();

                // Get committee information
                var committee = db.Committees.Find(committeeId);
                if (committee == null)
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound, "Committee does not exist");
                }
                committeeDetail.CommitteeInfo = committee;

                // Get committee head
                var committeeHeadId = committee.user_id;
                var committeeHead = db.Users.Find(committeeHeadId);
                if (committeeHead != null)
                {
                    committeeDetail.CommitteeHead = committeeHead;
                }

                // Get all committee members
                var committeeMemberIds = db.CommitteeMembers.Where(cm => cm.committee_id == committeeId).Select(cm => cm.user_id).ToList();
                var committeeMembers = db.Users.Where(u => committeeMemberIds.Contains(u.id) && u.id != committeeHeadId).ToList();
                committeeDetail.CommitteeMembers = committeeMembers;

                // Get assigned job IDs
                var assignedJobIds = db.CommitteeJobs.Where(cj => cj.committee_id == committeeId)
                                             .Select(cj => cj.job_id)
                                             .ToList();

                // Retrieve job titles using LINQ to Entities
                var assignedJobTitles = db.Jobs.Where(j => assignedJobIds.Contains(j.id))
                                               .Select(j => j.title)
                                               .ToList();

                committeeDetail.AssignedJobTitles = assignedJobTitles;

                return Request.CreateResponse(HttpStatusCode.OK, committeeDetail);
            }
            catch (Exception exp)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, exp.Message);
            }
        }




        //        var assignedJobs = db.CommitteeJobs.Where(cj => cj.committee_id == committeeId).ToList();
        //        committeeDetail.AssignedJobs = assignedJobs;

        //        // Get assigned job titles
        //        var assignedJobTitles = assignedJobs.Select(j => j.Job.title).ToList();
        //        committeeDetail.AssignedJobTitles = assignedJobTitles;

        //        return Request.CreateResponse(HttpStatusCode.OK, committeeDetail);
        //    }
        //    catch (Exception exp)
        //    {
        //        return Request.CreateResponse(HttpStatusCode.InternalServerError, exp.Message);
        //    }


        //}
    }
}