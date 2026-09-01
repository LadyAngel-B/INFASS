namespace INFASS.Models
{
    public class User
    {
        public int Id { get; set; }

        public string FullName { get; set; }

        public string Email { get; set; }

        public string Password { get; set; }

        public string ConfirmPassword { get; set; }

        public User()
        {
        }

        public User(
            string fullName,
            string email,
            string password)
        {
            FullName = fullName;
            Email = email;
            Password = password;
        }
    }
}