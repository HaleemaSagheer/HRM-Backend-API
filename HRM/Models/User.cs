//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace HRM.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class User
    {
        public int id { get; set; }
        public string name { get; set; }
        public System.DateTime dob { get; set; }
        public string address { get; set; }
        public string mobile_num { get; set; }
        public string email { get; set; }
        public string password { get; set; }
        public string role { get; set; }
        public string image { get; set; }
        public string gender { get; set; }
        public string cnic { get; set; }
        public string CustomActivationStatus { get; internal set; }
    }
}
