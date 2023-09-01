using System.Collections.Generic;

namespace HRM.Controllers
{
    public class ApplicantStatusChangeRequest
    {
            public List<int> UserIds { get; set; }
            public int JobId { get; set; }
            public string Status { get; set; }
        

    }
}