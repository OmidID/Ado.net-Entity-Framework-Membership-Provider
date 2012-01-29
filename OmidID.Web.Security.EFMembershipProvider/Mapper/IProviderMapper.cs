using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OmidID.Web.Security.Mapper {
    public interface IProviderMapper<TFrom, TTo, TEnum> : IMapper<TFrom, TEnum> {

        TTo To(string ProviderName, TFrom From);
        TFrom To(TTo From);
        TFrom To(TFrom To, TTo From);

    }
}
