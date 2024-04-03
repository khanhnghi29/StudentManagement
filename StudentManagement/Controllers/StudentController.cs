using Azure;
using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Newtonsoft.Json;
using StudentManagement.Models;
using System;
using System.Data;
using System.Net.WebSockets;

namespace StudentManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        public StudentController(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        SqlConnection con = new SqlConnection("Data Source = LAPTOP-NV7FSM6L\\KHANHNGHI29; Initial Catalog = LabourHub; Integrated Security = True; Encrypt=False");

        /// <summary>
        /// Lay toan bo thon tin hoc sinh sinh vien
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetStudent")]
        public async Task<string> GetStudentAsync()
        {
            var query = "SELECT * FROM Students";
            using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {

                var students = await connection.QueryAsync<GetStudent>(query);

                if (students.Any())
                {
                    return JsonConvert.SerializeObject(new StudentResponseMessage
                    {
                        Status = "200",
                        Data = JsonConvert.SerializeObject(students)
                    });
                }

                return JsonConvert.SerializeObject(new StudentResponseMessage
                {
                    Status = "404",
                    Data = "No data found"
                });
            }
        }
        

        /// <summary>
        /// Lay thong tin Sinh vien theo id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("GetOneStudent/{id}")]
        public async Task<string> GetOneStudentAsync(string id)
        {
            string query = "SELECT * FROM Students WHERE UserId = @id";
            using(var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                var student = await connection.QuerySingleOrDefaultAsync<GetStudent>(query, new {id});
                if (student != null)
                {
                    return JsonConvert.SerializeObject(new StudentResponseMessage
                    {
                        Status = "200",
                        Data = JsonConvert.SerializeObject(student)
                    });
                }

                return JsonConvert.SerializeObject(new StudentResponseMessage
                {
                    Status = "404",
                    Message = "No student found"
                });
            }
        }
        
        /// <summary>
        /// Them mot sinh vien vao co so du lieu , dauvao la 1 sinh vien 
        /// </summary>
        /// <param name="student"></param>
        /// <returns></returns>
        [HttpPost("InsertStudent")]
        public async Task<string> PostStudentAsync(Student student)
        {
            string query = "INSERT INTO Students (StudentID, FirstName, LastName, Email, PhoneNumber, UserID) VALUES (@StudentID, @FirstName, @LastName, @Email, @PhoneNumber, @UserID)";
            using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {


                var id = Guid.NewGuid().ToString();
                try
                {
                    await connection.ExecuteAsync(query
                        ,
                        new
                        {
                            StudentID = id,
                            FirstName = student.FirstName,
                            LastName = student.LastName,
                            Email = student.Email,
                            PhoneNumber = student.PhoneNumber,
                            UserID = student.UserID
                        }
                    );

                    return JsonConvert.SerializeObject(new StudentResponseMessage
                    {
                        Status = "200",
                        Message = "Successfully Inserted Data"
                    });
                }
                catch (Exception ex)
                {
                    // Handle potential exceptions (e.g., database errors)
                    return JsonConvert.SerializeObject(new StudentResponseMessage
                    {
                        Status = "400",
                        Message = $"Student Not Inserted: {ex.Message}"
                    });
                }



            }
        }
        
        
        /// <summary>
        /// Tim hoc sinh theo Id tu table Student
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("FindUser/{id}")]

        public async Task<string> GetStudentCount(string id)
        {
            StudentResponseMessage response = new StudentResponseMessage();

            SqlDataAdapter adapter = new SqlDataAdapter("SELECT * FROM Students WHERE StudentID = '" + id + "'", con);

            DataTable data = new DataTable();

            adapter.Fill(data);
            List<GetStudent> studentEditList = new List<GetStudent>();

            if (data.Rows.Count > 0)
            {
                response.Data = JsonConvert.SerializeObject(data);
                response.Status = "200";

                return JsonConvert.SerializeObject(response);
            }

            else
            {
                response.Status = "404";

                return JsonConvert.SerializeObject(response);
            }
        }

        /// <summary>
        /// Chinh ssu thong tin cua 1 Student
        /// </summary>
        /// <param name="id"></param>
        /// <param name="student"></param>
        /// <returns></returns>
        [HttpPut("UpdateStudent/{id}")]
        public async Task<string> PutStudentAsync(string id, Student student)
        {
            string query = "UPDATE Students SET FirstName = @FirstName, LastName = @LastName, Email = @Email, PhoneNumber = @PhoneNumber WHERE StudentID = @StudentID";
            using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                    var rowsAffected = await connection.ExecuteAsync(query
                        ,
                        new
                        {
                            StudentID = id,
                            FirstName = student.FirstName,
                            LastName = student.LastName,
                            Email = student.Email,
                            PhoneNumber = student.PhoneNumber
                        }
                    );

                if (rowsAffected > 0)
                {
                    return JsonConvert.SerializeObject(new StudentResponseMessage
                    {
                        Status = "200",
                        Message = "Successfully Updated Data"
                    });
                }
                else
                {
                    return JsonConvert.SerializeObject(new StudentResponseMessage
                    {
                        Status = "400",
                        Message = "Student Not Found or Not Updated"
                    });
                }



            }
        }
        
        /// <summary>
        /// Xoa 1 sinh vien ra khoi co so du lieu theo id, chi duoc thuc hien boi Admin
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("DeleteStudent/{id}")]
        public async Task<string> DeleteStudentAsync(string id)
        {
            string query = "DELETE FROM Students WHERE StudentID = @StudentID";
            using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
           
                var rowsAffected = await connection.ExecuteAsync(query
                    ,
                    new { StudentID = id }
                );

                if (rowsAffected > 0)
                {
                    return JsonConvert.SerializeObject(new StudentResponseMessage
                    {
                        Status = "200",
                        Message = "Student Deleted Successfully"
                    });
                }
                else
                {
                    return JsonConvert.SerializeObject(new StudentResponseMessage
                    {
                        Status = "400",
                        Message = "Student Not Found"
                    });
                }
            }
        }
        
    }
}
