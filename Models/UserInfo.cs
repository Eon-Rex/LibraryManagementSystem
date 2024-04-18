using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMSProjectCode.Models
{
    public class UserInfo
    {
        public int IssuedID { get; set; }
        public string BookId { get; set; }
        public string StudentId { get; set; }
        public string StudentName { get; set; }
        public string StudentMobileNumber { get; set; }
        public string BookName { get; set; }
        public DateTime Date { get; set; }
        public string NumberofDays { get; set; }
        public string AuthorName { get; set; }
        public string  Status { get; set; }


    }
}