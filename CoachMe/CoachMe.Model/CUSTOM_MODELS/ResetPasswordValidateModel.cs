using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace COACHME.MODEL.CUSTOM_MODELS
{
    public partial class ResetPasswordValidateModel
    {
        public int AUTO_ID { get; set; }
        public string USER_NAME { get; set; }
        public string TOKEN_HASH { get; set; }
        public Nullable<System.DateTime> TOKEN_EXPIRATION { get; set; }
        public Nullable<bool> TOKEN_USED { get; set; }

        //[Required]
        //[StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        //[DataType(DataType.Password)]
        //[Display(Name = "Password")]
        //public string PASSWORD { get; set; }

        //[DataType(DataType.Password)]
        //[Display(Name = "ConfirmPassword")]
        //[Compare("PASSWORD", ErrorMessage = "The password and confirmation password do not match.")]
        //public string CONFIRM_PASSWORD { get; set; }
    }
}
