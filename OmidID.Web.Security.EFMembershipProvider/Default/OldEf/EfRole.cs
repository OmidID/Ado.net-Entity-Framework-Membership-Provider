using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using OmidID.Web.Security.Mapper;

namespace OmidID.Web.Security.Default.OldEf {
    [Table("Roles")]
    public class EfRole {

        [Key]
        public int RoleID { get; set; }

        [Required]
        [MaxLength(256)]
        public string RoleName { get; set; }

        [MaxLength(256)]
        public string Description { get; set; }

        [ForeignKey("ApplicationID")]
        [RoleColumn(RoleColumnType.Application)]
        public EfApplication Application { get; set; }

        [Required]
        public int ApplicationID { get; set; }

        public ICollection<EfUserInRole> RoleUsers { get; set; }

    }
}
