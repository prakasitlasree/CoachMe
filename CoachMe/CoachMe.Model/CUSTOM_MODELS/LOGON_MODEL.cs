using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace COACHME.MODEL.CUSTOM_MODELS
{
  public partial class LOGON_MODEL
    {
        [Required]
        [Display(Name = "Email")]
        [EmailAddress]
        public string EMAIL { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string PASSWORD { get; set; }

        [Display(Name = "Remember me?")]
        public bool REMEMBER_ME { get; set; } 
    }
}
