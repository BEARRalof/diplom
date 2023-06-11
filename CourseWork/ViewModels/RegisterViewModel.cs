using System.ComponentModel.DataAnnotations;

namespace CourseWork.ViewModels
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Поле не может быть пустым.")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Поле не может быть пустым.")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Required(ErrorMessage = "Поле не может быть пустым.")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required(ErrorMessage = "Поле не может быть пустым.")]
        [DataType(DataType.Password)]
        [Compare("Password")]
        public string PasswordConfirm { get; set; }
    }
}
