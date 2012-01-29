using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using OmidID.Web.Security.Mapper;

namespace OmidID.Web.Security.Default {

    //[Mapper.Mapper(typeof(DefaultUserMapper))]
    [Table("Users")]
    public class DefaultUser {

        #region Default Info

        [Key]
        [UserColumn(UserColumnType.UserID)]
        public long UserID { get; set; }

        [Required]
        [MaxLength(300)]
        [UserColumn(UserColumnType.Username)]
        public string Username { get; set; }

        [Required]
        [MaxLength(300)]
        [UserColumn(UserColumnType.Email)]
        public string Email { get; set; }

        [ForeignKey("ApplicationID")]
        [UserColumn(UserColumnType.Application)]
        public DefaultApplication Application { get; set; }

        [Required]
        public int ApplicationID { get; set; }

        #endregion

        #region Password

        [MaxLength(100)]
        [Required]
        [UserColumn(UserColumnType.Password)]
        public string Password { get; set; }

        [Required]
        [UserColumn(UserColumnType.PasswordFormat)]
        public int PasswordFormat { get; set; }

        [MaxLength(100)]
        [UserColumn(UserColumnType.PasswordSalt)]
        public string PasswordSalt { get; set; }


        [MaxLength(100)]
        [UserColumn(UserColumnType.PasswordQuestion)]
        public string PasswordQuestion { get; set; }

        [MaxLength(100)]
        [UserColumn(UserColumnType.PasswordAnswer)]
        public string PasswordAnswer { get; set; }

        #endregion

        #region Status

        [UserColumn(UserColumnType.IsAnonymous)]
        public bool IsAnonymus { get; set; }

        [UserColumn(UserColumnType.IsApproved)]
        public bool IsApproved { get; set; }

        [UserColumn(UserColumnType.IsLockedOut)]
        public bool IsLockedOut { get; set; }

        [UserColumn(UserColumnType.Comment)]
        public string Comment { get; set; }

        #endregion

        #region Dates

        [UserColumn(UserColumnType.LastActivityDate)]
        public DateTime LastActivityDate { get; set; }

        [UserColumn(UserColumnType.CreateOn)]
        public DateTime CreateOn { get; set; }

        [UserColumn(UserColumnType.LastLoginDate)]
        public DateTime LastLoginDate { get; set; }

        [UserColumn(UserColumnType.LastPasswordChangedDate)]
        public DateTime LastPasswordChangedDate { get; set; }

        [UserColumn(UserColumnType.LastLockoutDate)]
        public DateTime LastLockoutDate { get; set; }

        #endregion

        #region Attempt

        [UserColumn(UserColumnType.FailedPasswordAttemptCount)]
        public int FailedPasswordAttemptCount { get; set; }

        [UserColumn(UserColumnType.FailedPasswordAttemptWindowStart)]
        public DateTime FailedPasswordAttemptWindowStart { get; set; }

        [UserColumn(UserColumnType.FailedPasswordAnswerAttemptCount)]
        public int FailedPasswordAnswerAttemptCount { get; set; }

        [UserColumn(UserColumnType.FailedPasswordAnswerAttemptWindowStart)]
        public DateTime FailedPasswordAnswerAttemptWindowStart { get; set; }

        #endregion

    }
}
