using System;
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
    }
}
