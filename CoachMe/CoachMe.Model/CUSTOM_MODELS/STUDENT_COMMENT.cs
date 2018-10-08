using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace COACHME.MODEL.CUSTOM_MODELS
{
    public class STUDENT_COMMENT
    {
        public int AUTO_ID { get; set; }
        public Nullable<int> MEMBER_REGIS_COURSE_ID { get; set; }
        public string COMMENT { get; set; }
        public string COMMENT_BY { get; set; }
        public Nullable<System.DateTime> COMMENT_DATE { get; set; }

        public virtual MEMBER_REGIS_COURSE MEMBER_REGIS_COURSE { get; set; }


    }
}
