using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.ComponentModel.DataAnnotations {

    /// <summary>
    /// this attribute help you, if you want to have multiple table for your custom provider
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class JoinItAttribute : Attribute { }
}
