using LMSProjectCode.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LMSProjectCode.Controllers
{
    public class AdminController : Controller
    {
        private readonly string connectionString = ConfigurationManager.ConnectionStrings["DB_WeighBridge"].ConnectionString;

        // GET: Admin
        public ActionResult ShowUserInfo()
        {

            List<IssuedBookModel> bookCollection = new List<IssuedBookModel>();
            string query = "Select * from IssuedBooksInformation";
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {

                    SqlCommand command = new SqlCommand(query, connection);

                    connection.Open();

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            bookCollection.Add(new IssuedBookModel
                            {
                                StudentName = reader["StudentName"].ToString(),
                                studentId = Convert.ToInt32(reader["StudentId"]),
                                BookId = Convert.ToInt32(reader["BookId"]),
                                BookName = reader["BookName"].ToString(),
                                AuthorName = reader["AuthorName"].ToString(),
                                Id = Convert.ToInt32(reader["IssuedID"]),
                                IssuedDate = reader["Date"].ToString(),
                                NumberOdDays = reader["NumberofDays"].ToString(),
                                Status = reader["status"].ToString(),
                                MobileNumber = Convert.ToInt32(reader["StudentMobileNUmber"])
                            });
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return View(bookCollection);
        }
    }
}