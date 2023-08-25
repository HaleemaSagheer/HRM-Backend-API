using HRM.Models;
using System.Collections.Generic;

namespace HRM.Controllers
{
    internal class CommitteeDetail
    {
        public CommitteeDetail()
        {
            // Constructor initialization
        }

        //public Committee CommitteeInfo { get; set; }
        //public User CommitteeHead { get; set; }
        //public List<User> CommitteeMembers { get; set; }
        ////public CommitteeJob AssignedJob { get; set; }
        ////public List<CommitteeJob> AssignedJobs { get; set; }
        //public List<string> AssignedJobs { get; set; } 
        //public List<string> AssignedJobTitles { get; set; }
        public Committee CommitteeInfo { get; set; }
        public User CommitteeHead { get; set; }
        public List<User> CommitteeMembers { get; set; }
        public List<string> AssignedJobTitles { get; set; }
    }
}
