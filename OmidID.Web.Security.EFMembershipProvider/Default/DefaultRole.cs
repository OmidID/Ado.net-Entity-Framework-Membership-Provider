using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using OmidID.Web.Security.Mapper;

namespace OmidID.Web.Security.Default {

    [Table("Roles")]
    public class DefaultRole {

        [Key]
        [RoleColumn(RoleColumnType.RoleID)]
        public int RoleID { get; set; }

        [Required]
        [RoleColumn(RoleColumnType.RoleName)]
        public string RoleName { get; set; }

        [Required]
        [RoleColumn(RoleColumnType.CreateOn)]
        public DateTime CreateOn { get; set; }

        [ForeignKey("ApplicationID")]
        [RoleColumn(RoleColumnType.Application)]
        public DefaultApplication Application { get; set; }

        [Required]
        public int ApplicationID { get; set; }

    }

}
