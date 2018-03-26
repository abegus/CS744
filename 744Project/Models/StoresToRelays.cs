using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace _744Project.Models
{
    public class StoresToRelays
    {
        //[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        //public StoresToRelays()
        //{
          
        //}

        //[Key]
        public int storesToRelaysID { get; set; }
        public Boolean isActive { get; set; }
        public int? weight { get; set; }

        [StringLength(50)]
        public string relayID { get; set; }
        public virtual Relay Relay { get; set; }

        [StringLength(50)]
        public string storeID { get; set; }
        public virtual Store Store { get; set; }
        

    }
}