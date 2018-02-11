namespace _744Project.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class ProcessCenter
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public ProcessCenter()
        {
            ProcessCenterTransactions = new HashSet<ProcessCenterTransaction>();
            RelayToProcessCenterConnections = new HashSet<RelayToProcessCenterConnection>();
        }

        [StringLength(50)]
        public string processCenterID { get; set; }

        [StringLength(50)]
        public string processCenterIP { get; set; }

        [StringLength(50)]
        public string processCenterName { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ProcessCenterTransaction> ProcessCenterTransactions { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<RelayToProcessCenterConnection> RelayToProcessCenterConnections { get; set; }
    }
}
