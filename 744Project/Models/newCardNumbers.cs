﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Spatial;

namespace _744Project.Models
{
    public class newCreditCardNumbers
    {


        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public newCreditCardNumbers()
        {

        }
        [Key]
        public int cardId { get; set; }
        [StringLength(16)]
        public string cardNumber { get; set; }

    }
}






