namespace _744Project.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class RelayToProcessCenterConnection
    {
        public int relayToProcessCenterConnectionID { get; set; }

        public int? relayID { get; set; }

        public int? processCenterID { get; set; }

        public int? relayToProcessCenterConnectionWeight { get; set; }

        public virtual ProcessCenter ProcessCenter { get; set; }

        public virtual Relay Relay { get; set; }
    }
}
