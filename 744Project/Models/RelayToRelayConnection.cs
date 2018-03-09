namespace _744Project.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class RelayToRelayConnection
    {
        [Key]
       public int relayConnectionID { get; set; }

        
        [StringLength(50)]
        public string relayID { get; set; }

        //[Key]
        [StringLength(50)]
        public string relayID2 { get; set; }

        public int? relayWeight { get; set; }

        public Boolean isActive { get; set; }

        public virtual Relay Relay { get; set; }
        public virtual Relay Relay2 { get; set; }
    }
}
