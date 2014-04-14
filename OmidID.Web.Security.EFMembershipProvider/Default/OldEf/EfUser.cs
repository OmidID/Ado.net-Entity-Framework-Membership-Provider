using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OmidID.Web.Security.Default.OldEf {

    [Table("Users")]
    [EFDataMapper(typeof(EfUserMapper))]
    public class EfUser {

        [Key]
        [UserColumn(Mapper.UserColumnType.UserID)]
        public int UserID { get; set; }

        [Required]
        [MaxLength(256)]
        [UserColumn(Mapper.UserColumnType.Username)]
        public string Username { get; set; }

        [MaxLength(256)]
        [UserColumn(Mapper.UserColumnType.Email)]
        public string Email { get; set; }

        [Required]
        [UserColumn(Mapper.UserColumnType.IsAnonymous)]
        public bool IsAnonymous { get; set; }

        [Required]
        [UserColumn(Mapper.UserColumnType.LastActivityDate)]
        public DateTime LastActivityDate { get; set; }

        [Required]
        [MaxLength(128)]
        [UserColumn(Mapper.UserColumnType.Password)]
        public string Password { get; set; }

        [Required]
        [UserColumn(Mapper.UserColumnType.PasswordFormat)]
        public int PasswordFormat { get; set; }

        [Required]
        [MaxLength(128)]
        [UserColumn(Mapper.UserColumnType.PasswordSalt)]
        public string PasswordSalt { get; set; }

        [MaxLength(128)]
        [UserColumn(Mapper.UserColumnType.PasswordQuestion)]
        public string PasswordQuestion { get; set; }

        [MaxLength(128)]
        [UserColumn(Mapper.UserColumnType.PasswordAnswer)]
        public string PasswordAnswer { get; set; }

        [Required]
        [UserColumn(Mapper.UserColumnType.IsApproved)]
        public bool IsApproved { get; set; }

        [Required]
        [UserColumn(Mapper.UserColumnType.CreateOn)]
        public DateTime CreateOn { get; set; }

        [Required]
        [UserColumn(Mapper.UserColumnType.LastLoginDate)]
        public DateTime LastLoginDate { get; set; }

        [Required]
        [UserColumn(Mapper.UserColumnType.LastPasswordChangedDate)]
        public DateTime LastPasswordChangedDate { get; set; }

        [Required]
        [UserColumn(Mapper.UserColumnType.LastLockoutDate)]
        public DateTime LastLockoutDate { get; set; }

        [Required]
        [UserColumn(Mapper.UserColumnType.FailedPasswordAttemptCount)]
        public int FailedPasswordAttemptCount { get; set; }

        [Required]
        [UserColumn(Mapper.UserColumnType.FailedPasswordAttemptWindowStart)]
        public DateTime FailedPasswordAttemptWindowStart { get; set; }

        [Required]
        [UserColumn(Mapper.UserColumnType.FailedPasswordAnswerAttemptCount)]
        public int FailedPasswordAnswerAttemptCount { get; set; }

        [Required]
        [UserColumn(Mapper.UserColumnType.FailedPasswordAnswerAttemptWindowStart)]
        public DateTime FailedPasswordAnswerAttemptWindowStart { get; set; }

        [UserColumn(Mapper.UserColumnType.Comment)]
        public string Comment { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public int? TimeZone { get; set; }

        [Required]
        [UserColumn(Mapper.UserColumnType.IsLockedOut)]
        public byte Status { get; set; }

        [ForeignKey("ApplicationID")]
        [UserColumn(Mapper.UserColumnType.Application)]
        public EfApplication Application { get; set; }

        [Required]
        public int ApplicationID { get; set; }

        public ICollection<EfUserInRole> UserRoles { get; set; }

    }

}
