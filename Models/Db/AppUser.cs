using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace minesweeper.Models.Db
{
    public class AppUser : IdentityUser
    {
        public IEnumerable<Record> Records { get; set; }
    }
}
