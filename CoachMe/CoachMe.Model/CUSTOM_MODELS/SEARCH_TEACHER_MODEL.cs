using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace COACHME.MODEL.CUSTOM_MODELS
{
    public class SEARCH_TEACHER_MODEL
    {
        public string COURSE_NAME { get; set; }
        public string PROVINCE { get; set; }
        public string AREA { get; set; }
        public  string TEACH_TYPE { get; set; }
        public List<string> TEACH_LOCATE { get; set; }
        public List<string> TEACH_GENDER { get; set; }
        public List<string> STUDENT_LEVEL { get; set; }
        
        public List<CUSTOM_MEMBERS> LIST_MEMBERS { get; set; }
    }
}
