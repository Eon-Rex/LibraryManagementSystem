using LMSProjectCode.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Xml.Linq;

namespace LMSProjectCode.Controllers
{
    public class HomeController : Controller
    {
        private readonly string connectionString = ConfigurationManager.ConnectionStrings["DB_WeighBridge"].ConnectionString;

        //private readonly string connectionString = "Data Source=DESKTOP-7PD7Q5Q\\SQLEXPRESS;Initial Catalog=Login_DBS;User Id=User1;Password=Suraj;";
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult UserMethod()
        {

            List<BookInformationViewModel> bookCollection = new List<BookInformationViewModel>();


            // SQL query to select data from BookCollection table
            string query = "SELECT BookId, BookName, Author, Quantity FROM BookCollection where Quantity > 0";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);

                connection.Open();

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        // Map the retrieved data to BookInformationViewModel
                        bookCollection.Add(new BookInformationViewModel
                        {
                            BookId = Convert.ToInt32(reader["BookId"]),
                            BookName = reader["BookName"].ToString(),
                            Author = reader["Author"].ToString(),
                            Quantity = Convert.ToInt32(reader["Quantity"])
                        });
                    }
                }
            }

            return View(bookCollection);
        }
        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
        [HttpPost]
        public ActionResult SubmitBook(string BookName, string Author, string Quantity)
        {
            if (string.IsNullOrEmpty(BookName) || string.IsNullOrEmpty(Author) || string.IsNullOrEmpty(Quantity))
            {
                List<string> blankFields = new List<string>();

                if (string.IsNullOrEmpty(BookName))
                    blankFields.Add("BookName");

                if (string.IsNullOrEmpty(Author))
                    blankFields.Add("Author");

                if (string.IsNullOrEmpty(Quantity))
                    blankFields.Add("Quantity");

                return Json(new { success = false, blankFields = blankFields });
            }

            using (var connection = new SqlConnection(connectionString))
            {
               // connection.Open();
                connection.Open();
                string selectQuery = "SELECT COUNT(*) FROM BookCollection WHERE BookName = @BookName AND Author = @Author";
                string insertQuery = "INSERT INTO BookCollection (BookName, Author, Quantity) VALUES (@BookName, @Author, @Quantity)";
                string updateQuery = "UPDATE BookCollection SET Quantity = @Quantity WHERE BookName = @BookName AND Author = @Author";

                try
                {
                    using (var selectCommand = new SqlCommand(selectQuery, connection))
                    {
                        selectCommand.Parameters.AddWithValue("@BookName", BookName);
                        selectCommand.Parameters.AddWithValue("@Author", Author);

                        int existingCount = (int)selectCommand.ExecuteScalar();

                        if (existingCount > 0)
                        {
                            // If record exists, update it
                            using (var updateCommand = new SqlCommand(updateQuery, connection))
                            {
                                updateCommand.Parameters.AddWithValue("@BookName", BookName);
                                updateCommand.Parameters.AddWithValue("@Author", Author);
                                updateCommand.Parameters.AddWithValue("@Quantity", Quantity);

                                updateCommand.ExecuteNonQuery();
                            }
                        }
                        else
                        {
                            using (var insertCommand = new SqlCommand(insertQuery, connection))
                            {
                                insertCommand.Parameters.AddWithValue("@BookName", BookName);
                                insertCommand.Parameters.AddWithValue("@Author", Author);
                                insertCommand.Parameters.AddWithValue("@Quantity", Quantity);

                                insertCommand.ExecuteNonQuery();
                            }
                        }
                    }

                    return Json(new { success = true ,message = "record already exist quantity"});
                }
                catch (Exception ex)
                {
                    return Json(new { success = false, error = ex.Message });
                }
                finally
                {
                    connection.Close();
                }


                return View(); // Consider returning a specific error message in case of a database error
            }
        }

        public ActionResult ShowBookInformation()
        {
            List<BookInformationViewModel> bookCollection = new List<BookInformationViewModel>();


            // SQL query to select data from BookCollection table
            string query = "SELECT BookId, BookName, Author, Quantity FROM BookCollection";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);

                connection.Open();

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        // Map the retrieved data to BookInformationViewModel
                        bookCollection.Add(new BookInformationViewModel
                        {
                            BookId = Convert.ToInt32(reader["BookId"]),
                            BookName = reader["BookName"].ToString(),
                            Author = reader["Author"].ToString(),
                            Quantity = Convert.ToInt32(reader["Quantity"])
                        });
                    }
                }
            }

            return View(bookCollection);
        }
        [HttpPost]
        public JsonResult DeleteRecord(string Id)
        {
            int bookId = Convert.ToInt32(Id);

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string query = "DELETE FROM BookCollection WHERE BookId = @BookId";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@BookId", bookId);

                int rowsAffected = command.ExecuteNonQuery();

                if (rowsAffected > 0)
                {
                    return Json(new { success = true });
                }
                else
                {
                    return Json(new { success = false, message = "Book not found or already deleted." });
                }
            }
        }
        [HttpPost]
        public JsonResult UpdateRecord(int BookId, string UpdatedBookName, string UpdatedAuthor, int UpdatedQuantity)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string query = "UPDATE BookCollection SET BookName = @UpdatedBookName, Author = @UpdatedAuthor, Quantity = @UpdatedQuantity WHERE BookId = @BookId";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@BookId", BookId);
                        command.Parameters.AddWithValue("@UpdatedBookName", UpdatedBookName);
                        command.Parameters.AddWithValue("@UpdatedAuthor", UpdatedAuthor);
                        command.Parameters.AddWithValue("@UpdatedQuantity", UpdatedQuantity);

                        int rowsAffected = command.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            return Json(new { success = true });
                        }
                        else
                        {
                            return Json(new { success = false, message = "No record updated." });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }


        [HttpPost]
        public JsonResult RequestBook(int BookId, int NumberOfDays)
        {
            bool Isvalid = BookAlreadyIssued(BookId);
            if (!Isvalid) {
                bool BookInserted = BookInsertedMethod(BookId, NumberOfDays);
                if (BookInserted)
                {
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();
                        using (SqlCommand command = new SqlCommand("UPDATE BookCollection SET Quantity = Quantity - 1 WHERE BookId = @BookId AND Quantity > 0", connection))
                        {
                            command.Parameters.AddWithValue("@BookId", BookId);

                            int rowsAffected = command.ExecuteNonQuery();
                            if (rowsAffected > 0)
                            {
                                return Json(new { success = true, message = "Book Issued SuccessFully" });
                            }
                        }
                    }
                    return Json(new { success = false });
                }
                return Json(new { success = false });
            }
            else
            {
                return Json(new { success = false , message="You have Already Issued This Book"});
            }
        }

        private bool BookInsertedMethod(int BookId, int NumberOfDays)
        {
            string StudentID = Session["StudentId"] as string;

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string query = @"
                                    INSERT INTO issuedbooksinformation (BookID, StudentId, StudentEmail, StudentMobileNumber, BookName, Date, NumberOfDays, AuthorName,StudentName)
                                    SELECT B.Bookid, U.Id, U.Email, U.Mobile, B.BookName, GETDATE(), @NumberOfDays, B.Author, U.Username
                                    FROM bookcollection B
                                    INNER JOIN Users U ON U.Id = @StudentId
                                    WHERE B.BookId = @BookId;
                                ";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@BookId", BookId);
                        command.Parameters.AddWithValue("@NumberOfDays", NumberOfDays);
                        command.Parameters.AddWithValue("@StudentId", StudentID);

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
        private bool  BookAlreadyIssued(int BookId)
        { 
            string id = Session["StudentId"] as string;
            int.TryParse(id, out int StudentID);

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string Submitted = "NotSubmitted";
                    string query = "SELECT COUNT(*) FROM IssuedBooksInformation WHERE BookId = @BookId AND StudentId = @StudentId AND Status =@Submitted";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@BookId", BookId);
                        command.Parameters.AddWithValue("@StudentId", StudentID);
                        command.Parameters.AddWithValue("@Submitted", Submitted);

                        object result = command.ExecuteScalar();
                        int count = Convert.ToInt32(result);
                        if (count <=0 )
                        {
                            return false;// Now 'count' contains the number of records that match the conditions
                        }
                        else
                        {
                            return true;
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