using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using OmidID.Web.Security.Mapper;

namespace OmidID.Web.Security.Default.AspNet {

    [Table("Membership")]
    public class AspMembership {

        [Key]
        [Required]
        [ForeignKey("User")]
        public Guid UserID {
            get { return User == null ? _UserId : User.UserID; }
            set { if (User == null) _UserId = value; }
        }
        Guid _UserId;

        [Required]
        public Guid ApplicationID {
            get { return User == null ? _ApplicationId : User.ApplicationID; }
            set { if (User == null) _ApplicationId = value; }
        }
        Guid _ApplicationId;

        public AspUser User { get; set; }

        [Required]
        [MaxLength(128)]
        public string Password { get; set; }

        [Required]
        public int PasswordFormat { get; set; }

        [Required]
        [MaxLength(128)]
        public string PasswordSalt { get; set; }

        [MaxLength(16)]
        public string MobilePIN { get; set; }

        string _Email;
        [MaxLength(256)]
        public string Email {
            get { return _Email; }
            set { _Email = value; LoweredEmail = value.ToLower(); }
        }

        [MaxLength(256)]
        public string LoweredEmail { get; set; }

        [MaxLength(256)]
        public string PasswordQuestion { get; set; }

        [MaxLength(128)]
        public string PasswordAnswer { get; set; }

        [Required]
        public bool IsApproved { get; set; }

        [Required]
        public bool IsLockedOut { get; set; }

        [Required]
        [UserColumn(UserColumnType.CreateOn)]
        public DateTime CreateDate { get; set; }

        [Required]
        public DateTime LastLoginDate { get; set; }

        [Required]
        public DateTime LastPasswordChangedDate { get; set; }

        [Required]
        public DateTime LastLockoutDate { get; set; }

        [Required]
        public int FailedPasswordAttemptCount { get; set; }

        [Required]
        public DateTime FailedPasswordAttemptWindowStart { get; set; }

        [Required]
        public int FailedPasswordAnswerAttemptCount { get; set; }

        [Required]
        public DateTime FailedPasswordAnswerAttemptWindowStart { get; set; }

        public string Comment { get; set; }

    }

}
