namespace _744Project.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Relay
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Relay()
        {
            RelayToProcessCenterConnections = new HashSet<RelayToProcessCenterConnection>();
            RelayToRelayConnections = new HashSet<RelayToRelayConnection>();
            RelayTransactions = new HashSet<RelayTransaction>();
            Stores = new HashSet<Store>();

            StoresToRelays = new HashSet<StoresToRelays>();
        }

        [StringLength(50)]
        public string relayID { get; set; }

        [StringLength(50)]
        public string relayName { get; set; }

        [StringLength(50)]
        public string relayIP { get; set; }

        public int regionID { get; set; }

        public Boolean isGateway { get; set; }

        public int relayQueue { get; set; }

        public Boolean isActive { get; set; }

        public virtual Regions Region { get; set; }


        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<RelayToProcessCenterConnection> RelayToProcessCenterConnections { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<RelayToRelayConnection> RelayToRelayConnections { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<RelayTransaction> RelayTransactions { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Store> Stores { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<StoresToRelays> StoresToRelays { get; set; }


    }
}
