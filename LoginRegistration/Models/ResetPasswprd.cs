using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;

namespace LoginRegistration.Models
{
    public class ResetPasswprd
    {
        [Display(Name ="新密碼")]
        [Required(ErrorMessage ="請輸入6~10位數新密碼",AllowEmptyStrings =false)]
        [MinLength(6,ErrorMessage = "請輸入6~10位數新密碼")]
        [MaxLength(10, ErrorMessage = "密碼須為6~10位數")]
        [DataType(DataType.Password)]
        public string NewPWd { get; set; }

        [Display(Name = "確認新密碼")]
        [DataType(DataType.Password)]
        [Compare("NewPWd",ErrorMessage ="與新密碼不符")]
        public string ConfirmPwd { get; set; }
        [Required]
        public string ResetCode { get; set; }
    }
}