using LMSProjectCode.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;

namespace LMSProjectCode.Controllers
{
    public class UserController : Controller
    {
        private readonly string connectionString = ConfigurationManager.ConnectionStrings["DB_WeighBridge"].ConnectionString;

        // GET: User
        public ActionResult IssuedBookInfo()
        {

            List<IssuedBookModel> bookCollection = new List<IssuedBookModel>();

            string id = Session["StudentId"] as string;
            int.TryParse(id, out int StudentID);

            string query = "Select IssuedID,BookId,BookName,Date,NumberofDays,AuthorName,Status from IssuedBooksInformation WHERE StudentId = @StudentId";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {

                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@StudentId", StudentID);

                connection.Open();

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        bookCollection.Add(new IssuedBookModel
                        {
                            BookId = Convert.ToInt32(reader["BookId"]),
                            BookName = reader["BookName"].ToString(),
                            AuthorName = reader["AuthorName"].ToString(),
                            Id = Convert.ToInt32(reader["IssuedID"]),
                            IssuedDate = reader["Date"].ToString(),
                            NumberOdDays = reader["NumberofDays"].ToString(),
                            Status = reader["status"].ToString(),
                        });
                    }
                }
            }

            return View(bookCollection);
        }


        [HttpPost]
        public JsonResult ReturnBook(int Id, int BookId)
        {
            bool isreturn = BookReturn(Id);
            if (isreturn)
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand("UPDATE BookCollection SET Quantity = Quantity + 1 WHERE BookId = @BookId AND Quantity > 0", connection))
                    {
                        command.Parameters.AddWithValue("@BookId", BookId);

                        int rowsAffected = command.ExecuteNonQuery();
                        if (rowsAffected > 0)
                        {
                            return Json(new { success = true, message = "Book Return SuccessFully" });
                        }
                    }
                }
                return Json(new { success = false });
            }
            return Json(new { success = false });
        }

        public bool BookReturn(int Id)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string Status = "Submitted";
                    string query = "UPDATE IssuedBooksInformation SET Status = @Status WHERE IssuedID = @Id";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Id", Id);
                        command.Parameters.AddWithValue("@Status", Status);

                        int rowsAffected = command.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }


        
    }
}
