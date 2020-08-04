using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace TTMLibrary.ModelViews
{
    public class RegistrationModelView : UserModelView
    {
        public RegistrationModelView(string login) : base(login)
        {

        }

        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        [DataType(DataType.Password)]
        [StringLength(50,MinimumLength = 6, ErrorMessage = "Пароль должен содержать от 6 до 50 символов")]
        public string Password { get; set; }
        [Compare("Password", ErrorMessage = "Пароли не совпадают")]
        public string ConfirmPassword { get; set; }

    }
}
