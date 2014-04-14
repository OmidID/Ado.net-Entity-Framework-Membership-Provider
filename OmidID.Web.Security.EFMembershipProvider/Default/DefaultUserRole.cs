using OmidID.Web.Security.Mapper;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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
        public
#if USE_WEBMATRIX
        int
#else
        long
#endif
        UserID { get; set; }

        [ForeignKey("UserID")]
        public DefaultUser User { get; set; }
        [ForeignKey("RoleID")]
        public DefaultRole Role { get; set; }


    }

}
