#if USE_WEBMATRIX

using System;
using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;

namespace OmidID.Web.Security.Mapper {
    public class OAuthMembershipAutoMapper<TOAuth, TUserKey> : IOAuthMembershipMapper<TOAuth, TUserKey>
        where TOAuth : class
    {
        
        private ClassHelper<TOAuth, OAuthMembershipColumnType, OAuthMembershipAttribute> Helper { get; set; }

        internal OAuthMembershipAutoMapper(ClassHelper<TOAuth, OAuthMembershipColumnType, OAuthMembershipAttribute> helper) {
            Helper = helper;
        }

        public T Get<T>(TOAuth Entity, OAuthMembershipColumnType ColumnType) {
            var param = Expression.Parameter(Helper.ClassType, "p");
            var member = Helper.GetMemberAccess(param, ColumnType);
            if (member == null) return default(T);
            var ttype = typeof(T);
            Expression<Func<TOAuth, T>> lambda;
            if (member.Type == ttype)
                lambda = Expression.Lambda<Func<TOAuth, T>>(member, param);
            else
                lambda = Expression.Lambda<Func<TOAuth, T>>(Expression.Convert(member, typeof(T)), param);
            return lambda.Compile().Invoke(Entity);
        }

        public object Get(TOAuth Entity, OAuthMembershipColumnType ColumnType) {
            return Get<object>(Entity, ColumnType);
        }

        public bool Set<TValue>(TOAuth Entity, OAuthMembershipColumnType ColumnType, TValue Value) {
            var param1 = Expression.Parameter(Helper.ClassType, "p1");
            var param2 = Expression.Parameter(typeof(TValue), "p2");
            var member = Helper.GetMemberAccess(param1, ColumnType);
            if (member == null) return false;
            Expression<Action<TOAuth, TValue>> lambda;
            if (member.Type == param2.Type)
                lambda = Expression.Lambda<Action<TOAuth, TValue>>(Expression.Assign(member, param2), param1, param2);
            else
                lambda = Expression.Lambda<Action<TOAuth, TValue>>(Expression.Assign(member, Expression.Convert(param2, member.Type)), param1, param2);
            lambda.Compile().Invoke(Entity, Value);

            return true;
        }

        public TUserKey UserID(TOAuth user) { return Get<TUserKey>(user, OAuthMembershipColumnType.UserID); }
        public IOAuthMembershipMapper<TOAuth, TUserKey> UserID(TOAuth user, TUserKey value) { Set(user, OAuthMembershipColumnType.UserID, value); return this; }

        public string ProviderToken(TOAuth user) { return Get<string>(user, OAuthMembershipColumnType.ProviderToken); }
        public IOAuthMembershipMapper<TOAuth, TUserKey> ProviderToken(TOAuth user, string value) { Set(user, OAuthMembershipColumnType.ProviderToken, value); return this; }

        public string ProviderName(TOAuth user) { return Get<string>(user, OAuthMembershipColumnType.ProviderName); }
        public IOAuthMembershipMapper<TOAuth, TUserKey> ProviderName(TOAuth user, string value) { Set(user, OAuthMembershipColumnType.ProviderName, value); return this; }

    }
}
#endif