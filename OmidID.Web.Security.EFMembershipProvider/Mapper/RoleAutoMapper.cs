using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;

namespace OmidID.Web.Security.Mapper {
    internal class RoleAutoMapper<TRole> : IRoleMapper<TRole>
        where TRole : class {

        private ClassHelper<TRole, RoleColumnType, RoleColumnAttribute> Helper { get; set; }

        internal RoleAutoMapper(ClassHelper<TRole, RoleColumnType, RoleColumnAttribute> helper) {
            Helper = helper;
        }

        public T Get<T>(TRole Entity, RoleColumnType ColumnType) {
            var param = Expression.Parameter(Helper.ClassType, "p");
            var member = Helper.GetMemberAccess(param, ColumnType);
            if (member == null) return default(T);
            var ttype = typeof(T);
            Expression<Func<TRole, T>> lambda;
            if (member.Type == ttype)
                lambda = Expression.Lambda<Func<TRole, T>>(member, param);
            else
                lambda = Expression.Lambda<Func<TRole, T>>(Expression.Convert(member, typeof(T)), param);
            return lambda.Compile().Invoke(Entity);
        }

        public object Get(TRole Entity, RoleColumnType ColumnType) {
            return Get<object>(Entity, ColumnType);
        }

        public bool Set<TValue>(TRole Entity, RoleColumnType ColumnType, TValue Value) {
            var param1 = Expression.Parameter(Helper.ClassType, "p1");
            var param2 = Expression.Parameter(typeof(TValue), "p2");
            var member = Helper.GetMemberAccess(param1, ColumnType);
            if (member == null) return false;
            Expression<Action<TRole, TValue>> lambda;
            if (member.Type == param2.Type)
                lambda = Expression.Lambda<Action<TRole, TValue>>(Expression.Assign(member, param2), param1, param2);
            else
                lambda = Expression.Lambda<Action<TRole, TValue>>(Expression.Assign(member, Expression.Convert(param2, member.Type)), param1, param2);
            lambda.Compile().Invoke(Entity, Value);

            return true;
        }

        public object RoleID(TRole role) { return Get(role, RoleColumnType.RoleID); }
        public IRoleMapper<TRole> RoleID(TRole role, object value) { Set(role, RoleColumnType.RoleID, value); return this; }

        public string RoleName(TRole role) { return Get<string>(role, RoleColumnType.RoleName); }
        public IRoleMapper<TRole> RoleName(TRole role, string value) { Set(role, RoleColumnType.RoleName, value); return this; }

        public DateTime CreateOn(TRole role) { return Get<DateTime>(role, RoleColumnType.CreateOn); }
        public IRoleMapper<TRole> CreateOn(TRole role, DateTime value) { Set(role, RoleColumnType.CreateOn, value); return this; }

    }
}
