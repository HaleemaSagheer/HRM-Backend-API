using HRM.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HRM.Controllers
{
    public class Utility
    {
      //
        public void AssignApplicantsForShortlisting(HRMEntities2 db)
        {
            try
            {
                var clist = db.Committees.ToList();
            
            foreach (var c in clist)
            {
                    var allUnAssigned = db.Applies.Where(a => a.member_id == null);

                    if (db.CommitteeJobs.Where(a => a.committee_id == c.id).Count() == 0)
                        continue;
                var members = db.CommitteeMembers.Where(m => m.committee_id == c.id && m.is_activated==true).ToList();
                    if (members.Count() == 0)
                        continue;
                var unassigned = allUnAssigned.Join(db.CommitteeJobs.Where(a => a.committee_id == c.id), b => b.job_id, d => d.job_id, (b, d) => b);
                var total = unassigned.Count();
                var memCount = members.Count()+1;
                var remainder = total % memCount;
                var individualCount = total / memCount;
                
                    var selectedUnassigned = unassigned.Where(a => a.member_id == null).Take(individualCount);
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

                        unassigned = allUnAssigned.Join(db.CommitteeJobs.Where(a => a.committee_id == c.id), b => b.job_id, d => d.job_id, (b, d) => b);

                        selectedUnassigned = unassigned.Where(a=>a.member_id==null).Take(individualCount);
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

                        unassigned = allUnAssigned.Join(db.CommitteeJobs.Where(a => a.committee_id == c.id), b => b.job_id, d => d.job_id, (b, d) => b);

                        var firstmem = members.FirstOrDefault();
                    if (firstmem != null)
                    {
                            selectedUnassigned = unassigned.Where(a => a.member_id == null).Take(remainder);

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