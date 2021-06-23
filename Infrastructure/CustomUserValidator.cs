using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using minesweeper.Models.Db;
using Microsoft.AspNetCore.Identity;

namespace minesweeper.Infrastructure
{
    public class CustomUserValidator : IUserValidator<AppUser>
    {
        public Task<IdentityResult> ValidateAsync(UserManager<AppUser> manager, AppUser user)
        {
            string abc = "abcdefghijklmnopqrstuvwxyz";
            string abv = "абвгдеёжзийклмнопрстуфхцчшщьыъэюя";
            string AllowedCharacters = abc + abc.ToUpper() + abv + abv.ToUpper();

            List<IdentityError> errors = new List<IdentityError>();

            if (!user.UserName.All(c => AllowedCharacters.Contains(c)))
            {
                errors.Add(new IdentityError
                {
                    Code = "NotAllowedCharacters",
                    Description = "Имя пользователя может содержать только буквы"
                });
            };

            if (!manager.Users.All(u => u.Email.ToLower() != user.Email.ToLower()))
            {
                errors.Add(new IdentityError
                {
                    Code = "NotUniqueEmail",
                    Description = "Такой email уже зарегестрирован"
                });
            }


            return Task.FromResult(errors.Count() == 0 ? IdentityResult.Success : IdentityResult.Failed(errors.ToArray()));
        }
    }
}
