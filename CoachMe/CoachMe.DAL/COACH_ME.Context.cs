﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace COACHME.DAL
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    using COACHME.MODEL;
    
    public partial class COACH_MEEntities : DbContext
    {
        public COACH_MEEntities()
            : base("name=COACH_MEEntities")
        {
    	this.Configuration.ProxyCreationEnabled = false;
    
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<AMPHUR> AMPHUR { get; set; }
        public virtual DbSet<CATEGORY> CATEGORY { get; set; }
        public virtual DbSet<CONFIGURATION> CONFIGURATION { get; set; }
        public virtual DbSet<COURSE_COMMENT> COURSE_COMMENT { get; set; }
        public virtual DbSet<COURSE_PLACE> COURSE_PLACE { get; set; }
        public virtual DbSet<COURSES> COURSES { get; set; }
        public virtual DbSet<DISTRICT> DISTRICT { get; set; }
        public virtual DbSet<GEOGRAPHY> GEOGRAPHY { get; set; }
        public virtual DbSet<LOGON_ACTIVITY> LOGON_ACTIVITY { get; set; }
        public virtual DbSet<MEMBER_CATEGORY> MEMBER_CATEGORY { get; set; }
        public virtual DbSet<MEMBER_LOGON> MEMBER_LOGON { get; set; }
        public virtual DbSet<MEMBER_MATCHING> MEMBER_MATCHING { get; set; }
        public virtual DbSet<MEMBER_PACKAGE> MEMBER_PACKAGE { get; set; }
        public virtual DbSet<MEMBER_REGIS_COURSE> MEMBER_REGIS_COURSE { get; set; }
        public virtual DbSet<MEMBER_ROLE> MEMBER_ROLE { get; set; }
        public virtual DbSet<MEMBER_TEACH_COURSE> MEMBER_TEACH_COURSE { get; set; }
        public virtual DbSet<MEMBERS> MEMBERS { get; set; }
        public virtual DbSet<PLACE> PLACE { get; set; }
        public virtual DbSet<PROVINCE> PROVINCE { get; set; }
        public virtual DbSet<RESET_PASSWORD> RESET_PASSWORD { get; set; }
        public virtual DbSet<ROLE> ROLE { get; set; }
        public virtual DbSet<SURVEYS> SURVEYS { get; set; }
    }
}
