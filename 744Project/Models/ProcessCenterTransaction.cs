namespace _744Project.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class ProcessCenterTransaction
    {
        public int processCenterTransactionID { get; set; }

        [StringLength(50)]
        public string processCenterID { get; set; }

        public int? transactionID { get; set; }

        public DateTime? processCenterTransactionDate { get; set; }

        public virtual ProcessCenter ProcessCenter { get; set; }

        public virtual Transaction Transaction { get; set; }
    }
}
