using COACHME.MODEL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace COACHME.CustomModels
{
    public class RegisterModel
    {
        public string FULLNAME { get; set; }

        public string EMAIL { get; set; }

        public string USER_NAME { get; set; }

        public string PASSWORD { get; set; }

        public string CONFIRM_PASSWORD { get; set; }
         
        public string MOBILE { get; set; }
    }
}