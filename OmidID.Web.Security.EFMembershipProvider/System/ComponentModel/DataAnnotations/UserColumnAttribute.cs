using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OmidID.Web.Security.Mapper;

namespace System.ComponentModel.DataAnnotations {
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class UserColumnAttribute : Attribute, IColumnAttribute<UserColumnType> {
        public UserColumnAttribute(UserColumnType Type){
            ColumnType = Type;
        }

        public UserColumnType ColumnType {
            get;
            private set;
        }

    }
}
