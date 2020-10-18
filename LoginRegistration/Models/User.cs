using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace LoginRegistration.Models
{
    
    public partial class User
    {
        public string ConfirmPassword { get; set; }

    }
    public class UserMetadata
    {
        [Display(Name = "帳號:")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "帳號不得為空")]
        public string UserAccount { get; set; }
        [Display(Name = "生日:")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(ApplyFormatInEditMode =true,DataFormatString ="{0:MM/dd/yyyy}")]
        public string Birthday { get; set; }

        [Display(Name = "名:")]
        [Required(AllowEmptyStrings =false,ErrorMessage ="名不得為空")]
        public string FirstName { get; set; }

        [Display(Name = "姓:")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "名不得為空")]
        public string LastName { get; set; }
        [Display(Name = "EMAIL:")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Email不得為空")]
        public string Email { get; set; }

        [Display(Name = "密碼:")]
        [DataType(DataType.Password)]
        [MinLength(6,ErrorMessage ="密碼須為6~10位數")]
        [MaxLength(10,ErrorMessage = "密碼須為6~10位數")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "密碼不得為空")]
        public string Password { get; set; }

        [Display(Name = "確認密碼:")]
        [DataType(DataType.Password)]
        [Compare("Password",ErrorMessage ="與密碼不符合請重新輸入")]
        public string ConfirmPassword { get; set; }
        public string ActionCode { get; set; }
    }
}