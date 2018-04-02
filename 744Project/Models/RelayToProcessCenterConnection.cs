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

        [StringLength(50)]
        public string relayID { get; set; }

        [StringLength(50)]
        public string processCenterID { get; set; }
        [Display(Name = "Relay to PC Weight")]
        public int? relayToProcessCenterConnectionWeight { get; set; }

        public Boolean isActive { get; set; }

        public virtual ProcessCenter ProcessCenter { get; set; }

        public virtual Relay Relay { get; set; }
    }
}
