using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Newtonsoft.Json;
using StudentManagement.Models;
using System.Data;

namespace StudentManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        public UserController(IConfiguration configuration)
        {
            _configuration = configuration;
        }


        [HttpPost("Registration")]
        public async Task<string> RegistrationAsync(User user)
        {
            string query = "INSERT INTO Users (UserId, Email, UserName, Password, Address, PhoneNumber, UserRole) VALUES (@UserId, @Email, @UserName, @Password, @Address, @PhoneNumber, @UserRole)";
            using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                try
                {
                    var id = Guid.NewGuid().ToString();
                    await connection.ExecuteAsync(query
                        ,
                        new
                        {
                            UserId = id,
                            Email = user.Email,
                            UserName = user.UserName,
                            Password = user.Password, 
                            Address = user.Address,
                            PhoneNumber = user.PhoneNumber,
                            UserRole = user.UserRole
                        }
                    );

                    return JsonConvert.SerializeObject(new UserResponseMessage
                    {
                        Status = "200",
                        Message = "Registration Successful",
                        UserName = user.UserName,
                        Token = id,
                        Role = user.UserRole
                    });
                }
                catch (Exception ex)
                {
                    return JsonConvert.SerializeObject(new UserResponseMessage
                    {
                        Status = "400",
                        Message = $"Registration Failed: {ex.Message}"
                    });
                }
            }
        }
        

        [HttpPost("Login")]
        public string Login(Login user)
        {
            UserResponseMessage responce = new UserResponseMessage();
            SqlDataAdapter da = new SqlDataAdapter("SELECT * FROM Users WHERE Email = '" + user.Email + "' AND Password = '" + user.Password + "'", _configuration.GetConnectionString("DefaultConnection"));

            DataTable dt = new DataTable();

            da.Fill(dt);
            if (dt.Rows.Count > 0)
            {
                responce.Status = "200";
                responce.Message = "Successfully Login";
                responce.Data = JsonConvert.SerializeObject(dt);

                return JsonConvert.SerializeObject(responce);
            }

            else
            {
                responce.Status = "400";
                responce.Message = "Invalid Cratatial";

                return JsonConvert.SerializeObject(responce);
            }
        }
    }
}
