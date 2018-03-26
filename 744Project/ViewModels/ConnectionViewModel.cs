using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using _744Project.Models;
using System.Collections;


namespace _744Project.ViewModels
{
    public class ConnectionViewModel
    {
        public bool isActive { get; set; }
        public string sourceIp { get; set; }
        public int sourceType { get; set; }
        public string targetIp { get; set; }
        public int targetType { get; set; }
    }
}