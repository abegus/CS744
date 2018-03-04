using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using _744Project.Models;
using System.Collections;

namespace _744Project.ViewModels
{
    public class AccountsCreditsViewModel
    {
        //private MasterModel db = new MasterModel();
        public Account account { get; set; }
        public CreditCard creditCard { get; set; }
    }

}