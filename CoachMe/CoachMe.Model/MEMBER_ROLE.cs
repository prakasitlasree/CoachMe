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
    
    public partial class MEMBER_ROLE
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public MEMBER_ROLE()
        {
            this.COURSE_COMMENT = new HashSet<COURSE_COMMENT>();
            this.MEMBER_REGIS_COURSE = new HashSet<MEMBER_REGIS_COURSE>();
            this.MEMBER_TEACH_COURSE = new HashSet<MEMBER_TEACH_COURSE>();
        }
    
        public int AUTO_ID { get; set; }
        public int MEMBER_ID { get; set; }
        public int ROLE_ID { get; set; }
        public string CREATED_BY { get; set; }
        public Nullable<System.DateTime> CREATED_DATE { get; set; }
        public string UPDATED_BY { get; set; }
        public Nullable<System.DateTime> UPDATED_DATE { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<COURSE_COMMENT> COURSE_COMMENT { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MEMBER_REGIS_COURSE> MEMBER_REGIS_COURSE { get; set; }
        public virtual MEMBERS MEMBERS { get; set; }
        public virtual ROLE ROLE { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MEMBER_TEACH_COURSE> MEMBER_TEACH_COURSE { get; set; }
    }
}
