using System;
using System.Web.Security;

namespace OmidID.Web.Security.Mapper {
    public interface IUserMapper<TUser> : IProviderMapper<TUser, System.Web.Security.MembershipUser, UserColumnType> {

        object UserID(TUser user);
        IUserMapper<TUser> UserID(TUser user, object value);

        string Username(TUser user);
        IUserMapper<TUser> Username(TUser user, string value);

        string Email(TUser user);
        IUserMapper<TUser> Email(TUser user, string value);

        bool IsAnonymous(TUser user);
        IUserMapper<TUser> IsAnonymous(TUser user, bool value);

        string Password(TUser user);
        IUserMapper<TUser> Password(TUser user, string value);

        MembershipPasswordFormat PasswordFormat(TUser user);
        IUserMapper<TUser> PasswordFormat(TUser user, MembershipPasswordFormat value);

        string PasswordSalt(TUser user);
        IUserMapper<TUser> PasswordSalt(TUser user, string value);

        string PasswordQuestion(TUser user);
        IUserMapper<TUser> PasswordQuestion(TUser user, string value);

        string PasswordAnswer(TUser user);
        IUserMapper<TUser> PasswordAnswer(TUser user, string value);

        bool IsApproved(TUser user);
        IUserMapper<TUser> IsApproved(TUser user, bool value);

        DateTime CreateOn(TUser user);
        IUserMapper<TUser> CreateOn(TUser user, DateTime value);

        DateTime LastLoginDate(TUser user);
        IUserMapper<TUser> LastLoginDate(TUser user, DateTime value);

        DateTime LastActivityDate(TUser user);
        IUserMapper<TUser> LastActivityDate(TUser user, DateTime value);

        DateTime LastPasswordChangedDate(TUser user);
        IUserMapper<TUser> LastPasswordChangedDate(TUser user, DateTime value);

        DateTime LastLockoutDate(TUser user);
        IUserMapper<TUser> LastLockoutDate(TUser user, DateTime value);

        int FailedPasswordAttemptCount(TUser user);
        IUserMapper<TUser> FailedPasswordAttemptCount(TUser user, int value);

        DateTime FailedPasswordAttemptWindowStart(TUser user);
        IUserMapper<TUser> FailedPasswordAttemptWindowStart(TUser user, DateTime value);

        int FailedPasswordAnswerAttemptCount(TUser user);
        IUserMapper<TUser> FailedPasswordAnswerAttemptCount(TUser user, int value);

        DateTime FailedPasswordAnswerAttemptWindowStart(TUser user);
        IUserMapper<TUser> FailedPasswordAnswerAttemptWindowStart(TUser user, DateTime value);

        string Comment(TUser user);
        IUserMapper<TUser> Comment(TUser user, string value);

        bool IsLockedOut(TUser user);
        IUserMapper<TUser> IsLockedOut(TUser user, bool value);


#if USE_WEBMATRIX

        string WebMatrixConfirmationCode(TUser user);
        IUserMapper<TUser> WebMatrixConfirmationCode(TUser user, string value);

        string WebMatrixPasswordValidationToken(TUser user);
        IUserMapper<TUser> WebMatrixPasswordValidationToken(TUser user, string value);

        DateTime WebMatrixPasswordValidationTokenExpireOn(TUser user);
        IUserMapper<TUser> WebMatrixPasswordValidationTokenExpireOn(TUser user, DateTime value);

        bool WebMatrixRegistered(TUser user);
        IUserMapper<TUser> WebMatrixRegistered(TUser user, bool value);

#endif

    }
}
