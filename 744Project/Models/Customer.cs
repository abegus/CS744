namespace _744Project.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Customer
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Customer()
        {
            Accounts = new HashSet<Account>();
            CreditCards = new HashSet<CreditCard>();
        }
        [Display(Name = "Customer ID")]
        public int customerID { get; set; }
        [Display(Name ="Customer First Name")]
        [StringLength(50)]
        public string customerFirstname { get; set; }
        [Display(Name = "Customer Middle Name")]
        [StringLength(50)]
        public string middleName { get; set; }
        [Display(Name = "Customer Last Name")]
        [StringLength(50)]
        public string customerLastname { get; set; }
        [Display(Name = "Customer Phone")]
        [StringLength(50)]
        public string customerPhone { get; set; }
        [Display(Name = "Customer SSN")]
        [StringLength(50)]
        public string customerSSN { get; set; }
        [Display(Name = "Customer Address")]
        [StringLength(100)]
        public string customerAddress { get; set; }
        [Display(Name = "Customer City")]
        [StringLength(50)]
        public string customerCity { get; set; }
        [Display(Name = "Customer State")]
        [StringLength(50)]
        public string customerState { get; set; }
        [Display(Name = "Customer ZIP")]
        [StringLength(50)]
        public string customerZip { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Account> Accounts { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CreditCard> CreditCards { get; set; }
    }
}
