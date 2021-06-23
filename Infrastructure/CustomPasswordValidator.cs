using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using minesweeper.Models.Db;
using Microsoft.AspNetCore.Identity;

namespace minesweeper.Infrastructure
{
    public class CustomPasswordValidator : IPasswordValidator<AppUser>
    {
        public Task<IdentityResult> ValidateAsync(UserManager<AppUser> manager, AppUser user, string password)
        {
            List<IdentityError> errors = new List<IdentityError>();
            if (password.Length < 6)
            {
                errors.Add(new IdentityError
                {
                    Code = "PasswordLengthToLow",
                    Description = "Пароль не может быть меньше 6 символов"
                });
            }
            return Task.FromResult(errors.Count == 0 ? IdentityResult.Success : IdentityResult.Failed(errors.ToArray()));
        }
    }
}
