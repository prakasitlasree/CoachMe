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
        public int REGISTER_ID { get; set; }
        public Nullable<int> TEACH_COURSE_ID { get; set; }
        public string DESCRIPTION { get; set; }
        public string STATUS { get; set; }
        public Nullable<System.DateTime> START_DATE { get; set; }
        public Nullable<System.DateTime> END_DATE { get; set; }
        public string CREATED_BY { get; set; }
        public Nullable<System.DateTime> CREATED_DATE { get; set; }
        public string UPDATED_BY { get; set; }
        public Nullable<System.DateTime> UPDATED_DATE { get; set; }
    
        public virtual MEMBER_ROLE MEMBER_ROLE { get; set; }
        public virtual MEMBER_TEACH_COURSE MEMBER_TEACH_COURSE { get; set; }
    }
}
