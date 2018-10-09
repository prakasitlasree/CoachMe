using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace COACHME.MODEL.CUSTOM_MODELS
{
    public class CUSTOM_MEMBERS
    {
        public int AUTO_ID { get; set; }
        public string FULLNAME { get; set; }
        public string FIRST_NAME { get; set; }
        public string LAST_NAME { get; set; }
        public string MOBILE { get; set; }
        public Nullable<System.DateTime> DATE_OF_BIRTH { get; set; }
        public string ID_CARD { get; set; }
        public string SEX { get; set; }
        public Nullable<int> AGE { get; set; }
        public string ABOUT { get; set; }
        public string LOCATION { get; set; }
        public string CATEGORY { get; set; }
        public string PROFILE_IMG_URL { get; set; }
        public string CREATED_BY { get; set; }
        public Nullable<System.DateTime> CREATED_DATE { get; set; }
        public string UPDATED_BY { get; set; }
        public Nullable<System.DateTime> UPDATED_DATE { get; set; }
        public string NICKNAME { get; set; }
        public string COURSE { get; set; }
        public string USER_NAME { get; set; }      
        public List<string> LIST_STUDENT_COMMENT { get; set; }
        public string STATUS { get; set; }
        
        public int REGIS_COURSE_ID { get; set; }

    }
}
