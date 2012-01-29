using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OmidID.Web.Security.Mapper {
    public interface IUserRoleMapper<TUserRole> : IMapper<TUserRole, UserRoleColumnType> {

        object RoleID(TUserRole role);
        IUserRoleMapper<TUserRole> RoleID(TUserRole role, object value);

        object UserID(TUserRole role);
        IUserRoleMapper<TUserRole> UserID(TUserRole role, object value);

    }
}
