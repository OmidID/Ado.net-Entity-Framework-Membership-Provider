using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace OmidID.Web.Security.Default.OldEf {
    [Table("UsersInRoles")]
    public class EfUserInRole {

        [Key]
        [Column("Users_UserID", Order = 1)]
        public int UserID { get; set; }

        [ForeignKey("UserID")]
        public EfUser User { get; set; }

        [Key]
        [Column("Roles_RoleID", Order = 2)]
        public int RoleID { get; set; }

        [ForeignKey("RoleID")]
        public EfRole Role { get; set; }

    }
}
