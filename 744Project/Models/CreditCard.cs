namespace _744Project.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class CreditCard
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public CreditCard()
        {
            Transactions = new HashSet<Transaction>();
        }
        [Display(Name = "Card ID")]
        [Key]
        public int cardID { get; set; }
        [Required]
        [Display(Name = "Card Number")]
        [StringLength(50)]
        public string cardNumber { get; set; }
        [Required]
        [Display(Name = "Card Expiration Date")]
        [DataType(DataType.Date)]
        public DateTime cardExpirationDate { get; set; }
        [Required]
        [Display(Name = "Card Security Code")]
        [StringLength(50)]
        public string cardSecurityCode { get; set; }
        [Required]
        [Display(Name = "Card Maximum Allowed")]
        [StringLength(50)]
        public string cardMaxAllowed { get; set; }

        public int? accountID { get; set; }

        public int? customerID { get; set; }

        public virtual Account Account { get; set; }

        public virtual Customer Customer { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Transaction> Transactions { get; set; }
    }
}
