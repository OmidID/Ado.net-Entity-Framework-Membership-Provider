using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using OmidID.Web.Security.Mapper;

namespace OmidID.Web.Security.Default.AspNet {

    [Table("Roles")]
    public class AspRole {

        public AspRole() { RoleID = Guid.NewGuid(); }

        [Key]
        public Guid RoleID { get; set; }

        string _RoleName;
        [Required]
        [MaxLength(256)]
        public string RoleName {
            get { return _RoleName; }
            set { _RoleName = value; LoweredRoleName = value.ToLower(); }
        }

        [Required]
        [MaxLength(256)]
        public string LoweredRoleName { get; set; }

        [MaxLength(256)]
        public string Description { get; set; }

        [ForeignKey("ApplicationID")]
        public AspApplication Application { get; set; }

        [Required]
        public Guid ApplicationID { get; set; }

        //public ICollection<UserInRole> RoleUsers { get; set; }

    }

}
