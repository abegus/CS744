using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using _744Project.Models;
using System.Collections;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Spatial;

namespace _744Project.ViewModels
{
    public class NodeConfiguration
    {
        public List<NodeLocation> nodePosition { get; set; }
    }

    public class NodeLocation
    {
        public string id { get; set; }
        public Coordinates pos { get; set; }
        public int category { get; set; }
       public bool isActive { get; set; }
    }

    public class Coordinates
    {
        public decimal x { get; set; }
        public decimal y { get; set; }
    }
}