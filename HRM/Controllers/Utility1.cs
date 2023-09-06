using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using HRM.Models;

namespace HRM.Controllers
{
    public class Utility1
    {
        public void AssignApplicantsForShortlisting(HRMEntities2 db)
        {
            try
            {
                var clist = db.Committees.Where(x=>x.committee_type== "Scrutiny").ToList();

                foreach (var c in clist)
                {
                    var allUnAssigned = db.Applies.Where(a => a.member_id == null);

                    if (db.CommitteeJobs.Where(a => a.committee_id == c.id).Count() == 0)
                        continue;
                    var members = db.CommitteeMembers.Where(m => m.committee_id == c.id && m.is_activated == true).ToList();
                    if (members.Count() == 0)
                        continue;
                   
                    var total = allUnAssigned.Count();
                    var memCount = members.Count() + 1;
                    var remainder = total % memCount;
                    var individualCount = total / memCount;

                    var selectedUnassigned = allUnAssigned.Where(a => a.member_id == null).Take(individualCount);
                    foreach (var ap in selectedUnassigned)
                    {
                        ap.member_id = c.user_id;
                        var rec = db.Applies.First(x => x.job_id == ap.job_id && x.user_id == ap.user_id);
                        rec.member_id = c.user_id;
                    }

                    db.SaveChanges();


                    foreach (var m in members)
                    {
                        allUnAssigned = db.Applies.Where(a => a.member_id == null);

                       // unassigned = allUnAssigned.Join(db.CommitteeJobs.Where(a => a.committee_id == c.id), b => b.job_id, d => d.job_id, (b, d) => b);

                        selectedUnassigned = allUnAssigned.Where(a => a.member_id == null).Take(individualCount);
                        foreach (var ap in selectedUnassigned)
                        {
                            ap.member_id = m.user_id;
                            var rec = db.Applies.First(x => x.job_id == ap.job_id && x.user_id == ap.user_id);
                            rec.member_id = m.user_id;
                        }

                        db.SaveChanges();

                    }

                    if (remainder != 0)
                    {
                        allUnAssigned = db.Applies.Where(a => a.member_id == null);

                        //unassigned = allUnAssigned.Join(db.CommitteeJobs.Where(a => a.committee_id == c.id), b => b.job_id, d => d.job_id, (b, d) => b);

                        var firstmem = members.FirstOrDefault();
                        if (firstmem != null)
                        {
                            selectedUnassigned = allUnAssigned.Where(a => a.member_id == null).Take(remainder);

                            foreach (var ap in selectedUnassigned)
                            {
                                ap.member_id = firstmem.user_id;
                                var rec = db.Applies.First(x => x.job_id == ap.job_id && x.user_id == ap.user_id);
                                rec.member_id = firstmem.user_id;
                            }

                        }
                    }
                }
                db.SaveChanges();
            }
            catch (Exception ex)
            {

            }
        }
    }
}