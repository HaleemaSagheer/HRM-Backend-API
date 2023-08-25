using System.Collections.Generic;

namespace HRM.Controllers
{
    public class CommitteeMembersRequest
    {
        public int committee_id { get; set; }
        public List<int> user_ids { get; set; }
    }
}