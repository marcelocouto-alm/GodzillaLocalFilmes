namespace GodzillaLocalFilmes.Users
{
    public class User
    {
        public Guid UserId { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Name { get; set; }

        public User(string email, string password, string name)
        {
            UserId = Guid.NewGuid();
            Email = email;
            Password = password;
            Name = name;
        }
    }
}

