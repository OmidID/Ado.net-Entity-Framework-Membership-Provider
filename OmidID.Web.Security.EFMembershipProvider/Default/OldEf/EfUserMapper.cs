using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OmidID.Web.Security.Mapper;
using System.Web.Security;

namespace OmidID.Web.Security.Default.OldEf {

    #region Helper class

    internal static class InternalConverterHelper {
        public static T CType<T>(this object val) { return (T)Convert.ChangeType(val, typeof(T)); }
    }

    #endregion

    public class EfUserMapper : IUserMapper<EfUser> {

        #region IMapper<EfUser,MembershipUser,UserColumnType> Members

        public System.Web.Security.MembershipUser To(string ProviderName, EfUser From) {
            return MembershipUser.Map(ProviderName, From, true);
        }

        public EfUser To(System.Web.Security.MembershipUser From) {
            EfUser model = new EfUser();
            return To(model, From);
        }

        public EfUser To(EfUser model, System.Web.Security.MembershipUser From) {
            model.Username = From.UserName;
            model.UserID = (int)From.ProviderUserKey;
            model.Email = From.Email;
            model.PasswordQuestion = From.PasswordQuestion;
            model.Comment = From.Comment;
            model.IsApproved = From.IsApproved;
            model.Status = From.IsLockedOut ? (byte)2 : (byte)1;
            model.CreateOn = From.CreationDate;
            model.LastLoginDate = From.LastLoginDate;
            model.LastActivityDate = From.LastActivityDate;
            model.LastPasswordChangedDate = From.LastPasswordChangedDate;
            model.LastLockoutDate = From.LastLockoutDate;

            if (From is MembershipUser) {
                var nFrom = (MembershipUser)From;
                model.TimeZone = nFrom.TimeZone;
                model.FirstName = nFrom.FirstName;
                model.LastName = nFrom.LastName;
            }

            return model;
        }

        public T Get<T>(EfUser Entity, UserColumnType ColumnType) {
            switch (ColumnType) {
                case UserColumnType.Application: return Entity.Application.CType<T>();
                case UserColumnType.UserID: return Entity.UserID.CType<T>();
                case UserColumnType.Username: return Entity.Username.CType<T>();
                case UserColumnType.Email: return Entity.Email.CType<T>();
                case UserColumnType.IsAnonymous: return Entity.IsAnonymous.CType<T>();
                case UserColumnType.Password: return Entity.Password.CType<T>();
                case UserColumnType.PasswordFormat: return Entity.PasswordFormat.CType<T>();
                case UserColumnType.PasswordSalt: return Entity.PasswordSalt.CType<T>();
                case UserColumnType.PasswordQuestion: return Entity.PasswordQuestion.CType<T>();
                case UserColumnType.PasswordAnswer: return Entity.PasswordAnswer.CType<T>();
                case UserColumnType.IsApproved: return Entity.IsApproved.CType<T>();
                case UserColumnType.CreateOn: return Entity.CreateOn.CType<T>();
                case UserColumnType.LastLoginDate: return Entity.LastLoginDate.CType<T>();
                case UserColumnType.LastActivityDate: return Entity.LastActivityDate.CType<T>();
                case UserColumnType.LastPasswordChangedDate: return Entity.LastPasswordChangedDate.CType<T>();
                case UserColumnType.LastLockoutDate: return Entity.LastLockoutDate.CType<T>();
                case UserColumnType.FailedPasswordAttemptCount: return Entity.FailedPasswordAttemptCount.CType<T>();
                case UserColumnType.FailedPasswordAttemptWindowStart: return Entity.FailedPasswordAttemptWindowStart.CType<T>();
                case UserColumnType.FailedPasswordAnswerAttemptCount: return Entity.FailedPasswordAnswerAttemptCount.CType<T>();
                case UserColumnType.FailedPasswordAnswerAttemptWindowStart: return Entity.FailedPasswordAnswerAttemptWindowStart.CType<T>();
                case UserColumnType.Comment: return Entity.Comment.CType<T>();
                case UserColumnType.IsLockedOut: return (Entity.Status == 2).CType<T>(); ;
                default: return default(T);
            }
        }

        public object Get(EfUser Entity, UserColumnType ColumnType) {
            return Get<object>(Entity, ColumnType);
        }

        public bool Set<TValue>(EfUser Entity, UserColumnType ColumnType, TValue Value) {
            switch (ColumnType) {
                case UserColumnType.Application: Entity.Application = Value.CType<EfApplication>(); break;
                case UserColumnType.UserID: Entity.UserID = Value.CType<int>(); break;
                case UserColumnType.Username: Entity.Username = Value.CType<string>(); break;
                case UserColumnType.Email: Entity.Email = Value.CType<string>(); break;
                case UserColumnType.IsAnonymous: Entity.IsAnonymous = Value.CType<bool>(); break;
                case UserColumnType.Password: Entity.Password = Value.CType<string>(); break;
                case UserColumnType.PasswordFormat: Entity.PasswordFormat = Value.CType<int>(); break;
                case UserColumnType.PasswordSalt: Entity.PasswordSalt = Value.CType<string>(); break;
                case UserColumnType.PasswordQuestion: Entity.PasswordQuestion = Value.CType<string>(); break;
                case UserColumnType.PasswordAnswer: Entity.PasswordAnswer = Value.CType<string>(); break;
                case UserColumnType.IsApproved: Entity.IsApproved = Value.CType<bool>(); break;
                case UserColumnType.CreateOn: Entity.CreateOn = Value.CType<DateTime>(); break;
                case UserColumnType.LastLoginDate: Entity.LastLoginDate = Value.CType<DateTime>(); break;
                case UserColumnType.LastActivityDate: Entity.LastActivityDate = Value.CType<DateTime>(); break;
                case UserColumnType.LastPasswordChangedDate: Entity.LastPasswordChangedDate = Value.CType<DateTime>(); break;
                case UserColumnType.LastLockoutDate: Entity.LastLockoutDate = Value.CType<DateTime>(); break;
                case UserColumnType.FailedPasswordAttemptCount: Entity.FailedPasswordAttemptCount = Value.CType<int>(); break;
                case UserColumnType.FailedPasswordAttemptWindowStart: Entity.FailedPasswordAttemptWindowStart = Value.CType<DateTime>(); break;
                case UserColumnType.FailedPasswordAnswerAttemptCount: Entity.FailedPasswordAnswerAttemptCount = Value.CType<int>(); break;
                case UserColumnType.FailedPasswordAnswerAttemptWindowStart: Entity.FailedPasswordAnswerAttemptWindowStart = Value.CType<DateTime>(); break;
                case UserColumnType.Comment: Entity.Comment.CType<string>(); break;
                case UserColumnType.IsLockedOut: Entity.Status = Value.CType<bool>() ? (byte)2 : (byte)1; break;
                default: return false;
            }
            return true;
        }

        public object UserID(EfUser user) { return Get(user, UserColumnType.UserID); }
        public IUserMapper<EfUser> UserID(EfUser user, object value) { Set(user, UserColumnType.UserID, value); return this; }

        public string Username(EfUser user) { return Get<string>(user, UserColumnType.Username); }
        public IUserMapper<EfUser> Username(EfUser user, string value) { Set(user, UserColumnType.Username, value); return this; }

        public string Email(EfUser user) { return Get<string>(user, UserColumnType.Email); }
        public IUserMapper<EfUser> Email(EfUser user, string value) { Set(user, UserColumnType.Email, value); return this; }

        public bool IsAnonymous(EfUser user) { return Get<bool>(user, UserColumnType.IsAnonymous); }
        public IUserMapper<EfUser> IsAnonymous(EfUser user, bool value) { Set(user, UserColumnType.IsAnonymous, value); return this; }

        public string Password(EfUser user) { return Get<string>(user, UserColumnType.Password); }
        public IUserMapper<EfUser> Password(EfUser user, string value) { Set(user, UserColumnType.Password, value); return this; }

        public System.Web.Security.MembershipPasswordFormat PasswordFormat(EfUser user) { return Get<MembershipPasswordFormat>(user, UserColumnType.PasswordFormat); }
        public IUserMapper<EfUser> PasswordFormat(EfUser user, System.Web.Security.MembershipPasswordFormat value) { Set(user, UserColumnType.PasswordFormat, value); return this; }

        public string PasswordSalt(EfUser user) { return Get<string>(user, UserColumnType.PasswordSalt); }
        public IUserMapper<EfUser> PasswordSalt(EfUser user, string value) { Set(user, UserColumnType.PasswordSalt, value); return this; }

        public string PasswordQuestion(EfUser user) { return Get<string>(user, UserColumnType.PasswordQuestion); }
        public IUserMapper<EfUser> PasswordQuestion(EfUser user, string value) { Set(user, UserColumnType.PasswordQuestion, value); return this; }

        public string PasswordAnswer(EfUser user) { return Get<string>(user, UserColumnType.PasswordAnswer); }
        public IUserMapper<EfUser> PasswordAnswer(EfUser user, string value) { Set(user, UserColumnType.PasswordAnswer, value); return this; }

        public bool IsApproved(EfUser user) { return Get<bool>(user, UserColumnType.IsApproved); }
        public IUserMapper<EfUser> IsApproved(EfUser user, bool value) { Set(user, UserColumnType.IsApproved, value); return this; }

        public DateTime CreateOn(EfUser user) { return Get<DateTime>(user, UserColumnType.CreateOn); }
        public IUserMapper<EfUser> CreateOn(EfUser user, DateTime value) { Set(user, UserColumnType.CreateOn, value); return this; }

        public DateTime LastLoginDate(EfUser user) { return Get<DateTime>(user, UserColumnType.LastLoginDate); }
        public IUserMapper<EfUser> LastLoginDate(EfUser user, DateTime value) { Set(user, UserColumnType.LastLoginDate, value); return this; }

        public DateTime LastActivityDate(EfUser user) { return Get<DateTime>(user, UserColumnType.LastActivityDate); }
        public IUserMapper<EfUser> LastActivityDate(EfUser user, DateTime value) { Set(user, UserColumnType.LastActivityDate, value); return this; }

        public DateTime LastPasswordChangedDate(EfUser user) { return Get<DateTime>(user, UserColumnType.LastPasswordChangedDate); }
        public IUserMapper<EfUser> LastPasswordChangedDate(EfUser user, DateTime value) { Set(user, UserColumnType.LastPasswordChangedDate, value); return this; }

        public DateTime LastLockoutDate(EfUser user) { return Get<DateTime>(user, UserColumnType.LastLockoutDate); }
        public IUserMapper<EfUser> LastLockoutDate(EfUser user, DateTime value) { Set(user, UserColumnType.LastLockoutDate, value); return this; }

        public int FailedPasswordAttemptCount(EfUser user) { return Get<int>(user, UserColumnType.FailedPasswordAttemptCount); }
        public IUserMapper<EfUser> FailedPasswordAttemptCount(EfUser user, int value) { Set(user, UserColumnType.FailedPasswordAttemptCount, value); return this; }

        public DateTime FailedPasswordAttemptWindowStart(EfUser user) { return Get<DateTime>(user, UserColumnType.FailedPasswordAttemptWindowStart); }
        public IUserMapper<EfUser> FailedPasswordAttemptWindowStart(EfUser user, DateTime value) { Set(user, UserColumnType.FailedPasswordAttemptWindowStart, value); return this; }

        public int FailedPasswordAnswerAttemptCount(EfUser user) { return Get<int>(user, UserColumnType.FailedPasswordAnswerAttemptCount); }
        public IUserMapper<EfUser> FailedPasswordAnswerAttemptCount(EfUser user, int value) { Set(user, UserColumnType.FailedPasswordAnswerAttemptCount, value); return this; }

        public DateTime FailedPasswordAnswerAttemptWindowStart(EfUser user) { return Get<DateTime>(user, UserColumnType.FailedPasswordAnswerAttemptWindowStart); }
        public IUserMapper<EfUser> FailedPasswordAnswerAttemptWindowStart(EfUser user, DateTime value) { Set(user, UserColumnType.FailedPasswordAnswerAttemptWindowStart, value); return this; }

        public string Comment(EfUser user) { return Get<string>(user, UserColumnType.Comment); }
        public IUserMapper<EfUser> Comment(EfUser user, string value) { Set(user, UserColumnType.Comment, value); return this; }

        public bool IsLockedOut(EfUser user) { return Get<bool>(user, UserColumnType.IsLockedOut); }
        public IUserMapper<EfUser> IsLockedOut(EfUser user, bool value) { Set(user, UserColumnType.IsLockedOut, value); return this; }

        #endregion
    }
}
