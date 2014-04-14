
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
        ProviderVerification,
        ProviderVerificationExpireOn,
        Application

#if USE_WEBMATRIX
        ,
        ConfirmationCode,
        PasswordValidationToken,
        PasswordValidationTokenExpireOn,
        Registered
#endif

    }
}
