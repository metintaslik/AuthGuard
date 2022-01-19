namespace AuthGuard.API.Models.DTOs
{
    public class UserDto
    {
        public int ID { get; set; }
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public int Gender { get; set; }
        public int Age { get; set; }
        public bool IsActive { get; set; }
    }
}