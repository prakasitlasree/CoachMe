//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace COACHME.MODEL
{
    using System;
    using System.Collections.Generic;
    
    public partial class MEMBER_CATEGORY
    {
        public int AUTO_ID { get; set; }
        public int MEMBER_ID { get; set; }
        public Nullable<int> CATEGORY_ID { get; set; }
        public string NAME { get; set; }
        public string CREATED_BY { get; set; }
        public Nullable<System.DateTime> CREATED_DATE { get; set; }
        public string UPDATED_BY { get; set; }
        public Nullable<System.DateTime> UPDATED_DATE { get; set; }
    
        public virtual CATEGORY CATEGORY { get; set; }
        public virtual MEMBERS MEMBERS { get; set; }
    }
}
