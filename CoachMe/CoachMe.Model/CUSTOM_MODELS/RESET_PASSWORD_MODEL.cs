using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace COACHME.MODEL.CUSTOM_MODELS
{
    public partial class RESET_PASSWORD_MODEL
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string EMAIL { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string NEW_PASSWORD { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "ConfirmPassword")]
        [Compare("NEW_PASSWORD", ErrorMessage = "The password and confirmation password do not match.")]
        public string CONFIRM_NEW_PASSWORD { get; set; }

        public string TOKEN_HASH { get; set; }
    }
}
