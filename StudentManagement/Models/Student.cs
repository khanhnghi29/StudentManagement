namespace StudentManagement.Models
{
    public class Student
    {
        public Guid StudentID { get; set; } = Guid.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string UserID { get; set; } = string.Empty;
    }
}
