using _744Project.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace _744Project.ViewModels
{
    public class QuestionsAnswersViewModel
    {
        public SecurityQuestion securityQuestions { get; set; }
        public Questions questions { get; set; }        
    }
}