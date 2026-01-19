namespace CookiesAuth.Models
{
    public class UserStoreService
    {
        private List<User> _users = new List<User>
        {
            new User {Id = 1 , UserName = "Saurabh" ,Password = "saurabh@123"},
            new User {Id = 2 , UserName = "Harsh" ,Password = "harsh@123"}
        };

        public User? ValidateUser(string userName, string password)
        {
            return _users.FirstOrDefault(u =>
            u.UserName.Equals(userName, StringComparison.OrdinalIgnoreCase) &&
            u.Password == password);
        }

        public User? GetUser(int id)
        {
            return _users.FirstOrDefault(u => u.Id == id);
        }
    }
}
