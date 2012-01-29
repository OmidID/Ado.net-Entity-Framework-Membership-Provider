using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.ComponentModel.DataAnnotations {

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class EFDataMapperAttribute : Attribute {

        public Type MapperType { get; set; }
        public EFDataMapperAttribute(Type Mapper) {
            MapperType = Mapper;
        }

    }
}
