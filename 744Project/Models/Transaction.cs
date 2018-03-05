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

        [Display(Name = "Transaction Time")]
        [Required]
        public DateTime? transactionTime { get; set; }

        [Display(Name = "Transaction Amount")]
        [Required]
        [StringLength(50, MinimumLength = 2)]
        [RegularExpression("^[0-9]*$", ErrorMessage = "Transaction Amount must be digits.")]
        public string transactionAmount { get; set; }

        [Display(Name = "Transaction Type")]
        [Required]
        [StringLength(50, MinimumLength = 2)]
        public string transactionType { get; set; }

        [Display(Name = "Transaction Merchant")]
        [Required]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "The transaction Merchant length is two at least")]
        public string transactionMerchant { get; set; }

        public bool? transactionStatus { get; set; }

        public bool? encryptedFlag { get; set; }

        [Display(Name = "Card ID")]
        [Required]
        public int? cardID { get; set; }

        [Display(Name = "Store ID")]
        [Required]
        [StringLength(50)]
        public string storeID { get; set; }

        /* [Display(Name = "Account ID")]
         [Required]
         public int? accountID { get; set; }

         public virtual Account Account { get; set; }*/

        public virtual CreditCard CreditCard { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ProcessCenterTransaction> ProcessCenterTransactions { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<RelayTransaction> RelayTransactions { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<StoreTransaction> StoreTransactions { get; set; }
    }
}
