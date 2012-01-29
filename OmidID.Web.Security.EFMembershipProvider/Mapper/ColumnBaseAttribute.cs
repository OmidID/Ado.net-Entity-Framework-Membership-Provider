using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OmidID.Web.Security.Mapper {
    public interface IColumnAttribute<TColumn>
        where TColumn : struct {

        TColumn ColumnType { get; }

    }
}
