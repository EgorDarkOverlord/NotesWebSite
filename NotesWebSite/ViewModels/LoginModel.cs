using System.ComponentModel.DataAnnotations;

namespace NotesWebSite.ViewModels
{
    public class LoginModel
    {
        [Required (ErrorMessage = "Не указан логин")]
        public string Login { get; set; }
        
        [Required (ErrorMessage = "Не указан пароль")]
        public string Password { get; set; }
    }
}
