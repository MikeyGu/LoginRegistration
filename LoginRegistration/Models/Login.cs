using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace LoginRegistration.Models
{
    public class Login
    {
        [Display(Name = "帳號:")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "帳號不得為空")]
        public string account { get; set; }
        [Display(Name = "密碼:")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "密碼不得為空")]
        [DataType(DataType.Password)]
        public string userPwd { get; set; }
        [Display(Name = "記住我")]
        public bool rememberMe { get; set; }
    }
}