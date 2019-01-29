using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace COACHME.MODEL.CUSTOM_MODELS
{
    public class HOME_MODEL
    {

        #region ============= PAGING =============
        public int PAGE_NUMBER { get; set; }
        public int NEXT { get; set; }
        public int PREVIOS { get; set; }
        public decimal PAGE_COUNT { get; set; }
        public int PAGE_SIZE = 10;
        #endregion

        #region ============= SEARCH OPTION =======
        public int SEARCH_TYPE { get; set; }
        public int CATEGORY_ID { get; set; }
        public int PROVINCE_ID { get; set; }
        public int AMPHUR_ID { get; set; }
        public string ID_NAME_TEACHER { get; set; }
        public string ID_ABOUT_TEACHER { get; set; }
        #endregion

        #region ============= SELECT OPTION =======
        public int MEMBER_AUTO_ID { get; set; }
        public int TEACHER_AUTO_ID { get; set; }
        public int COURSE_ID { get; set; }
        #endregion


        public MEMBER MEMBER { get; set; }
        public List<LIST_MEMBERS> LIST_MEMBERS { get; set; }
    }

    public class LIST_MEMBERS
    {
        public int AUTO_ID { get; set; }
        public string FULLNAME { get; set; }
        public string FIRST_NAME { get; set; }
        public string LAST_NAME { get; set; }
        public string MOBILE { get; set; }
        public int? AMPHUR_ID { get; set; }
        public Nullable<System.DateTime> DATE_OF_BIRTH { get; set; }
        public string DATE_OF_BIRTH_TEXT { get; set; }
        public string SEX_RADIO { get; set; }
        public string ID_CARD { get; set; }
        public string ACCEPT_BY { get; set; }
        public int? TEACHING_TYPE { get; set; }
        public string TEACHING_TYPE_NAME { get; set; }
        public int? STUDENT_LEVEL { get; set; }
        public string STUDENT_LEVEL_NAME { get; set; }

        public bool VERIFY { get; set; }
        public string SEX { get; set; }
        public Nullable<int> AGE { get; set; }
        public string ABOUT { get; set; }
        public string ABOUT_IMG_1 { get; set; }
        public string ABOUT_IMG_2 { get; set; }
        public string ABOUT_IMG_3 { get; set; }
        public string ABOUT_IMG_4 { get; set; }
        public string LOCATION { get; set; }
        public string CATEGORY { get; set; }
        public string PROFILE_IMG_URL { get; set; }
        public string PROFILE_IMG_URL_FULL { get; set; }
        public string CREATED_BY { get; set; }
        public Nullable<System.DateTime> CREATED_DATE { get; set; }
        public string UPDATED_BY { get; set; }
        public Nullable<System.DateTime> UPDATED_DATE { get; set; }
        public string NICKNAME { get; set; }
        public string COURSE { get; set; }
        public int? COURSE_ID { get; set; }
        public int MEMBER_ROLE_ID { get; set; }
        public string USER_NAME { get; set; }
        public string STATUS { get; set; }
        public bool REGISTER_STATUS { get; set; }
        public int REGIS_COURSE_ID { get; set; }
        public string COURSE_BANNER { get; set; }
        public string ROLE { get; set; }
        public string LINE_ID { get; set; }
        public string FACEBOOK_URL { get; set; }

        public List<DATA_MEMBER_CATEGORY> LIST_MEMBER_CETEGORY { get; set; }
        public List<string> LIST_STUDENT_COMMENT { get; set; }
    }

    public class MEMBER
    {
        public string PROFILE_IMG_URL { get; set; }
        public string FULLNAME { get; set; }
        public string USER_NAME { get; set; }
        public string FIRST_NAME { get; set; }
        public string LAST_NAME { get; set; }
        public int ROLE_ID { get; set; }
        public string GENDER { get; set; }
        public string EMAIL { get; set; }

    }
    public class DATA_MEMBER_CATEGORY
    {
        public int AUTO_ID { get; set; }
        public int CATEGORY_ID { get; set; }
        public string NAME { get; set; }
    }

}
