using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using _744Project.Models;
namespace _744Project.ViewModels
{
    public class RegionsRelaysStoresViewModel
    {        
        public Regions regions { get; set; }
        public Relay relay { get; set; }
        public Store store { get; set; }
        public RelayToProcessCenterConnection relayToProcessCenterConnection { get; set; }
    }
}