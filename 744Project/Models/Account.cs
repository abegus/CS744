namespace _744Project.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Account
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Account()
        {
            CreditCards = new HashSet<CreditCard>();
            Transactions = new HashSet<Transaction>();
        }        
        [Display(Name = "Account ID")]
        public int accountID { get; set; }
        [Required]
        [Display(Name = "Account Number")]
        [StringLength(50)]
        public string accountNumber { get; set; }
        [Required]
        [Display(Name = "Account Balance")]
        [StringLength(50)]
        public string accountBalance { get; set; }

        //public int? customerID { get; set; }
        [Required]
        [Display(Name = "Account Name")]
        public string accountName { get; set; }

        //public virtual Customer Customer { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CreditCard> CreditCards { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Transaction> Transactions { get; set; }
    }
}
