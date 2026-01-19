using System.ComponentModel.DataAnnotations;

namespace CookiesAuth.Models
{
    public class User
    {
        public int Id { get; set; }

        public string UserName { get; set; } = null!;

        public string Password { get; set; } = null!;

        public string FullName { get; set; } = string.Empty;
    }
}
