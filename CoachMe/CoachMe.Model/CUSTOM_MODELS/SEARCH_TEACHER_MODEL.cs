using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace COACHME.MODEL.CUSTOM_MODELS
{
    public class SEARCH_TEACHER_MODEL
    {

        public MEMBERS MEMBERS { get; set; }
        public string PROVINCE { get; set; }
        public string AREA { get; set; }
        public  string TEACH_TYPE { get; set; }
        public string LOCATION { get; set; }
        public List<string> TEACH_LOCATE { get; set; }
        public List<int?> TEACHING_TYPE { get; set; }
        public List<string> TEACH_GENDER { get; set; }
        public List<int?> STUDENT_LEVEL { get; set; }
        public List<COURSES> LIST_COURSE { get; set; }
        public string  SELECTED_COURSE { get; set; }
        public List<CATEGORY> LIST_CATEGORY { get; set; }
        public string SELECTED_CATEGORY { get; set; }

        public List<string> LIST_SEARCH_TYPE { get; set; }
        public List<string> LIST_PROVINCE { get; set; }
        public List<int?> LIST_COURSE_ID { get; set; }
        public string SEARCH_ALL { get; set; }
        public List<int?> LIST_PROVINCE_ID { get; set; }
        public List<int?> LIST_AMPHUR_ID { get; set; }
        public List<int> LIST_MEMBER_CETEGORY { get; set; }
        public List<int> LIST_MEMBER_TEACH_COURSE { get; set; }

        public int TEACHER_ROLE_ID { get; set; }
        public int STUDENT_ROLE_ID { get; set; }

        #region ============= PAGING =============
        public int PAGE_NUMBER { get; set; }
        public int NEXT { get; set; }
        public int PREVIOS { get; set; }
        public int PAGE_COUNT { get; set; }
        #endregion


        #region ======= course model =======

        public int COURSE_ID { get; set; }
        public string COURSE_NAME { get; set; }
        public int MEMBER_ROLE_ID { get; set; }
     
        #endregion


    }
}
