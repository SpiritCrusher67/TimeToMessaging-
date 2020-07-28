using System.ComponentModel.DataAnnotations;

namespace TTMLibrary.ModelViews
{
    public class EmailModelView
    {
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [Required]
        [EmailAddress]
        public string NewEmail { get; set; }
    }
}
