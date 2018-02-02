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

        public int? relayID { get; set; }

        public int? relayWeight { get; set; }

        public virtual Relay Relay { get; set; }
    }
}
