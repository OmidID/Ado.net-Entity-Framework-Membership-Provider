using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;

namespace OmidID.Web.Security.Mapper {
    internal class UserRoleAutoMapper<TUserRole> : IUserRoleMapper<TUserRole>
        where TUserRole : class {

        private ClassHelper<TUserRole, UserRoleColumnType, UserRoleColumnAttribute> Helper { get; set; }

        internal UserRoleAutoMapper(ClassHelper<TUserRole, UserRoleColumnType, UserRoleColumnAttribute> helper) {
            Helper = helper;
        }

        public T Get<T>(TUserRole Entity, UserRoleColumnType ColumnType) {
            var param = Expression.Parameter(Helper.ClassType, "p");
            var member = Helper.GetMemberAccess(param, ColumnType);
            if (member == null) return default(T);
            var ttype = typeof(T);
            Expression<Func<TUserRole, T>> lambda;
            if (member.Type == ttype)
                lambda = Expression.Lambda<Func<TUserRole, T>>(member, param);
            else
                lambda = Expression.Lambda<Func<TUserRole, T>>(Expression.Convert(member, typeof(T)), param);
            return lambda.Compile().Invoke(Entity);
        }

        public object Get(TUserRole Entity, UserRoleColumnType ColumnType) {
            return Get<object>(Entity, ColumnType);
        }

        public bool Set<TValue>(TUserRole Entity, UserRoleColumnType ColumnType, TValue Value) {
            var param1 = Expression.Parameter(Helper.ClassType, "p1");
            var param2 = Expression.Parameter(typeof(TValue), "p2");
            var member = Helper.GetMemberAccess(param1, ColumnType);
            if (member == null) return false;
            Expression<Action<TUserRole, TValue>> lambda;
            if (member.Type == param2.Type)
                lambda = Expression.Lambda<Action<TUserRole, TValue>>(Expression.Assign(member, param2), param1, param2);
            else
                lambda = Expression.Lambda<Action<TUserRole, TValue>>(Expression.Assign(member, Expression.Convert(param2, member.Type)), param1, param2);
            lambda.Compile().Invoke(Entity, Value);

            return true;
        }

        public object RoleID(TUserRole role) { return Get(role, UserRoleColumnType.RoleID); }
        public IUserRoleMapper<TUserRole> RoleID(TUserRole role, object value) { Set(role, UserRoleColumnType.RoleID, value); return this; }

        public object UserID(TUserRole role) { return Get(role, UserRoleColumnType.UserID); }
        public IUserRoleMapper<TUserRole> UserID(TUserRole role, object value) { Set(role, UserRoleColumnType.UserID, value); return this; }

    }
}
