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
            StoresToRelays = new HashSet<StoresToRelays>();
        }
        [Key]
        [Display(Name = "Store ID")]
        [StringLength(50)]
        public string storeID { get; set; }

        [StringLength(50)]
        [Display(Name = "Store IP")]
        public string storeIP { get; set; }

        [StringLength(50)]
        [Display(Name = "Store Name")]
        public string storeName { get; set; }
        [Display(Name = "Store Weight")]
        public int? storeWeight { get; set; }

        [StringLength(50)]        
        public string relayID { get; set; }

        public virtual Relay Relay { get; set; }

        public int regionID { get; set; }
        public virtual Regions Region { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<StoreTransaction> StoreTransactions { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<StoresToRelays> StoresToRelays { get; set; }
    
    }
}
