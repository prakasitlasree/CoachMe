using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace COACHME.MODEL.CUSTOM_MODELS
{
   public partial class FORGOT_PASSWORD_MODEL
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string EMAIL { get; set; }
    }
}
