using COACHME.MODEL;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace COACHME.MODEL.CUSTOM_MODELS
{
    public partial class REGISTER_MODEL
    {
        [Required]
        [EmailAddress]
        [Display(Name = "EMAIL")]
        public string EMAIL { get; set; }

        [Required]
        [Display(Name = "FULLNAME")]
        public string FULLNAME { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "PASSWORD")]
        public string PASSWORD { get; set; }

        [Required]
        //[StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        //[DataType(DataType.Password)]
        //[Display(Name = "PASSWORD")]
        public string GENDER { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "CONFIRM_PASSWORD")]
        [Compare("PASSWORD", ErrorMessage = "The password and confirmation password do not match.")]
        public string CONFIRM_PASSWORD { get; set; }

        [Required]
        [Display(Name = "AGREE")]
        [Range(typeof(bool), "true", "true", ErrorMessage = "Please read and accept the Terms of use.")]
        public bool AGREE { get; set; }

        [Required]
        public int ROLE { get; set; }

        //[Required]
        //public DateTime DATE_OF_BIRTH { get; set; }

    }
}