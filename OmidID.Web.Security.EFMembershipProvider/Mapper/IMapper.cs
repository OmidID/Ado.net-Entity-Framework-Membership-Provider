using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OmidID.Web.Security.Mapper {
    public interface IMapper<TEntity, TEnum> {

        T Get<T>(TEntity Entity, TEnum ColumnType);
        object Get(TEntity Entity, TEnum ColumnType);
        bool Set<TValue>(TEntity Entity, TEnum ColumnType, TValue Value);

    }
}
