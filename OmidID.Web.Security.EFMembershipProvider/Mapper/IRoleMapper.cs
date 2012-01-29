using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OmidID.Web.Security.Mapper {
    public interface IRoleMapper<TRole> : IMapper<TRole, RoleColumnType> {

        object RoleID(TRole role);
        IRoleMapper<TRole> RoleID(TRole role, object value);

        string RoleName(TRole role);
        IRoleMapper<TRole> RoleName(TRole role, string value);

        DateTime CreateOn(TRole role);
        IRoleMapper<TRole> CreateOn(TRole role, DateTime value);

    }
}
