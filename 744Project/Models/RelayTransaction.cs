namespace _744Project.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class RelayTransaction
    {
        public int relayTransactionID { get; set; }

        [StringLength(50)]
        public string relayID { get; set; }

        public int? transactionID { get; set; }

        public DateTime? relayTransactionDate { get; set; }

        public virtual Relay Relay { get; set; }

        public virtual Transaction Transaction { get; set; }
    }
}
