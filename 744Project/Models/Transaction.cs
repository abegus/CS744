namespace _744Project.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Transaction
    {
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

        public int? accountID { get; set; }

        public virtual Account Account { get; set; }

        public virtual CreditCard CreditCard { get; set; }
    }
}
