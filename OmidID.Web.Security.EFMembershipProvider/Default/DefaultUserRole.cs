using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using OmidID.Web.Security.Mapper;

namespace OmidID.Web.Security.Default {

    [Table("UserRoles")]
    public class DefaultUserRole {

        [Key]
        [Column(Order = 1)]
        [UserRoleColumn(UserRoleColumnType.RoleID)]
        public int RoleID { get; set; }

        [Key]
        [Column(Order = 2)]
        [UserRoleColumn(UserRoleColumnType.UserID)]
        public long UserID { get; set; }

        [ForeignKey("UserID")]
        public DefaultUser User { get; set; }
        [ForeignKey("RoleID")]
        public DefaultRole Role { get; set; }


    }

}
