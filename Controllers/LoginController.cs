using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq.Expressions;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Web.Services.Description;

namespace LMSProjectCode.Controllers
{
    public class LoginController : Controller
    {
        //private readonly string connectionString = "Data Source=DESKTOP-7PD7Q5Q\\SQLEXPRESS;Initial Catalog=Login_DBS;User Id=User1;Password=Suraj;";

        private readonly string connectionString = ConfigurationManager.ConnectionStrings["DB_WeighBridge"].ConnectionString;

        [HttpGet]
        public ActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Register(string Name, string password, string email, string MobileNumber)
        {
            if (string.IsNullOrEmpty(Name) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(email) || string.IsNullOrEmpty(MobileNumber))
            {
                List<string> blankFields = new List<string>();

                if (string.IsNullOrEmpty(Name))
                    blankFields.Add("User Name");

                if (string.IsNullOrEmpty(password))
                    blankFields.Add("Password");

                if (string.IsNullOrEmpty(email))
                    blankFields.Add("Email");

                if (string.IsNullOrEmpty(MobileNumber))
                    blankFields.Add("MobileNumber");

                return Json(new { success = false, blankFields = blankFields });
            }
              using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string query = "INSERT INTO Users (Username, Password, Email, Mobile , Status) VALUES (@Username, @Password, @Email, @Mobile,@Status)";
                    try
                    {
                        using (var command = new SqlCommand(query, connection))
                        {
                            command.Parameters.AddWithValue("@Username", Name);
                            command.Parameters.AddWithValue("@Password", password);
                            command.Parameters.AddWithValue("@Email", email);
                            command.Parameters.AddWithValue("@Mobile", MobileNumber);
                            command.Parameters.AddWithValue("@Status", "User");

                            command.ExecuteNonQuery();
                        }
                        return Json(new { success = true });
                    }
                    catch (Exception ex)
                    {

                    }
                    return View();
                }
            
        }

        // GET: Login
        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Login(string username, string password)
        {
            
            bool isValidUser = ValidateUser(username, password);
            bool isAdmin = false;

            if (isValidUser)
            {
                // Check if user is admin
                isAdmin = ValidateStatus(username, password);

                // Redirect to Admin view if user is an admin
                if (isAdmin)
                {
                    return Json(new { success = true, redirectTo = Url.Action("ShowBookInformation", "Home") });
                }
                // Redirect to User view if user is not an admin
                else
                {
                    return Json(new { success = true, redirectTo = Url.Action("UserMethod", "Home") });
                }
            }
            else
            {
                // Handle invalid username or password
                return Json(new { success = false, message = "Invalid username or password" });
            }
        }

        public bool ValidateUser(string username, string password)
        {
            bool isValid = false;
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query = "SELECT COUNT(*) FROM Users WHERE Username = @Username AND Password = @Password";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Username", username);
                        command.Parameters.AddWithValue("@Password", password);

                        connection.Open();
                        int count = (int)command.ExecuteScalar();
                        isValid = (count > 0);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return isValid;
        }
        public bool ValidateStatus(string username, string password)
        {
            bool isValid = false;
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query = "SELECT status,Id,Username FROM Users WHERE Username = @Username AND Password = @Password";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Username", username);
                        command.Parameters.AddWithValue("@Password", password);

                        connection.Open();
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                Session["Username"]= reader["Username"].ToString();
                                Session["StudentId"] = reader["Id"].ToString();
                                string status = reader["Status"].ToString();
                                if (status == "Admin")
                                {
                                    isValid = true;
                                }
                                else
                                {
                                    isValid = false;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return isValid;
        }

    }
}