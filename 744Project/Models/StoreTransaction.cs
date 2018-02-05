namespace _744Project.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class StoreTransaction
    {
        public int storeTransactionID { get; set; }

        [StringLength(50)]
        public string storeID { get; set; }

        public int? transactionID { get; set; }

        public DateTime? storeTransactionDate { get; set; }

        public virtual Store Store { get; set; }

        public virtual Transaction Transaction { get; set; }
    }
}
