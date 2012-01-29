using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OmidID.Web.Security.Mapper;

namespace System.ComponentModel.DataAnnotations {
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class UserRoleColumnAttribute : Attribute , IColumnAttribute<UserRoleColumnType> {
        public UserRoleColumnAttribute(UserRoleColumnType Type){
            ColumnType = Type;
        }

        public UserRoleColumnType ColumnType {
            get;
            private set;
        }

    }
}
