using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace _744Project.Models
{
    public class Questions
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Questions(){
            SecurityQuestions = new HashSet<SecurityQuestion>();
        }
        [Key]
        public int QuestionID { get; set; }

        [StringLength(50)]
        public string QuestionText { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SecurityQuestion> SecurityQuestions { get; set; }
    }
}