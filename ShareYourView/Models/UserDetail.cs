//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ShareYourView.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class UserDetail
    {
        public int user_ID { get; set; }
        public string user_FName { get; set; }
        public string user_LName { get; set; }
        public string user_Email { get; set; }
        public string user_Username { get; set; }
        public string user_Password { get; set; }
        public bool user_IsVerified { get; set; }
        public System.Guid user_ActivationCode { get; set; }
    }
}
