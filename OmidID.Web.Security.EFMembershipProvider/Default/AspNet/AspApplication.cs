using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OmidID.Web.Security.Default.AspNet {

    [Table("Applications")]
    public class AspApplication : Models.Application {

        public AspApplication() { ApplicationID = Guid.NewGuid(); }

        [Key]
        public Guid ApplicationID { get; set; }

        [Required]
        [MaxLength(256)]
        public string LoweredApplicationName { get; set; }

        string _ApplicationName;
        [Required]
        [MaxLength(256)]
        [Column("ApplicationName")]
        public override string Name {
            get { return _ApplicationName; }
            set { _ApplicationName = value; LoweredApplicationName = value.ToLower(); }
        }

    }

}
