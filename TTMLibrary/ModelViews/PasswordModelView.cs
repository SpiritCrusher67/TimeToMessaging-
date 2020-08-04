using System.ComponentModel.DataAnnotations;

namespace TTMLibrary.ModelViews
{
    public class PasswordModelView : UserModelView
    {
        public PasswordModelView(string login) : base(login)
        {

        }

        [Required]
        [DataType(DataType.Password)]
        public string OldPassword { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [StringLength(50, MinimumLength = 6, ErrorMessage = "Пароль должен содержать от 6 до 50 символов")]
        public string Password { get; set; }
        [Compare("Password", ErrorMessage = "Пароли не совпадают")]
        public string ConfirmPassword { get; set; }
    }
}
