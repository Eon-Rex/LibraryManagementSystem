using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LMSProjectCode.Models
{
    public class IssuedBookModel
    {
        public int Id { get; set; }
        public int studentId { get; set; }
        public string StudentName { get; set; }
        public int BookId { get; set; }
        public string BookName { get; set; }
        public string AuthorName { get; set; }
        public string IssuedDate { get; set; }
        public string NumberOdDays { get; set; }
        public string Fine { get; set; }
        public int MobileNumber { get; set; }
        public string SubmissionLastDate
        {
            get
            {
                if (DateTime.TryParse(IssuedDate, out DateTime startDate) && int.TryParse(NumberOdDays, out int numberOfDays))
                {
                    return startDate.AddDays(numberOfDays).ToString("dd-MM-yyyy"); // Format the date as needed
                }
                return string.Empty;
            }
        }
        public string Status { get; set;}
        public int ColorCode
        {
            get
            {
                if (Status == "Submitted")
                {
                    return 1; 
                }
                else
                {
                    DateTime submissionDate;
                    if (DateTime.TryParse(SubmissionLastDate, out submissionDate))
                    {
                        if (submissionDate < DateTime.Now)
                        {
                            return 2; 
                        }
                        else
                        {
                            return 3;
                        }
                    }
                    return 0; 
                }
            }
        }
    }
}