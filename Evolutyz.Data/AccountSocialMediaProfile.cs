//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Evolutyz.Data
{
    using System;
    using System.Collections.Generic;
    
    public partial class AccountSocialMediaProfile
    {
        public int Acc_SocialMediaProfileId { get; set; }
        public Nullable<int> Asmp_AccountId { get; set; }
        public Nullable<int> Asmp_SocialMediaId { get; set; }
        public string Url { get; set; }
        public Nullable<System.DateTime> Asmp_CreatedDate { get; set; }
        public Nullable<System.DateTime> Asmp_ModifiedDate { get; set; }
    }
}
