using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.ComponentModel.DataAnnotations {
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class NoPrefixAttribute : Attribute { }
}
