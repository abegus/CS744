using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace _744Project.Models
{
    public class SecurityQuestions
    {
        // public HashSet<Question> Question { get; }
        [Key]
        public int SecurityQuestionsID { get; set; }

        [StringLength(50)]
        public string AspNetUserID { get; set; }

        public int QuestionID { get; set; }

        [StringLength(50)]
        public string Answer { get; set; }
    }
}