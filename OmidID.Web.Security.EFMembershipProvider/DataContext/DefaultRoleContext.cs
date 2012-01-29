using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;
using System.Data.Entity.Infrastructure;
using System.Data.Entity;

namespace OmidID.Web.Security.DataContext {
    public class DefaultRoleContext<TRole, TUserRole, TRoleKey> : IRoleContext<TRole, TUserRole, TRoleKey>
        where TRole : class
        where TUserRole : class
        where TRoleKey : struct {

        #region Variables

        private System.Reflection.MethodInfo containsMethod_string;
        private System.Reflection.MethodInfo containsMethod_TRoleKey;

        #endregion

        #region Properties

        public EFRoleProvider<TRole, TUserRole, TRoleKey> Provider { get; set; }
        internal Mapper.ClassHelper<TRole, Mapper.RoleColumnType, RoleColumnAttribute> RoleHelper { get; set; }
        internal Mapper.ClassHelper<TUserRole, Mapper.UserRoleColumnType, UserRoleColumnAttribute> UserRoleHelper { get; set; }

        public Type RoleType { get; set; }
        public bool RoleTableNoPrefix { get; private set; }
        public TableAttribute RoleTableName { get; private set; }

        public Type UserRoleType { get; set; }
        public bool UserRoleTableNoPrefix { get; private set; }
        public TableAttribute UserRoleTableName { get; private set; }

        public Type ApplicationType { get; set; }
        public bool ApplicationTableNoPrefix { get; private set; }
        public TableAttribute ApplicationTableName { get; private set; }

        #endregion

        #region Initialize And Constractor

        internal DefaultRoleContext(Mapper.ClassHelper<TRole, Mapper.RoleColumnType, RoleColumnAttribute> RoleHelper,
                                    Mapper.ClassHelper<TUserRole, Mapper.UserRoleColumnType, UserRoleColumnAttribute> UserRoleHelper) {
            this.RoleHelper = RoleHelper;
            this.UserRoleHelper = UserRoleHelper;
        }

        internal InternalRoleContext<TRole, TUserRole, TRoleKey> GetDatabase() {
            return new InternalRoleContext<TRole, TUserRole, TRoleKey>(this);
        }

        public void Initialize(EFRoleProvider<TRole, TUserRole, TRoleKey> Provider) {
            this.Provider = Provider;

            //Role Table
            var roleType = typeof(TRole);
            var tblNameAttr = roleType.GetCustomAttributes(typeof(TableAttribute), true);
            var noPrefixAttr = roleType.GetCustomAttributes(typeof(NoPrefixAttribute), true);

            RoleTableNoPrefix = noPrefixAttr != null && noPrefixAttr.Length > 0;
            var first = tblNameAttr.FirstOrDefault() as TableAttribute;

            if (first != null)
                RoleTableName = first;
            else
                RoleTableName = new TableAttribute(roleType.Name.Trim().Replace(" ", ""));

            if (!RoleTableNoPrefix && !string.IsNullOrWhiteSpace(Provider.TablePrefix)) {
                var schema = RoleTableName.Schema;
                RoleTableName = new TableAttribute(Provider.TablePrefix + RoleTableName.Name);
                if (schema != null) RoleTableName.Schema = schema;
            }
            RoleType = roleType;

            //User Role Table
            var userRoleType = typeof(TUserRole);
            tblNameAttr = userRoleType.GetCustomAttributes(typeof(TableAttribute), true);
            noPrefixAttr = userRoleType.GetCustomAttributes(typeof(NoPrefixAttribute), true);

            UserRoleTableNoPrefix = noPrefixAttr != null && noPrefixAttr.Length > 0;
            first = tblNameAttr.FirstOrDefault() as TableAttribute;

            if (first != null)
                UserRoleTableName = first;
            else
                UserRoleTableName = new TableAttribute(userRoleType.Name.Trim().Replace(" ", ""));

            if (!UserRoleTableNoPrefix && !string.IsNullOrWhiteSpace(Provider.TablePrefix)) {
                var schema = UserRoleTableName.Schema;
                UserRoleTableName = new TableAttribute(Provider.TablePrefix + UserRoleTableName.Name);
                if (schema != null) UserRoleTableName.Schema = schema;
            }
            UserRoleType = userRoleType;

            if (Provider.SupportApplication) {
                //Check application table
                var propAppType = RoleHelper.Get(Mapper.RoleColumnType.Application);
                var appType = propAppType.PropertyType;

                tblNameAttr = appType.GetCustomAttributes(typeof(TableAttribute), true);
                noPrefixAttr = appType.GetCustomAttributes(typeof(NoPrefixAttribute), true);

                ApplicationTableNoPrefix = noPrefixAttr != null && noPrefixAttr.Length > 0;
                first = tblNameAttr.FirstOrDefault() as TableAttribute;

                if (first != null)
                    ApplicationTableName = first;
                else
                    ApplicationTableName = new TableAttribute(appType.Name.Trim().Replace(" ", ""));

                if (!ApplicationTableNoPrefix && !string.IsNullOrWhiteSpace(Provider.TablePrefix)) {
                    var schema = ApplicationTableName.Schema;
                    ApplicationTableName = new TableAttribute(Provider.TablePrefix + ApplicationTableName.Name);
                    if (schema != null) ApplicationTableName.Schema = schema;
                }

                ApplicationType = appType;
            }
        }

        private DbQuery<T> RInclude<T>(DbSet<T> dbset) where T : class {
            DbQuery<T> current = dbset;
            foreach (var inc in RoleHelper.Includes)
                current = current.Include(inc);
            return current;
        }

        private DbQuery<T> URInclude<T>(DbSet<T> dbset) where T : class {
            DbQuery<T> current = dbset;
            foreach (var inc in UserRoleHelper.Includes)
                current = current.Include(inc);
            return current;
        }

        #endregion

        #region Role Expression(s)

        private Expression<Func<TRole, TValue>> GetRoleLambda<TValue>(Mapper.RoleColumnType columnType) {
            var param = Expression.Parameter(RoleType, "p");
            var memberAccess = RoleHelper.GetMemberAccess(param, columnType);

            return Expression.Lambda<Func<TRole, TValue>>(memberAccess, param);
        }

        private Expression<Func<TRole, bool>> GetRoleLambda(Mapper.RoleColumnType columnType, object value) {
            return GetRoleLambda(columnType, value, false);
        }

        private Expression<Func<TRole, bool>> GetRoleLambda(Mapper.RoleColumnType columnType, object value, bool appFilter) {
            var param = Expression.Parameter(RoleType, "p");
            var memberAccess = RoleHelper.GetMemberAccess(param, columnType);
            var content = Expression.Constant(value);
            var equal = Expression.Equal(memberAccess, content);
            //if (appFilter && Provider.SupportApplication) {
            //    var appMember = RoleHelper.GetMemberAccess(param, Mapper.RoleColumnType.Application);
            //    var appName = Expression.Property(appMember, "Name");
            //    var appCheck = Expression.Equal(appName, Expression.Constant(Provider.ApplicationName));

            //    return Expression.Lambda<Func<TRole, bool>>(Expression.And(equal, appCheck), param);
            //}

            return Expression.Lambda<Func<TRole, bool>>(equal, param);
        }

        private Expression<Func<TRole, bool>> GetRoleApplication() {
            var param = Expression.Parameter(RoleType, "p");
            var appMember = RoleHelper.GetMemberAccess(param, Mapper.RoleColumnType.Application);
            var appName = Expression.Property(appMember, "Name");
            var appCheck = Expression.Equal(appName, Expression.Constant(Provider.ApplicationName));

            return Expression.Lambda<Func<TRole, bool>>(appCheck, param);
        }

        private Expression<Func<TRole, bool>> GetRoleContainsLambda(Mapper.RoleColumnType columnType, IEnumerable<string> value, bool appFilter) {
            var param = Expression.Parameter(RoleType, "p");
            var memberAccess = RoleHelper.GetMemberAccess(param, columnType);
            if (containsMethod_string == null) {
                var type = typeof(System.Linq.Enumerable);
                containsMethod_string = type.GetMethods().First(f => f.Name.Equals("Contains")).MakeGenericMethod(typeof(string));
            }
            var call = Expression.Call(null, containsMethod_string, Expression.Constant(value), memberAccess);

            if (appFilter && Provider.SupportApplication) {
                var appMember = RoleHelper.GetMemberAccess(param, Mapper.RoleColumnType.Application);
                var appName = Expression.Property(appMember, "Name");
                var appCheck = Expression.Equal(appName, Expression.Constant(Provider.ApplicationName));

                return Expression.Lambda<Func<TRole, bool>>(Expression.And(call, appCheck), param);
            }

            return Expression.Lambda<Func<TRole, bool>>(call, param);
        }

        private Expression<Func<TRole, bool>> GetRoleContainsLambda(Mapper.RoleColumnType columnType, IEnumerable<TRoleKey> value, bool appFilter) {
            var param = Expression.Parameter(RoleType, "p");
            var memberAccess = RoleHelper.GetMemberAccess(param, columnType);
            if (containsMethod_TRoleKey == null) {
                var type = typeof(System.Linq.Enumerable);
                containsMethod_TRoleKey = type.GetMethods().First(f => f.Name.Equals("Contains")).MakeGenericMethod(typeof(TRoleKey));
            }
            var call = Expression.Call(null, containsMethod_TRoleKey, Expression.Constant(value), memberAccess);

            if (appFilter && Provider.SupportApplication) {
                var appMember = RoleHelper.GetMemberAccess(param, Mapper.RoleColumnType.Application);
                var appName = Expression.Property(appMember, "Name");
                var appCheck = Expression.Equal(appName, Expression.Constant(Provider.ApplicationName));

                return Expression.Lambda<Func<TRole, bool>>(Expression.And(call, appCheck), param);
            }

            return Expression.Lambda<Func<TRole, bool>>(call, param);
        }

        #endregion

        #region UserRole Expression(s)

        private Expression<Func<TUserRole, bool>> GetContainsJoinLambda(IEnumerable<TUserRole> Joins, bool Not) {
            var param = Expression.Parameter(UserRoleType, "p");

            if (Joins.Count() < 1)
                return Expression.Lambda<Func<TUserRole, bool>>(Expression.Constant(true), param);

            var memberAccess1 = UserRoleHelper.GetMemberAccess(param, Mapper.UserRoleColumnType.RoleID);
            var memberAccess2 = UserRoleHelper.GetMemberAccess(param, Mapper.UserRoleColumnType.UserID);

            var allEquals = Joins.Select(s => Expression.And(Expression.Equal(memberAccess1, Expression.Constant(Provider.UserRoleMapper.RoleID(s))),
                                                             Expression.Equal(memberAccess2, Expression.Constant(Provider.UserRoleMapper.UserID(s)))));
            Expression all = allEquals.Aggregate((o, n) => Expression.Or(o, n));
            if (Not) all = Expression.Not(all);

            return Expression.Lambda<Func<TUserRole, bool>>(all, param);
        }

        private Expression<Func<TUserRole, bool>> GetUserRoleLambda(Mapper.UserRoleColumnType columnType, object value) {
            var param = Expression.Parameter(UserRoleType, "p");
            var memberAccess = UserRoleHelper.GetMemberAccess(param, columnType);
            var content = Expression.Constant(value);
            var equal = Expression.Equal(memberAccess, content);

            return Expression.Lambda<Func<TUserRole, bool>>(equal, param);
        }

        private Expression<Func<TUserRole, TValue>> GetUserRoleLambda<TValue>(Mapper.UserRoleColumnType columnType) {
            var param = Expression.Parameter(UserRoleType, "p");
            var memberAccess = UserRoleHelper.GetMemberAccess(param, columnType);

            return Expression.Lambda<Func<TUserRole, TValue>>(memberAccess, param);
        }

        #endregion

        #region Create / Delete / Update

        public TRole CreateRole(TRole Role) {
            using (var db = GetDatabase()) {
                if (Provider.SupportApplication)
                    Provider.RoleMapper.Set(Role, Mapper.RoleColumnType.Application, db.GetApplication());

                try {
                    db.Roles.Add(Role);
                    db.SaveChanges();
                } catch (Exception ex) {
                    throw ex;
                }

                return Role;
            }
        }

        public TRole UpdateRole(TRole Role) {
            using (var db = GetDatabase()) {
                if (Provider.SupportApplication)
                    Provider.RoleMapper.Set(Role, Mapper.RoleColumnType.Application, db.GetApplication());

                db.Entry<TRole>(Role).State = System.Data.EntityState.Modified;
                db.SaveChanges();

                return Role;
            }
        }

        public TRole DeleteRole(TRole Role) {
            using (var db = GetDatabase()) {
                db.Entry<TRole>(Role).State = System.Data.EntityState.Deleted;
                db.SaveChanges();

                return Role;
            }
        }

        #endregion

        #region Get Role

        public TRole GetRole(TRoleKey RoleKey) {
            using (var db = GetDatabase())
                return RInclude(db.Roles).FirstOrDefault(GetRoleLambda(Mapper.RoleColumnType.RoleID, RoleKey, true));
        }

        public TRole GetRole(string RoleName) {
            using (var db = GetDatabase())
                return RInclude(db.Roles).FirstOrDefault(GetRoleLambda(Mapper.RoleColumnType.RoleName, RoleName, true));
        }

        public IEnumerable<TRole> GetRoles() {
            using (var db = GetDatabase())
                if (Provider.SupportApplication)
                    return RInclude(db.Roles).Where(GetRoleApplication()).ToList();
                else
                    return RInclude(db.Roles).ToList();
        }

        public IEnumerable<string> GetRoleNames() {
            using (var db = GetDatabase())
                if (Provider.SupportApplication)
                    return RInclude(db.Roles).Where(GetRoleApplication()).Select(GetRoleLambda<string>(Mapper.RoleColumnType.RoleName)).ToList();
                else
                    return RInclude(db.Roles).Select(GetRoleLambda<string>(Mapper.RoleColumnType.RoleName)).ToList();
        }

        public IEnumerable<string> GetRoleNames(object UserID) {
            using (var db = GetDatabase()) {
                var query1 = URInclude(db.UserRoles).Where(GetUserRoleLambda(Mapper.UserRoleColumnType.UserID, UserID))
                                                    .Select(GetUserRoleLambda<TRoleKey>(Mapper.UserRoleColumnType.RoleID));
                return RInclude(db.Roles).Where(GetRoleContainsLambda(Mapper.RoleColumnType.RoleID, query1, true))
                                         .Select(GetRoleLambda<string>(Mapper.RoleColumnType.RoleName)).ToList();
            }
        }

        public TRoleKey GetRoleID(string RoleName) {
            using (var db = GetDatabase())
                return RInclude(db.Roles).Where(GetRoleLambda(Mapper.RoleColumnType.RoleName, RoleName, true))
                                         .Select(GetRoleLambda<TRoleKey>(Mapper.RoleColumnType.RoleID)).FirstOrDefault();
        }

        public IEnumerable<TRoleKey> GetRoleID(string[] RoleName) {
            using (var db = GetDatabase())
                return RInclude(db.Roles).Where(GetRoleContainsLambda(Mapper.RoleColumnType.RoleName, RoleName, true))
                                         .Select(GetRoleLambda<TRoleKey>(Mapper.RoleColumnType.RoleID)).ToList();
        }

        #endregion

        #region Checking

        public bool IsRoleExist(string RoleName) {
            using (var db = GetDatabase())
                return db.Roles.Any(GetRoleLambda(Mapper.RoleColumnType.RoleName, RoleName, true));
        }

        public bool IsJoinExist(TUserRole Join) {
            using (var db = GetDatabase())
                return db.UserRoles.Any(GetContainsJoinLambda(new TUserRole[] { Join }, false));
        }

        #endregion

        #region Add / Remove User(s) to Role(s)

        public void RemoveUsersFromRoles(IEnumerable<TUserRole> Joins) {
            using (var db = GetDatabase()) {
                foreach (var item in db.UserRoles.Where(GetContainsJoinLambda(Joins, false)))
                    db.UserRoles.Remove(item);

                db.SaveChanges();
            }
        }

        public void AddUsersToRoles(IEnumerable<TUserRole> Joins) {
            using (var db = GetDatabase()) {
                var exist = db.UserRoles.Where(GetContainsJoinLambda(Joins, false));
                var notExist = Joins.AsQueryable().Where(GetContainsJoinLambda(exist, true));

                foreach (var item in notExist)
                    db.UserRoles.Add(item);

                db.SaveChanges();
            }
        }

        #endregion

        #region Get User(s) of RoleID

        public IEnumerable<TUserRole> GetUserInRole(TRoleKey RoleID) {
            using (var db = GetDatabase()) {
                return URInclude(db.UserRoles).Where(GetUserRoleLambda(Mapper.UserRoleColumnType.RoleID, RoleID)).ToList();
            }
        }

        #endregion

    }
}
