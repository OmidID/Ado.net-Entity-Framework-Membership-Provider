using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Linq.Expressions;

namespace OmidID.Web.Security.DataContext {

    public class DefaultUserContext
#if USE_WEBMATRIX
<TUser, TOAuthMembership, TKey> : IUserContext<TUser, TOAuthMembership, TKey>
        where TUser : class
        where TOAuthMembership : class
        where TKey : struct {
#else
<TUser, TKey> : IUserContext<TUser, TKey>
        where TUser : class
        where TKey : struct {
#endif


        #region Variables

        private System.Reflection.MethodInfo containsMethod_string;
        private System.Reflection.MethodInfo containsMethod_TKey;

        #endregion

        #region Properties
#if USE_WEBMATRIX
        public EFMembershipProvider<TUser, TOAuthMembership, TKey> Provider { get; set; }
        internal Mapper.ClassHelper<TUser, Mapper.UserColumnType, UserColumnAttribute> Helper { get; set; }
        internal Mapper.ClassHelper<TOAuthMembership, Mapper.OAuthMembershipColumnType, OAuthMembershipAttribute> Helper_OAuth { get; set; }
        public Type OAuthType { get; set; }
#else
        public EFMembershipProvider<TUser, TKey> Provider { get; set; }
        internal Mapper.ClassHelper<TUser, Mapper.UserColumnType, UserColumnAttribute> Helper { get; set; }

#endif
        public Type UserType { get; set; }
        public bool UserTableNoPrefix { get; private set; }
        public TableAttribute UserTableName { get; private set; }

        public Type ApplicationType { get; set; }
        public bool ApplicationTableNoPrefix { get; private set; }
        public TableAttribute ApplicationTableName { get; private set; }

        #endregion

        #region Initialize And Constractor

#if USE_WEBMATRIX
        internal InternalUserContext<TUser, TOAuthMembership, TKey> GetDatabase() {
            return new InternalUserContext<TUser, TOAuthMembership, TKey>(this);
        }
#else
        internal InternalUserContext<TUser, TKey> GetDatabase() {
            return new InternalUserContext<TUser, TKey>(this);
        }
#endif

        internal DefaultUserContext(Mapper.ClassHelper<TUser, Mapper.UserColumnType, UserColumnAttribute> Helper
#if USE_WEBMATRIX
                                    ,Mapper.ClassHelper<TOAuthMembership, Mapper.OAuthMembershipColumnType, OAuthMembershipAttribute> Helper_OAuth
#endif
) {
            this.Helper = Helper;
#if USE_WEBMATRIX
            this.Helper_OAuth = Helper_OAuth;
#endif
        }

        public void Initialize(
#if USE_WEBMATRIX
            EFMembershipProvider<TUser, TOAuthMembership, TKey> Provider
#else
            EFMembershipProvider<TUser, TKey> Provider
#endif

) {
            this.Provider = Provider;

            var userType = typeof(TUser);
#if USE_WEBMATRIX
            OAuthType = typeof(TOAuthMembership);
#endif
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

#if USE_WEBMATRIX

        private DbQuery<T> Include_OAuth<T>(DbSet<T> dbset) where T : class {
            DbQuery<T> current = dbset;
            foreach (var inc in Helper_OAuth.Includes)
                current = current.Include(inc);
            return current;
        }

#endif

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

#if USE_WEBMATRIX

        private Expression<Func<TOAuthMembership, bool>> GetOAuthLambda(Mapper.OAuthMembershipColumnType columnType, object value) {
            var param = Expression.Parameter(OAuthType, "p");
            var memberAccess = Helper_OAuth.GetMemberAccess(param, columnType);
            var content = Expression.Constant(value);
            var equal = Expression.Equal(memberAccess, content);

            return Expression.Lambda<Func<TOAuthMembership, bool>>(equal, param);
        }

        private Expression<Func<TOAuthMembership, bool>> GetOAuthDualLambda(Mapper.OAuthMembershipColumnType columnType1, object value1, Mapper.OAuthMembershipColumnType columnType2, object value2) {
            var param = Expression.Parameter(OAuthType, "p");
            var memberAccess1 = Helper_OAuth.GetMemberAccess(param, columnType1);
            var memberAccess2 = Helper_OAuth.GetMemberAccess(param, columnType2);
            var content1 = Expression.Constant(value1);
            var content2 = Expression.Constant(value2);
            var equal1 = Expression.Equal(memberAccess1, content1);
            var equal2 = Expression.Equal(memberAccess2, content2);

            return Expression.Lambda<Func<TOAuthMembership, bool>>(Expression.And(equal1, equal2), param);
        }

#endif

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
                db.Entry<TUser>(Entity).State = EntityState.Modified;
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
                db.Entry<TUser>(Entity).State = EntityState.Deleted;
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

#if USE_WEBMATRIX

        public TUser GetByConfirmationCode(string confirmToken) {
            using (var db = GetDatabase())
                return Include(db.Users).FirstOrDefault(GetUserLambda(Mapper.UserColumnType.ConfirmationCode, confirmToken, true));
        }

        public TUser GetByConfirmationCode(string username, string confirmToken) {
            using (var db = GetDatabase())
                return Include(db.Users).Where(GetUserLambda(Mapper.UserColumnType.Username, username, true))
                                        .FirstOrDefault(GetUserLambda(Mapper.UserColumnType.ConfirmationCode, confirmToken, true));
        }

        public TUser GetUserByPasswordToken(string Token) {
            using (var db = GetDatabase())
                return Include(db.Users).FirstOrDefault(GetUserLambda(Mapper.UserColumnType.PasswordValidationToken, Token, true));
        }

        public IEnumerable<TOAuthMembership> GetAccountsForUser(string username) {
            using (var db = GetDatabase()) {
                var user = Include(db.Users).FirstOrDefault(GetUserLambda(Mapper.UserColumnType.Username, username, true));
                return Include_OAuth(db.OAuthMembership).Where(GetOAuthLambda(Mapper.OAuthMembershipColumnType.UserID, Provider.Mapper.UserID(user))).ToArray();
            }
        }


        public TOAuthMembership GetOAuthMembership(string provider, string providerUserID) {
            using (var db = GetDatabase())
                return Include_OAuth(db.OAuthMembership).FirstOrDefault(GetOAuthDualLambda(Mapper.OAuthMembershipColumnType.ProviderName, provider, Mapper.OAuthMembershipColumnType.ProviderToken, providerUserID));
        }

        public TOAuthMembership AddOAuth(TOAuthMembership Entity) {
            using (var db = GetDatabase())
                try {
                    db.OAuthMembership.Add(Entity);
                    db.SaveChanges();
                } catch (Exception ex) {
                    throw ex;
                }
            return Entity;
        }

        public TOAuthMembership UpdateOAuth(TOAuthMembership Entity) {
            using (var db = GetDatabase())
                try {
                    db.Entry(Entity).State = EntityState.Modified;
                    db.SaveChanges();
                } catch (Exception ex) {
                    throw ex;
                }
            return Entity;
        }

        public TOAuthMembership DeleteOAuth(TOAuthMembership Entity) {
            using (var db = GetDatabase())
                try {
                    db.Entry(Entity).State = EntityState.Deleted;
                    db.SaveChanges();
                } catch (Exception ex) {
                    throw ex;
                }
            return Entity;
        }

#endif

    }
}
