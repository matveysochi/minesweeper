using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace minesweeper.Models.Db
{
    public class SignModel
    {
        [Required(ErrorMessage = "Требуется ввести имя пользователя")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Требуется ввести email")]
        [UIHint("email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Требуется ввести пароль")]
        [UIHint("password")]
        public string Password { get; set; }
    }
    public class LoginModel
    {
        [Required(ErrorMessage = "Требуется ввести email")]
        [UIHint("email")]
        public string Email { get; set; }
        
        [Required(ErrorMessage = "Требуется ввести пароль")]
        [UIHint("password")]
        public string Password { get; set; }
    }
}
