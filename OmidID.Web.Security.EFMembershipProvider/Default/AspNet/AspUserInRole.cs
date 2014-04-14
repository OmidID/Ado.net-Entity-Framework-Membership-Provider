using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OmidID.Web.Security.Default.AspNet {

    [Table("UsersInRoles")]
    public class AspUserInRole {

        [Key]
        [Column("UserID", Order = 1)]
        [UserRoleColumn(Mapper.UserRoleColumnType.UserID)]
        public Guid UserID { get; set; }

        [ForeignKey("UserID")]
        public AspUser User { get; set; }



        [Key]
        [Column("RoleID", Order = 1)]
        [UserRoleColumn(Mapper.UserRoleColumnType.RoleID)]
        public Guid RoleID { get; set; }

        [ForeignKey("RoleID")]
        public AspRole Role { get; set; }

    }

}
