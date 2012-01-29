using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity;

namespace OmidID.Web.Security.DataContext {
    public interface IRoleContext<TRole, TUserRole, TRoleKey>
        where TRole : class
        where TUserRole : class
        where TRoleKey : struct {

        void Initialize(EFRoleProvider<TRole, TUserRole, TRoleKey> Provider);

        TRole GetRole(TRoleKey RoleKey);
        TRole GetRole(string RoleName);
        TRole CreateRole(TRole Role);
        TRole UpdateRole(TRole Role);
        TRole DeleteRole(TRole Role);
        IEnumerable<TRole> GetRoles();
        IEnumerable<string> GetRoleNames();
        IEnumerable<string> GetRoleNames(object UserID);
        TRoleKey GetRoleID(string RoleName);
        IEnumerable<TRoleKey> GetRoleID(string[] RoleName);
        bool IsRoleExist(string RoleName);

        IEnumerable<TUserRole> GetUserInRole(TRoleKey RoleID);
        void RemoveUsersFromRoles(IEnumerable<TUserRole> Joins);
        void AddUsersToRoles(IEnumerable<TUserRole> Joins);
        bool IsJoinExist(TUserRole Join);

    }
}
