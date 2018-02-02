namespace _744Project.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Transaction
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Transaction()
        {
            ProcessCenterTransactions = new HashSet<ProcessCenterTransaction>();
            RelayTransactions = new HashSet<RelayTransaction>();
            StoreTransactions = new HashSet<StoreTransaction>();
        }

        public int transactionID { get; set; }

        public DateTime? transactionTime { get; set; }

        [StringLength(50)]
        public string transactionAmount { get; set; }

        [StringLength(50)]
        public string transactionType { get; set; }

        [StringLength(100)]
        public string transactionMerchant { get; set; }

        public bool? transactionStatus { get; set; }

        public int? cardID { get; set; }

        public int? connectionID { get; set; }

        public int? accountID { get; set; }

        public virtual Account Account { get; set; }

        public virtual CreditCard CreditCard { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ProcessCenterTransaction> ProcessCenterTransactions { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<RelayTransaction> RelayTransactions { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<StoreTransaction> StoreTransactions { get; set; }
    }
}
