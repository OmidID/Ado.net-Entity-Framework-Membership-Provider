using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Security;
using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;

namespace OmidID.Web.Security.Mapper {
    internal class UserAutoMapper<TUser> : IUserMapper<TUser>
        where TUser : class {

        private ClassHelper<TUser, UserColumnType, UserColumnAttribute> Helper { get; set; }

        internal UserAutoMapper(ClassHelper<TUser, UserColumnType, UserColumnAttribute> helper) {
            Helper = helper;
        }

        #region IMapper<TUser,MembershipUser,UserColumnType> Members

        public System.Web.Security.MembershipUser To(string ProviderName, TUser From) {
            return new System.Web.Security.MembershipUser(
                ProviderName,
                Get<string>(From, UserColumnType.Username),
                Get(From, UserColumnType.UserID),
                Get<string>(From, UserColumnType.Email),
                Get<string>(From, UserColumnType.PasswordQuestion),
                Get<string>(From, UserColumnType.Comment),
                Get<bool>(From, UserColumnType.IsApproved),
                Get<bool>(From, UserColumnType.IsLockedOut),
                Get<DateTime>(From, UserColumnType.CreateOn),
                Get<DateTime>(From, UserColumnType.LastLoginDate),
                Get<DateTime>(From, UserColumnType.LastActivityDate),
                Get<DateTime>(From, UserColumnType.LastPasswordChangedDate),
                Get<DateTime>(From, UserColumnType.LastLockoutDate));
        }

        public TUser To(System.Web.Security.MembershipUser From) {
            TUser model = Helper.New();
            return To(model, From);
        }

        public TUser To(TUser model, System.Web.Security.MembershipUser From) {
            Set(model, UserColumnType.Username, From.UserName);
            Set(model, UserColumnType.UserID, From.ProviderUserKey);
            Set(model, UserColumnType.Email, From.Email);
            Set(model, UserColumnType.PasswordQuestion, From.PasswordQuestion);
            Set(model, UserColumnType.Comment, From.Comment);
            Set(model, UserColumnType.IsApproved, From.IsApproved);
            Set(model, UserColumnType.IsLockedOut, From.IsLockedOut);
            Set(model, UserColumnType.CreateOn, From.CreationDate);
            Set(model, UserColumnType.LastLoginDate, From.LastLoginDate);
            Set(model, UserColumnType.LastActivityDate, From.LastActivityDate);
            Set(model, UserColumnType.LastPasswordChangedDate, From.LastPasswordChangedDate);
            Set(model, UserColumnType.LastLockoutDate, From.LastLockoutDate);

            return model;
        }

        public T Get<T>(TUser Entity, UserColumnType ColumnType) {
            var param = Expression.Parameter(Helper.ClassType, "p");
            var member = Helper.GetMemberAccess(param, ColumnType);
            if (member == null) return default(T);
            var ttype = typeof(T);
            Expression<Func<TUser, T>> lambda;
            if (member.Type == ttype)
                lambda = Expression.Lambda<Func<TUser, T>>(member, param);
            else
                lambda = Expression.Lambda<Func<TUser, T>>(Expression.Convert(member, typeof(T)), param);
            return lambda.Compile().Invoke(Entity);

            //var val = Get(Entity, ColumnType);
            //if (val == null) return default(T);

            //return (T)val;
        }

        public object Get(TUser Entity, UserColumnType ColumnType) {
            return Get<object>(Entity, ColumnType);
            //var prop = Helper.Get(ColumnType);
            //if (prop == null) return null;
            //return prop.GetValue(Entity, null);
        }

        public bool Set<TValue>(TUser Entity, UserColumnType ColumnType, TValue Value) {
            var param1 = Expression.Parameter(Helper.ClassType, "p1");
            var param2 = Expression.Parameter(typeof(TValue), "p2");
            var member = Helper.GetMemberAccess(param1, ColumnType);
            if (member == null) return false;
            Expression<Action<TUser, TValue>> lambda;
            if (member.Type == param2.Type)
                lambda = Expression.Lambda<Action<TUser, TValue>>(Expression.Assign(member, param2), param1, param2);
            else
                lambda = Expression.Lambda<Action<TUser, TValue>>(Expression.Assign(member, Expression.Convert(param2, member.Type)), param1, param2);
            lambda.Compile().Invoke(Entity, Value);

            return true;

            //var prop = Helper.Get(ColumnType);
            //if (prop == null) return false;

            //prop.SetValue(Entity, Value, null);
            //return true;
        }

        public object UserID(TUser user) { return Get(user, UserColumnType.UserID); }
        public IUserMapper<TUser> UserID(TUser user, object value) { Set(user, UserColumnType.UserID, value); return this; }

        public string Username(TUser user) { return Get<string>(user, UserColumnType.Username); }
        public IUserMapper<TUser> Username(TUser user, string value) { Set(user, UserColumnType.Username, value); return this; }

        public string Email(TUser user) { return Get<string>(user, UserColumnType.Email); }
        public IUserMapper<TUser> Email(TUser user, string value) { Set(user, UserColumnType.Email, value); return this; }

        public bool IsAnonymous(TUser user) { return Get<bool>(user, UserColumnType.IsAnonymous); }
        public IUserMapper<TUser> IsAnonymous(TUser user, bool value) { Set(user, UserColumnType.IsAnonymous, value); return this; }

        public string Password(TUser user) { return Get<string>(user, UserColumnType.Password); }
        public IUserMapper<TUser> Password(TUser user, string value) { Set(user, UserColumnType.Password, value); return this; }

        public System.Web.Security.MembershipPasswordFormat PasswordFormat(TUser user) { return Get<MembershipPasswordFormat>(user, UserColumnType.PasswordFormat); }
        public IUserMapper<TUser> PasswordFormat(TUser user, System.Web.Security.MembershipPasswordFormat value) { Set(user, UserColumnType.PasswordFormat, value); return this; }

        public string PasswordSalt(TUser user) { return Get<string>(user, UserColumnType.PasswordSalt); }
        public IUserMapper<TUser> PasswordSalt(TUser user, string value) { Set(user, UserColumnType.PasswordSalt, value); return this; }

        public string PasswordQuestion(TUser user) { return Get<string>(user, UserColumnType.PasswordQuestion); }
        public IUserMapper<TUser> PasswordQuestion(TUser user, string value) { Set(user, UserColumnType.PasswordQuestion, value); return this; }

        public string PasswordAnswer(TUser user) { return Get<string>(user, UserColumnType.PasswordAnswer); }
        public IUserMapper<TUser> PasswordAnswer(TUser user, string value) { Set(user, UserColumnType.PasswordAnswer, value); return this; }

        public bool IsApproved(TUser user) { return Get<bool>(user, UserColumnType.IsApproved); }
        public IUserMapper<TUser> IsApproved(TUser user, bool value) { Set(user, UserColumnType.IsApproved, value); return this; }

        public DateTime CreateOn(TUser user) { return Get<DateTime>(user, UserColumnType.CreateOn); }
        public IUserMapper<TUser> CreateOn(TUser user, DateTime value) { Set(user, UserColumnType.CreateOn, value); return this; }

        public DateTime LastLoginDate(TUser user) { return Get<DateTime>(user, UserColumnType.LastLoginDate); }
        public IUserMapper<TUser> LastLoginDate(TUser user, DateTime value) { Set(user, UserColumnType.LastLoginDate, value); return this; }

        public DateTime LastActivityDate(TUser user) { return Get<DateTime>(user, UserColumnType.LastActivityDate); }
        public IUserMapper<TUser> LastActivityDate(TUser user, DateTime value) { Set(user, UserColumnType.LastActivityDate, value); return this; }

        public DateTime LastPasswordChangedDate(TUser user) { return Get<DateTime>(user, UserColumnType.LastPasswordChangedDate); }
        public IUserMapper<TUser> LastPasswordChangedDate(TUser user, DateTime value) { Set(user, UserColumnType.LastPasswordChangedDate, value); return this; }

        public DateTime LastLockoutDate(TUser user) { return Get<DateTime>(user, UserColumnType.LastLockoutDate); }
        public IUserMapper<TUser> LastLockoutDate(TUser user, DateTime value) { Set(user, UserColumnType.LastLockoutDate, value); return this; }

        public int FailedPasswordAttemptCount(TUser user) { return Get<int>(user, UserColumnType.FailedPasswordAttemptCount); }
        public IUserMapper<TUser> FailedPasswordAttemptCount(TUser user, int value) { Set(user, UserColumnType.FailedPasswordAttemptCount, value); return this; }

        public DateTime FailedPasswordAttemptWindowStart(TUser user) { return Get<DateTime>(user, UserColumnType.FailedPasswordAttemptWindowStart); }
        public IUserMapper<TUser> FailedPasswordAttemptWindowStart(TUser user, DateTime value) { Set(user, UserColumnType.FailedPasswordAttemptWindowStart, value); return this; }

        public int FailedPasswordAnswerAttemptCount(TUser user) { return Get<int>(user, UserColumnType.FailedPasswordAnswerAttemptCount); }
        public IUserMapper<TUser> FailedPasswordAnswerAttemptCount(TUser user, int value) { Set(user, UserColumnType.FailedPasswordAnswerAttemptCount, value); return this; }

        public DateTime FailedPasswordAnswerAttemptWindowStart(TUser user) { return Get<DateTime>(user, UserColumnType.FailedPasswordAnswerAttemptWindowStart); }
        public IUserMapper<TUser> FailedPasswordAnswerAttemptWindowStart(TUser user, DateTime value) { Set(user, UserColumnType.FailedPasswordAnswerAttemptWindowStart, value); return this; }

        public string Comment(TUser user) { return Get<string>(user, UserColumnType.Comment); }
        public IUserMapper<TUser> Comment(TUser user, string value) { Set(user, UserColumnType.Comment, value); return this; }

        public bool IsLockedOut(TUser user) { return Get<bool>(user, UserColumnType.IsLockedOut); }
        public IUserMapper<TUser> IsLockedOut(TUser user, bool value) { Set(user, UserColumnType.IsLockedOut, value); return this; }

        #endregion
    }
}
