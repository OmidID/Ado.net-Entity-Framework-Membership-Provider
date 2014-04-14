using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OmidID.Web.Security.Default {

    [Table("Applications")]
    public class DefaultApplication : Models.Application {
        
        [Key]
        public int ApplicationID { get; set; }

        [Column()]
        [MaxLength(256)]
        public override string Name { get; set; }

    }
}
