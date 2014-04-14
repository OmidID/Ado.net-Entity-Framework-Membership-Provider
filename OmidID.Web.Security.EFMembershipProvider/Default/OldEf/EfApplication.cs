using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OmidID.Web.Security.Default.OldEf {

    [Table("Applications")]
    public class EfApplication : Models.Application {

        [Key]
        public int ApplicationID { get; set; }

        [Required]
        [MaxLength(100)]
        public override string Name { get; set; }

    }

}
