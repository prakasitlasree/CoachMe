using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace COACHME.MODEL.CUSTOM_MODELS
{
    public partial class ResetPasswordValidateModel
    {
        public int AUTO_ID { get; set; }
        public string USER_NAME { get; set; }
        public string TOKEN_HASH { get; set; }
        public Nullable<System.DateTime> TOKEN_EXPIRATION { get; set; }
        public Nullable<bool> TOKEN_USED { get; set; }
        public string PASSWORD { get; set; }

    }
}
