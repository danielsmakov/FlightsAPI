namespace FlightsAPI.Models.Entities
{
    public class User
    {
        public int Id { get; private set; }
        public string Username { get; set; }
        public string PasswordHash { get; set; }
        public int RoleId { get; set; }
        public Role Role { get; set; }
    }
}
