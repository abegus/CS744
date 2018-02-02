namespace _744Project.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Store
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Store()
        {
            StoreTransactions = new HashSet<StoreTransaction>();
        }

        public int storeID { get; set; }

        [StringLength(50)]
        public string storeName { get; set; }

        public int? storeWeight { get; set; }

        public int? relayID { get; set; }

        public virtual Relay Relay { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<StoreTransaction> StoreTransactions { get; set; }
    }
}
