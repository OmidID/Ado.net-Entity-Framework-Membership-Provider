using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using OmidID.Web.Security.Mapper;

namespace OmidID.Web.Security.Default.AspNet {

    [Table("Users")]
    public class AspUser {

        public AspUser() { UserID = Guid.NewGuid(); }

        [Key]
        [UserColumn(UserColumnType.UserID)]
        public Guid UserID { get; set; }

        [Required]
        public Guid ApplicationID { get; set; }

        [ForeignKey("ApplicationID")]
        [UserColumn(UserColumnType.Application)]
        public AspApplication Application { get; set; }

        string _UserName;
        [Required]
        [UserColumn(UserColumnType.Username)]
        [MaxLength(256)]
        public string UserName {
            get { return _UserName; }
            set { _UserName = value; LoweredUserName = value.ToLower(); }
        }

        [Required]
        [MaxLength(256)]
        public string LoweredUserName { get; set; }

        [MaxLength(16)]
        public string MobileAlias { get; set; }

        [Required]
        public bool IsAnonymous { get; set; }

        [Required]
        public DateTime LastActivityDate { get; set; }

        [JoinIt]
        [Required]
        public AspMembership Membership { get; set; }

        //public ICollection<UserInRole> UserRoles { get; set; }

    }

}
