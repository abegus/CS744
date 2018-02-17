using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Spatial;

namespace _744Project.Models
{
    public partial class NodePosition
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public NodePosition()
        {

        }
        [Key]
        [StringLength(50)]
        public string Ip { get; set; }

        public decimal x { get; set; }
        public decimal y { get; set; }
    }
}