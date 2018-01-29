namespace _744Project.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class ConnectionLink
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public ConnectionLink()
        {
            Connections = new HashSet<Connection>();
        }

        public int connectionLinkID { get; set; }

        public int? connectionID { get; set; }

        public int? transactionID { get; set; }

        public int? adminLogID { get; set; }

        public int? relayID { get; set; }

        public int? processCenterID { get; set; }

        public int? storeID { get; set; }

        public virtual AdminLog AdminLog { get; set; }

        public virtual ProcessCenter ProcessCenter { get; set; }

        public virtual Relay Relay { get; set; }

        public virtual Store Store { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Connection> Connections { get; set; }
    }
}
