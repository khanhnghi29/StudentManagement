﻿namespace StudentManagement.Models
{
    public class UserResponseMessage
    {
        public string Status { get; set; }
        public string Message { get; set; }
        public string UserName { get; set; }

        public string Role { get; set; }
        public string Token { get; set; }

        public string Data { get; set; }
    }
}
