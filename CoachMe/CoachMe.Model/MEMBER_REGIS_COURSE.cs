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
    
    public partial class MEMBER_REGIS_COURSE
    {
        public int AUTO_ID { get; set; }
        public Nullable<int> MEMBER_ROLE_ID { get; set; }
        public Nullable<int> COURSE_ID { get; set; }
        public string DESCRIPTION { get; set; }
        public string CREATED_BY { get; set; }
        public Nullable<System.DateTime> CREATED_DATE { get; set; }
        public string UPDATED_BY { get; set; }
        public Nullable<System.DateTime> UPDATED_DATE { get; set; }
    
        public virtual COURSES COURSES { get; set; }
    }
}
