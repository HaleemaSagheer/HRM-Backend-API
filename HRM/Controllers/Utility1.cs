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
                // Select the Scrutiny committee
                var scrutinyCommittee = db.Committees.FirstOrDefault(c => c.committee_type == "Scrutiny");

                if (scrutinyCommittee != null)
                {
                    var scrutinyMembers = db.CommitteeMembers
                        .Where(m => m.committee_id == scrutinyCommittee.id && m.is_activated == true)
                        .ToList();

                    var allUnAssigned = db.Applies.Where(a => a.member_id == null).ToList();
                    var totalCVs = allUnAssigned.Count;
                    var memberCount = scrutinyMembers.Count;

                    if (memberCount > 0 && totalCVs > 0)
                    {
                        var cvPerMember = totalCVs / memberCount;
                        var remainder = totalCVs % memberCount;
                        var memberIndex = 0;

                        foreach (var cv in allUnAssigned)
                        {
                            var member = scrutinyMembers[memberIndex];
                            cv.member_id = member.user_id;

                            // Move to the next member, and loop back to the first if needed
                            memberIndex = (memberIndex + 1) % memberCount;

                            if (remainder > 0)
                            {
                                // Distribute one remainder CV to each member
                                cv.member_id = member.user_id;
                                remainder--;
                            }
                        }

                        db.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }
    }
}