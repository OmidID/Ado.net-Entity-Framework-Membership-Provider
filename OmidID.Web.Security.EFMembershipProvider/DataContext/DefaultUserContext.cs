using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity;
using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;
using System.Data.Entity.Infrastructure;

namespace OmidID.Web.Security.DataContext {

    public class DefaultUserContext<TUser, TKey> : IUserContext<TUser, TKey>
        where TUser : class
        where TKey : struct {

        #region Variables

        private System.Reflection.MethodInfo containsMethod_string;
        private System.Reflection.MethodInfo containsMethod_TKey;

        #endregion

        #region Properties

        public EFMembershipProvider<TUser, TKey> Provider { get; set; }
        internal Mapper.ClassHelper<TUser, Mapper.UserColumnType, UserColumnAttribute> Helper { get; set; }

        public Type UserType { get; set; }
        public bool UserTableNoPrefix { get; private set; }
        public TableAttribute UserTableName { get; private set; }

        public Type ApplicationType { get; set; }
        public bool ApplicationTableNoPrefix { get; private set; }
        public TableAttribute ApplicationTableName { get; private set; }

        #endregion

        #region Initialize And Constractor

        internal InternalUserContext<TUser, TKey> GetDatabase() {
            return new InternalUserContext<TUser, TKey>(this);
        }

        internal DefaultUserContext(Mapper.ClassHelper<TUser, Mapper.UserColumnType, UserColumnAttribute> Helper) {
            this.Helper = Helper;
        }

        public void Initialize(EFMembershipProvider<TUser, TKey> Provider) {
            this.Provider = Provider;

            var userType = typeof(TUser);
            var tblNameAttr = userType.GetCustomAttributes(typeof(TableAttribute), true);
            var noPrefixAttr = userType.GetCustomAttributes(typeof(NoPrefixAttribute), true);

            UserTableNoPrefix = noPrefixAttr != null && noPrefixAttr.Length > 0;
            var first = tblNameAttr.FirstOrDefault() as TableAttribute;

            if (first != null)
                UserTableName = first;
            else
                UserTableName = new TableAttribute(userType.Name.Trim().Replace(" ", ""));

            if (!UserTableNoPrefix && !string.IsNullOrWhiteSpace(Provider.TablePrefix)) {
                var schema = UserTableName.Schema;
                UserTableName = new TableAttribute(Provider.TablePrefix + UserTableName.Name);
                if (schema != null) UserTableName.Schema = schema;
            }

            UserType = userType;

            if (Provider.SupportApplication) {
                var propAppType = Helper.Get(Mapper.UserColumnType.Application);
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

        private DbQuery<T> Include<T>(DbSet<T> dbset) where T : class {
            DbQuery<T> current = dbset;
            foreach (var inc in Helper.Includes)
                current = current.Include(inc);
            return current;
        }

        #endregion

        #region Expression(s)

        private Expression<Func<TUser, TValue>> GetUserLambda<TValue>(Mapper.UserColumnType columnType) {
            var param = Expression.Parameter(UserType, "p");
            var memberAccess = Helper.GetMemberAccess(param, columnType);

            return Expression.Lambda<Func<TUser, TValue>>(memberAccess, param);
        }

        private Expression<Func<TUser, bool>> GetUserLambda(Mapper.UserColumnType columnType, object value) {
            return GetUserLambda(columnType, value, false);
        }

        private Expression<Func<TUser, bool>> GetUserLambda(Mapper.UserColumnType columnType, object value, bool appFilter) {
            var param = Expression.Parameter(UserType, "p");
            var memberAccess = Helper.GetMemberAccess(param, columnType);
            var content = Expression.Constant(value);
            var equal = Expression.Equal(memberAccess, content);

            if (appFilter && Provider.SupportApplication) {
                var appMember = Helper.GetMemberAccess(param, Mapper.UserColumnType.Application);
                var appName = Expression.Property(appMember, "Name");
                var appCheck = Expression.Equal(appName, Expression.Constant(Provider.ApplicationName));

                return Expression.Lambda<Func<TUser, bool>>(Expression.And(equal, appCheck), param);
            }

            return Expression.Lambda<Func<TUser, bool>>(equal, param);
        }

        private Expression<Func<TUser, bool>> GetUserContainsLambda(Mapper.UserColumnType columnType, string value, bool appFilter) {
            var param = Expression.Parameter(UserType, "p");
            var memberAccess = Helper.GetMemberAccess(param, columnType);
            var type = typeof(string);
            var meth = type.GetMethod("Contains");
            var call = Expression.Call(memberAccess, meth, Expression.Constant(value));

            if (appFilter && Provider.SupportApplication) {
                var appMember = Helper.GetMemberAccess(param, Mapper.UserColumnType.Application);
                var appName = Expression.Property(appMember, "Name");
                var appCheck = Expression.Equal(appName, Expression.Constant(Provider.ApplicationName));

                return Expression.Lambda<Func<TUser, bool>>(Expression.And(call, appCheck), param);
            }

            return Expression.Lambda<Func<TUser, bool>>(call, param);
        }

        private Expression<Func<TUser, bool>> GetUserContainsLambda(Mapper.UserColumnType columnType, string[] value, bool appFilter) {
            var param = Expression.Parameter(UserType, "p");
            var memberAccess = Helper.GetMemberAccess(param, columnType);
            if (containsMethod_string == null) {
                var type = typeof(System.Linq.Enumerable);
                containsMethod_string = type.GetMethods().First(f => f.Name.Equals("Contains")).MakeGenericMethod(typeof(string));
            }
            var call = Expression.Call(null, containsMethod_string, Expression.Constant(value), memberAccess);

            if (appFilter && Provider.SupportApplication) {
                var appMember = Helper.GetMemberAccess(param, Mapper.UserColumnType.Application);
                var appName = Expression.Property(appMember, "Name");
                var appCheck = Expression.Equal(appName, Expression.Constant(Provider.ApplicationName));

                return Expression.Lambda<Func<TUser, bool>>(Expression.And(call, appCheck), param);
            }

            return Expression.Lambda<Func<TUser, bool>>(call, param);
        }

        private Expression<Func<TUser, bool>> GetUserContainsLambda(Mapper.UserColumnType columnType, TKey[] value, bool appFilter) {
            var param = Expression.Parameter(UserType, "p");
            var memberAccess = Helper.GetMemberAccess(param, columnType);
            if (containsMethod_TKey == null) {
                var type = typeof(System.Linq.Enumerable);
                containsMethod_TKey = type.GetMethods().First(f => f.Name.Equals("Contains")).MakeGenericMethod(typeof(TKey));
            }
            var call = Expression.Call(null, containsMethod_TKey, Expression.Constant(value), memberAccess);

            if (appFilter && Provider.SupportApplication) {
                var appMember = Helper.GetMemberAccess(param, Mapper.UserColumnType.Application);
                var appName = Expression.Property(appMember, "Name");
                var appCheck = Expression.Equal(appName, Expression.Constant(Provider.ApplicationName));

                return Expression.Lambda<Func<TUser, bool>>(Expression.And(call, appCheck), param);
            }

            return Expression.Lambda<Func<TUser, bool>>(call, param);
        }

        private Expression<Func<TUser, bool>> GetUserGreaterThanLambda(Mapper.UserColumnType columnType, object value, bool appFilter) {
            var param = Expression.Parameter(UserType, "p");
            var memberAccess = Helper.GetMemberAccess(param, columnType);
            var content = Expression.Constant(value);
            var equal = Expression.GreaterThan(memberAccess, content);

            if (appFilter && Provider.SupportApplication) {
                var appMember = Helper.GetMemberAccess(param, Mapper.UserColumnType.Application);
                var appName = Expression.Property(appMember, "Name");
                var appCheck = Expression.Equal(appName, Expression.Constant(Provider.ApplicationName));

                return Expression.Lambda<Func<TUser, bool>>(Expression.And(equal, appCheck), param);
            }

            return Expression.Lambda<Func<TUser, bool>>(equal, param);
        }

        #endregion

        #region Add / Delete / Update

        public TUser Add(TUser Entity) {
            using (var db = GetDatabase()) {
                if (Provider.SupportApplication)
                    Provider.Mapper.Set(Entity, Mapper.UserColumnType.Application, db.GetApplication());

                try {
                    db.Users.Add(Entity);
                    db.SaveChanges();
                } catch (Exception ex) {
                    throw ex;
                }

                return Entity;
            }
        }

        public TUser Update(TUser Entity) {
            using (var db = GetDatabase()) {
                db.Entry<TUser>(Entity).State = System.Data.EntityState.Modified;
                if (Provider.SupportApplication)
                    Provider.Mapper.Set(Entity, Mapper.UserColumnType.Application, db.GetApplication());

                try {
                    db.SaveChanges();
                } catch (Exception ex) {
                    throw ex;
                }

                return Entity;
            }
        }

        public TUser Delete(TUser Entity) {
            using (var db = GetDatabase()) {
                db.Entry<TUser>(Entity).State = System.Data.EntityState.Deleted;
                db.SaveChanges();

                return Entity;
            }
        }

        #endregion

        #region Get User(s)

        public TUser GetUser(TKey Key) {
            using (var db = GetDatabase())
                return Include(db.Users).FirstOrDefault(GetUserLambda(Mapper.UserColumnType.UserID, Key, true));
        }

        public TUser GetUser(string Username) {
            using (var db = GetDatabase())
                return Include(db.Users).FirstOrDefault(GetUserLambda(Mapper.UserColumnType.Username, Username, true));
        }

        public TUser GetUserByEmail(string Email) {
            using (var db = GetDatabase())
                return Include(db.Users).FirstOrDefault(GetUserLambda(Mapper.UserColumnType.Email, Email, true));
        }

        public IEnumerable<TUser> UserList(int PageSize, int PageIndex, out int TotalRecords) {
            using (var db = GetDatabase()) {
                var query = Include(db.Users).OrderBy(GetUserLambda<TKey>(Mapper.UserColumnType.UserID));
                TotalRecords = query.Count();
                return query.Skip(PageIndex * PageSize).Take(PageSize).ToList();
            }
        }

        public IEnumerable<TUser> FindByUsername(string Username, int PageSize, int PageIndex, out int TotalRecords) {
            using (var db = GetDatabase()) {
                var query = Include(db.Users).Where(GetUserContainsLambda(Mapper.UserColumnType.Username, Username, true))
                                             .OrderBy(GetUserLambda<TKey>(Mapper.UserColumnType.UserID));

                TotalRecords = query.Count();
                return query.Skip(PageIndex * PageSize).Take(PageSize).ToList();
            }
        }

        public IEnumerable<TUser> FindByEmail(string EmailAddress, int PageSize, int PageIndex, out int TotalRecords) {
            using (var db = GetDatabase()) {
                var query = Include(db.Users).Where(GetUserContainsLambda(Mapper.UserColumnType.Email, EmailAddress, true))
                                             .OrderBy(GetUserLambda<TKey>(Mapper.UserColumnType.UserID));

                TotalRecords = query.Count();
                return query.Skip(PageIndex * PageSize).Take(PageSize).ToList();
            }
        }

        public int NumberOfOnlineUsers(int TimeoutInMinute) {
            using (var db = GetDatabase()) {
                var utc = DateTime.Now.AddMinutes(-TimeoutInMinute);
                var query = db.Users.Where(GetUserGreaterThanLambda(Mapper.UserColumnType.LastActivityDate, utc, true));

                return query.Count();
            }
        }

        #endregion

        #region Extended Method(s) for Other Provider (Role / Profile)

        public TKey GetUserID(string Username) {
            using (var db = GetDatabase())
                return db.Users.Where(GetUserLambda(Mapper.UserColumnType.Username, Username, true))
                               .Select(GetUserLambda<TKey>(Mapper.UserColumnType.UserID)).FirstOrDefault();
        }

        public TKey[] GetUserID(string[] Username) {
            using (var db = GetDatabase())
                return db.Users.Where(GetUserContainsLambda(Mapper.UserColumnType.Username, Username, true))
                               .Select(GetUserLambda<TKey>(Mapper.UserColumnType.UserID)).ToArray();
        }

        public string GetUsername(TKey UserID) {
            using (var db = GetDatabase())
                return db.Users.Where(GetUserLambda(Mapper.UserColumnType.UserID, UserID, true))
                               .Select(GetUserLambda<string>(Mapper.UserColumnType.Username)).FirstOrDefault();
        }

        public string[] GetUsername(TKey[] UserID) {
            using (var db = GetDatabase())
                return db.Users.Where(GetUserContainsLambda(Mapper.UserColumnType.UserID, UserID, true))
                               .Select(GetUserLambda<string>(Mapper.UserColumnType.Username)).ToArray();
        }

        #endregion

    }
}
