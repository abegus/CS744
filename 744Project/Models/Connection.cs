namespace _744Project.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Connection
    {
        public int connectionID { get; set; }

        [StringLength(50)]
        public string connectionName { get; set; }

        public int? connectionLinkID { get; set; }

        public virtual ConnectionLink ConnectionLink { get; set; }
    }
}
