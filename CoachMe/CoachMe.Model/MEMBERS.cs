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
    
    public partial class MEMBERS
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public MEMBERS()
        {
            this.MEMBER_LOGON = new HashSet<MEMBER_LOGON>();
            this.MEMBER_ROLE = new HashSet<MEMBER_ROLE>();
        }
    
        public int AUTO_ID { get; set; }
        public string FULLNAME { get; set; }
        public string FIRST_NAME { get; set; }
        public string LAST_NAME { get; set; }
        public string MOBILE { get; set; }
        public Nullable<System.DateTime> DATE_OF_BIRTH { get; set; }
        public Nullable<int> AGE { get; set; }
        public string ABOUT { get; set; }
        public string PROFILE_IMG_URL { get; set; }
        public string ABOUT_IMG_URL1 { get; set; }
        public string ABOUT_IMG_URL2 { get; set; }
        public string ABOUT_IMG_URL3 { get; set; }
        public string ABOUT_IMG_URL4 { get; set; }
        public string ABOUT_IMG_URL5 { get; set; }
        public string CREATED_BY { get; set; }
        public Nullable<System.DateTime> CREATED_DATE { get; set; }
        public string UPDATED_BY { get; set; }
        public Nullable<System.DateTime> UPDATED_DATE { get; set; }
        public string NICKNAME { get; set; }
        public string ID_CARD { get; set; }
        public string SEX { get; set; }
        public string LOCATION { get; set; }
        public string CATEGORY { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MEMBER_LOGON> MEMBER_LOGON { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MEMBER_ROLE> MEMBER_ROLE { get; set; }
    }
}
