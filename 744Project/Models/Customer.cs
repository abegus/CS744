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

        public int customerID { get; set; }

        [StringLength(50)]
        public string customerFirstname { get; set; }

        [StringLength(50)]
        public string middleName { get; set; }

        [StringLength(50)]
        public string customerLastname { get; set; }

        [StringLength(50)]
        public string customerPhone { get; set; }

        [StringLength(50)]
        public string customerSSN { get; set; }

        [StringLength(100)]
        public string customerAddress { get; set; }

        [StringLength(50)]
        public string customerCity { get; set; }

        [StringLength(50)]
        public string customerState { get; set; }

        [StringLength(50)]
        public string customerZip { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Account> Accounts { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CreditCard> CreditCards { get; set; }
    }
}
