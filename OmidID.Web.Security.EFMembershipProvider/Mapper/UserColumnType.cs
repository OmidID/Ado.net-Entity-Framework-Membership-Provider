using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OmidID.Web.Security.Mapper {
    public enum UserColumnType {

        UserID,
        Username,
        Email,
        IsAnonymous,
        LastActivityDate,
        Password,
        PasswordFormat,
        PasswordSalt,
        PasswordQuestion,
        PasswordAnswer,
        IsApproved,
        CreateOn,
        LastLoginDate,
        LastPasswordChangedDate,
        LastLockoutDate,
        FailedPasswordAttemptCount,
        FailedPasswordAttemptWindowStart,
        FailedPasswordAnswerAttemptCount,
        FailedPasswordAnswerAttemptWindowStart,
        Comment,
        IsLockedOut,
        Application

    }
}
