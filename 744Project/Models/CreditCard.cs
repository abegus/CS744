namespace _744Project.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity;
    using System.Data.Entity.Spatial;

    public partial class CreditCard
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public CreditCard()
        {
            //Transactions = new HashSet<Transaction>();
        }
        [Display(Name = "Card ID")]
        [Key]
        public int cardID { get; set; }

        //[Required]
        [Display(Name = "Card Number")]
        [Range(1000000000000000, 9999999999999999, ErrorMessage = "Card Number must be 16 digits and not starting with a zero ")]
        public Int64 cardNumber { get; set; }
        [Required]
        [Display(Name = "Card Expiration Date")]
        [DataType(DataType.Date)]
        public DateTime cardExpirationDate { get; set; }
        [Required]
        [Display(Name = "Card Security Code")]
        [StringLength(3, MinimumLength = 3, ErrorMessage = "Security Code must be 3 digits")]
        [RegularExpression("^[0-9]*$", ErrorMessage = "Security Code must be 3 digits")]
        public string cardSecurityCode { get; set; }
        //[Required]
        //[Display(Name = "Card Maximum Allowed")]
        //[Range(0, 25000)]
        //public int cardMaxAllowed { get; set; }
        [Required]
        [Display(Name = "Account Number")]
        public int? accountID { get; set; }
        //
        [Required]
        [Display(Name = "Card First Name")]
        [StringLength(15, MinimumLength = 2)]
        public string firstName { get; set; }
        [Required]
        [Display(Name = "Card Last Name")]
        [StringLength(15, MinimumLength = 2)]
        public string lastName { get; set; }
        //

        //public int? customerID { get; set; }

        public virtual Account Account { get; set; }

        //public virtual Customer Customer { get; set; }

        //[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        //public virtual ICollection<Transaction> Transactions { get; set; }


    } 
}
