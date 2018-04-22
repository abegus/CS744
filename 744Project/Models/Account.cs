namespace _744Project.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity;
    using System.Data.Entity.Spatial;

    public partial class Account
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Account()
        {
            CreditCards = new HashSet<CreditCard>();            
        }
        [Display(Name = "Account ID")]
        public int accountID { get; set; }
        //[Required]
        [Display(Name = "Account Number")]
        [StringLength(50)]
        public string accountNumber { get; set; }
        [Required]
        //[Range( , 9999999999999999)]
        [Display(Name = "Account Balance")]
        public decimal accountBalance { get; set; }
        //public int? customerID { get; set; }
        //[Required]
        //[StringLength(150, MinimumLength = 2)]
        [Display(Name = "Account Name")]
        public string accountName { get; set; }

        //
        [Required]
        [Display(Name = "Account First Name")]
        [StringLength(15, MinimumLength = 2)]
        public string accountFirstName { get; set; }
        [Required]
        [Display(Name = "Account Last Name")]
        [StringLength(15, MinimumLength = 2)]
        public string accountLastName { get; set; }
        [Required]
        [Display(Name = "Address")]
        [StringLength(100)]
        public string accountAddress { get; set; }
        //
        [Required]
        [Display(Name = "City")]
        [StringLength(50, MinimumLength = 2)]
        public string accountCity { get; set; }
        [Required]
        [Display(Name = "State")]
        [StringLength(50, MinimumLength = 2)]
        public string accountState { get; set; }
        [Required]
        [Display(Name = "Zip Code")]
        [StringLength(5, MinimumLength = 5)]
        [RegularExpression("^[0-9]*$", ErrorMessage = "Zip Code must be 5 digits")]
        public string accountZip { get; set; }        
        [Required]
        [Display(Name = "Phone")]
        //[StringLength(10, MinimumLength = 10)]
        //[RegularExpression(@"^\(\d{3}\)\s{0,1}\d{3}-\d{7}$", ErrorMessage = "Enter a valid number")]
        //[RegularExpression("^[0-9]*$", ErrorMessage = "Phone number must be 10 digits")]
        public string accountPhone { get; set; }
        [Required]
        [Range(0, 25000)]
        [Display(Name = "Account Maximum Allowed")]
        public int accountMax { get; set; }
        //

        //public virtual Customer Customer { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CreditCard> CreditCards { get; set; }

        /*[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Transaction> Transactions { get; set; }*/

        
    }
}
