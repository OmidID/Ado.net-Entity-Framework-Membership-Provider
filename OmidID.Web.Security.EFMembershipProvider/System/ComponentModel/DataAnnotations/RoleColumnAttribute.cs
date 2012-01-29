using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OmidID.Web.Security.Mapper;

namespace System.ComponentModel.DataAnnotations {
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class RoleColumnAttribute : Attribute, IColumnAttribute<RoleColumnType> {
        public RoleColumnAttribute(RoleColumnType Type)
        {
            ColumnType = Type;
        }

        public RoleColumnType ColumnType
        {
            get;
            private set;
        }

    }
}
