﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace COACHME.MODEL.CUSTOM_MODELS
{
    public class CONTAINER_MODEL
    {
        public CONFIGURATION CONFIGURATION { get; set; }
        public COURSES COURSES { get; set; }
        public MEMBERS MEMBERS { get; set; }
        public MEMBER_TEACH_COURSE MEMBER_TEACH_COURSE { get; set; }
        public MEMBER_LOGON MEMBER_LOGON { get; set; }
        public MEMBER_REGIS_COURSE MEMBER_REGIS_COURSE { get; set; }
        public MEMBER_ROLE MEMBER_ROLE { get; set; }
        public PLACE PLACE { get; set; }
        public RESET_PASSWORD RESET_PASSWORD { get; set; }
        public ROLE ROLE { get; set; }
        public COURSE_PLACE COURSE_PLACE { get; set; }
        public List<CONFIGURATION> LIST_CONFIGURATION { get; set; }
        public List<COURSES> LIST_COURSES { get; set; }
        public List<MEMBERS> LIST_MEMBERS { get; set; }
        public List<MEMBER_TEACH_COURSE> LIST_MEMBER_TEACH_COURSE { get; set; }
        public List<MEMBER_LOGON> LIST_MEMBER_LOGON { get; set; }
        public List<MEMBER_REGIS_COURSE> LIST_MEMBER_REGIS_COURSE { get; set; }
        public List<MEMBER_ROLE> LIST_MEMBER_ROLE { get; set; }
        public List<PLACE> LIST_PLACE { get; set; }
        public List<RESET_PASSWORD> LIST_RESET_PASSWORD { get; set; }
        public List<ROLE> LIST_ROLE { get; set; }
        public List<COURSE_PLACE> LIST_COURSE_PLACE { get; set; }

        public List<CUSTOM_MEMBERS> LIST_CUSTOM_MEMBERS { get; set; }
    }
}
