using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OmidID.Web.Security.Default.OldEf {

    [Serializable]
    public class MembershipUser : System.Web.Security.MembershipUser {


        public static System.Web.Security.MembershipUser Map(string pname, EfUser user, bool EFMembership) {

            if (EFMembership)
                return new MembershipUser(pname, user.Username, user.UserID, user.Email, user.PasswordQuestion, user.Comment, user.IsApproved,
                                          user.Status == 2, user.CreateOn, user.LastLoginDate, user.LastActivityDate, user.LastPasswordChangedDate,
                                          user.LastLockoutDate, user.FirstName, user.LastName, user.TimeZone.GetValueOrDefault(0));
            else
                return new System.Web.Security.MembershipUser(pname, user.Username, user.UserID, user.Email, user.PasswordQuestion, user.Comment, user.IsApproved,
                                                              user.Status == 2, user.CreateOn, user.LastLoginDate, user.LastActivityDate, user.LastPasswordChangedDate,
                                                              user.LastLockoutDate);
        }


        public MembershipUser(string providerName, string name, object providerUserKey, string email, string passwordQuestion, string comment, bool isApproved, bool isLockedOut, DateTime creationDate, DateTime lastLoginDate, DateTime lastActivityDate, DateTime lastPasswordChangedDate, DateTime lastLockoutDate, string firstName, string lastName, int timeZone) :
            base(providerName, name, providerUserKey, email, passwordQuestion, comment, isApproved, isLockedOut, creationDate, lastLoginDate, lastActivityDate, lastPasswordChangedDate, lastLockoutDate) {

            FirstName = firstName;
            LastName = lastName;
            TimeZone = timeZone;
        }
        public MembershipUser() { }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int TimeZone { get; set; }

    }
}
