using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Security;
using System.Configuration.Provider;
using System.ComponentModel.DataAnnotations;
using System.Collections;

namespace OmidID.Web.Security {
    public class EFRoleProvider<TRole, TUserRole, TRoleKey> : RoleProvider
        where TRole : class
        where TUserRole : class
        where TRoleKey : struct {

        #region Variables

        #endregion

        #region Properties

        public override string ApplicationName { get; set; }
        public string ConnectionString { get; private set; }
        public string TablePrefix { get; private set; }
        public string TableSchema { get; private set; }
        public string UserMembershipProvider { get; private set; }

        internal Mapper.ClassHelper<TRole, Mapper.RoleColumnType, RoleColumnAttribute> RoleHelper { get; private set; }
        public Mapper.IRoleMapper<TRole> RoleMapper { get; private set; }

        internal Mapper.ClassHelper<TUserRole, Mapper.UserRoleColumnType, UserRoleColumnAttribute> UserRoleHelper { get; private set; }
        public Mapper.IUserRoleMapper<TUserRole> UserRoleMapper { get; private set; }

        public bool SupportApplication { get; private set; }
        public bool SupportCreateOn { get; private set; }

        public Func<string, object> GetUserID { get; private set; }
        public Func<string[], IEnumerable> GetUserIDArray { get; private set; }
        public Func<object, string> GetUsername { get; set; }
        public Func<IEnumerable, string[]> GetUsernameArray { get; set; }

        public DataContext.IRoleContext<TRole, TUserRole, TRoleKey> Context { get; set; }

        #endregion

        #region Initialize

        public override void Initialize(string name, System.Collections.Specialized.NameValueCollection config) {
            if (config == null)
                throw new ArgumentNullException("config");
            if (String.IsNullOrEmpty(name))
                name = "EFMembership";

            config.Remove("description");
            config.Add("description", "Ado.net Entity Framework Membership Provider");
            base.Initialize(name, config);

            TablePrefix = config.GetStringValue("tablePrefix", "");
            UserMembershipProvider = config.GetStringValue("membershipProvider", string.Empty);

            MembershipProvider membership;
            if (string.IsNullOrWhiteSpace(UserMembershipProvider))
                membership = Membership.Provider;
            else
                membership = Membership.Providers[UserMembershipProvider];

            if (membership == null)
                throw new ProviderException("Membership_provider_not_found".Resource());

            var memType = membership.GetType();
            var memDefType = typeof(EFMembershipProvider<,>);

            if (memDefType.Module != memType.Module)
                throw new ProviderException("Membership_provider_must_be_type_of_EFMembershipProvider".Resource());
            if (!memDefType.Name.Equals(memType.Name))
                throw new ProviderException("Membership_provider_must_be_type_of_EFMembershipProvider".Resource());

            string temp = config["connectionStringName"];
            if (temp == null || temp.Length < 1)
                throw new ProviderException("Connection_name_not_specified".Resource());
            if (System.Configuration.ConfigurationManager.ConnectionStrings[temp] == null)
                throw new ProviderException("Connection_not_found".Resource());
            ConnectionString = temp;

            ApplicationName = config.GetStringValue("applicationName", "/");
            if (ApplicationName.Length > 256)
                throw new ProviderException("Provider_application_name_too_long".Resource());

            var type = typeof(TRole);
            var attr = type.GetCustomAttributes(typeof(EFDataMapperAttribute), false);

            RoleHelper = new Mapper.ClassHelper<TRole, Mapper.RoleColumnType, RoleColumnAttribute>(TablePrefix, TableSchema);
            if (attr != null && attr.Length > 0) {
                var mapperAttr = attr[0] as EFDataMapperAttribute;
                RoleMapper = Activator.CreateInstance(mapperAttr.MapperType) as Mapper.IRoleMapper<TRole>;

                if (RoleMapper == null)
                    throw new ProviderException("Reflection_can_not_cast_object".Resource(mapperAttr.MapperType.FullName,
                                                                                          typeof(Mapper.IUserMapper<TRole>).FullName));
            } else {
                RoleMapper = new Mapper.RoleAutoMapper<TRole>(RoleHelper);
            }

            if (!RoleHelper.Properties.ContainsKey(Mapper.RoleColumnType.RoleName))
                throw new ProviderException("Reflection_property_is_required".Resource("RoleName"));
            if (!RoleHelper.Properties.ContainsKey(Mapper.RoleColumnType.RoleID))
                throw new ProviderException("Reflection_property_is_required".Resource("RoleID"));

            type = typeof(TUserRole);
            attr = type.GetCustomAttributes(typeof(EFDataMapperAttribute), false);

            UserRoleHelper = new Mapper.ClassHelper<TUserRole, Mapper.UserRoleColumnType, UserRoleColumnAttribute>(TablePrefix, TableSchema);
            if (attr != null && attr.Length > 0) {
                var mapperAttr = attr[0] as EFDataMapperAttribute;
                UserRoleMapper = Activator.CreateInstance(mapperAttr.MapperType) as Mapper.IUserRoleMapper<TUserRole>;

                if (UserRoleMapper == null)
                    throw new ProviderException("Reflection_can_not_cast_object".Resource(mapperAttr.MapperType.FullName,
                                                                                          typeof(Mapper.IUserMapper<TUserRole>).FullName));
            } else {
                UserRoleMapper = new Mapper.UserRoleAutoMapper<TUserRole>(UserRoleHelper);
            }

            var generics = memType.GetGenericArguments();
            if (generics[1] != UserRoleHelper.Get(Mapper.UserRoleColumnType.UserID).PropertyType)
                throw new ProviderException("Reflection_UserID_type_is_not_match".Resource());

            if (!UserRoleHelper.Properties.ContainsKey(Mapper.UserRoleColumnType.UserID))
                throw new ProviderException("Reflection_property_is_required".Resource("UserID"));
            if (!UserRoleHelper.Properties.ContainsKey(Mapper.UserRoleColumnType.RoleID))
                throw new ProviderException("Reflection_property_is_required".Resource("RoleID"));

            SupportApplication = RoleHelper.Properties.ContainsKey(Security.Mapper.RoleColumnType.Application);
            SupportCreateOn = RoleHelper.Properties.ContainsKey(Security.Mapper.RoleColumnType.CreateOn);

            var getUserMethod1 = memType.GetMethod("GetUserID", new Type[] { typeof(string) }, null);
            var getUserMethod2 = memType.GetMethod("GetUserID", new Type[] { typeof(string[]) }, null);
            Func<string, object> getUser1 = username => getUserMethod1.Invoke(membership, new object[] { username });
            Func<string[], IEnumerable> getUser2 = username => getUserMethod2.Invoke(membership, new object[] { username }) as IEnumerable;

            var getUsernameMethod1 = memType.GetMethod("GetUsername", new Type[] { generics[1] }, null);
            var getUsernameMethod2 = memType.GetMethod("GetUsername", new Type[] { typeof(object).MakeArrayType() }, null);
            Func<object, string> getUsername1 = userid => getUsernameMethod1.Invoke(membership, new object[] { userid }) as string;
            Func<IEnumerable, string[]> getUsername2 = userid => getUsernameMethod2.Invoke(membership, new object[] { userid }) as string[];

            GetUserID = getUser1;
            GetUserIDArray = getUser2;

            GetUsername = getUsername1;
            GetUsernameArray = getUsername2;

            attr = type.GetCustomAttributes(typeof(EFDataContextAttribute), false);
            if (attr != null && attr.Length > 0) {
                var contextAttr = attr[0] as EFDataContextAttribute;
                Context = Activator.CreateInstance(contextAttr.ContextType) as DataContext.IRoleContext<TRole, TUserRole, TRoleKey>;
                if (Context == null) {
                    throw new ProviderException("Reflection_can_not_cast_object".Resource(contextAttr.ContextType.FullName,
                                                                                          typeof(DataContext.IRoleContext<TRole, TUserRole, TRoleKey>).FullName));
                }
            } else {
                Context = new DataContext.DefaultRoleContext<TRole, TUserRole, TRoleKey>(RoleHelper, UserRoleHelper);
            }
            Context.Initialize(this);
        }

        #endregion

        #region Create Role

        public override void CreateRole(string roleName) {
            if (Context.IsRoleExist(roleName)) return;

            var role = RoleHelper.New();
            RoleMapper.RoleName(role, roleName);
            if (SupportCreateOn)
                RoleMapper.CreateOn(role, DateTime.UtcNow);

            Context.CreateRole(role);
        }

        #endregion

        #region Delete Role

        public override bool DeleteRole(string roleName, bool throwOnPopulatedRole) {
            try {
                var role = Context.GetRole(roleName);
                Context.DeleteRole(role);

                return true;
            } catch (Exception ex) {
                if (throwOnPopulatedRole)
                    throw ex;
                else
                    return false;
            }
        }

        #endregion

        #region List Of Roles

        public override string[] GetAllRoles() {
            var result = Context.GetRoleNames();
            return result is string[] ? (string[])result : result.ToArray();
        }

        #endregion

        #region Add And Remove From Role

        public override void AddUsersToRoles(string[] usernames, string[] roleNames) {
            var userid = GetUserIDArray(usernames);
            var roleId = Context.GetRoleID(roleNames);
            var list = new List<TUserRole>();
            foreach (var rid in roleId)
                foreach (var uid in userid) {
                    var rjoin = UserRoleHelper.New();
                    UserRoleMapper.UserID(rjoin, uid)
                                  .RoleID(rjoin, rid);
                    list.Add(rjoin);
                }

            Context.AddUsersToRoles(list);
        }

        public override void RemoveUsersFromRoles(string[] usernames, string[] roleNames) {
            var userid = GetUserIDArray(usernames);
            var roleId = Context.GetRoleID(roleNames);
            var list = new List<TUserRole>();
            foreach (var rid in roleId)
                foreach (var uid in userid) {
                    var rjoin = UserRoleHelper.New();
                    UserRoleMapper.UserID(rjoin, uid)
                                  .RoleID(rjoin, rid);
                    list.Add(rjoin);
                }

            Context.RemoveUsersFromRoles(list);
        }

        #endregion

        #region Exist Check

        public override bool RoleExists(string roleName) {
            return Context.IsRoleExist(roleName);
        }

        public override bool IsUserInRole(string username, string roleName) {
            var userid = GetUserID(username);
            var roleid = Context.GetRoleID(roleName);
            var join = UserRoleHelper.New();

            UserRoleMapper.UserID(join, userid)
                          .RoleID(join, roleid);

            return Context.IsJoinExist(join);
        }

        #endregion

        #region Search User

        public override string[] FindUsersInRole(string roleName, string usernameToMatch) {
            throw new NotImplementedException();
        }

        #endregion

        #region User In Role

        public override string[] GetRolesForUser(string username) {
            var userid = GetUserID(username);
            var result = Context.GetRoleNames(userid);

            return result is string[] ? (string[])result : result.ToArray();
        }

        public override string[] GetUsersInRole(string roleName) {
            var roleid = Context.GetRoleID(roleName);
            var join = Context.GetUserInRole(roleid).Select(s => UserRoleMapper.UserID(s));

            return GetUsernameArray(join.ToArray());
        }

        #endregion

    }
}
