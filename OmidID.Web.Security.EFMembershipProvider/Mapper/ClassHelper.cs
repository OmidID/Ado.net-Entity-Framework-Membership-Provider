using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Linq.Expressions;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OmidID.Web.Security.Mapper {
    internal class ClassHelper<T, TEnum, TAttr>
        where T : class
        where TEnum : struct
        where TAttr : IColumnAttribute<TEnum> {
        private string Schema;
        private string Prefix;

        public class PropRegister {
            public PropRegister(List<PropertyInfo> Parents) {
                if (Parents == null) IsRootProperty = true;
                else {
                    IsRootProperty = false;
                    this.Parents = Parents;
                }
            }

            public TEnum ColumnType { get; internal set; }
            public PropertyInfo Property { get; internal set; }
            public bool IsAttributeSet { get; internal set; }
            public bool IsRootProperty { get; internal set; }
            public List<PropertyInfo> Parents { get; internal set; }
        }

        public Type ClassType { get; private set; }
        public Dictionary<TEnum, PropRegister> Properties { get; private set; }
        public List<List<PropertyInfo>> JoinProperties { get; private set; }
        public Dictionary<Type, TableAttribute> TableNames { get; private set; }
        public List<string> Includes { get; private set; }

        public ClassHelper(string Prefix, string Schema) {
            this.Prefix = Prefix;
            this.Schema = Schema;

            ClassType = typeof(T);
            Properties = new Dictionary<TEnum, PropRegister>();
            JoinProperties = new List<List<PropertyInfo>>();
            TableNames = new Dictionary<Type, TableAttribute>();
            Includes = new List<string>();

            HirericalClassReader(ClassType, null);
        }

        private void HirericalClassReader(Type CheckType, List<PropertyInfo> Parents) {
            var props = CheckType.GetProperties();
            var attrType = typeof(TAttr);
            var joinAttrType = typeof(System.ComponentModel.DataAnnotations.JoinItAttribute);
            var tableAttrType = typeof(TableAttribute);
            var noPrefixAttrType = typeof(System.ComponentModel.DataAnnotations.NoPrefixAttribute);

            var names = CheckType.GetCustomAttributes(tableAttrType, true);
            var noprefix = CheckType.GetCustomAttributes(noPrefixAttrType, true);
            TableAttribute TableName;
            var TableNoPrefix = noprefix != null && noprefix.Length > 0;
            if (names != null && names.Length > 0)
                TableName = names[0] as TableAttribute;
            else
                TableName = new TableAttribute(CheckType.Name.Trim().Replace(" ", "_"));

            if (!string.IsNullOrEmpty(Schema))
                TableName.Schema = Schema;

            if (!TableNoPrefix && !string.IsNullOrWhiteSpace(Prefix)) {
                var schema = TableName.Schema;
                TableName = new TableAttribute(Prefix + TableName.Name);
                if (schema != null) TableName.Schema = schema;
            }
            TableNames[CheckType] = TableName;

            bool havechild = false;
            foreach (var prop in props) {
                var attrs = prop.GetCustomAttributes(attrType, true);
                if (attrs != null && attrs.Length > 0) {
                    var attr = (TAttr)attrs[0];
                    if (Properties.ContainsKey(attr.ColumnType)) {
                        if (!Properties[attr.ColumnType].IsAttributeSet) {
                            Properties[attr.ColumnType] = new PropRegister(Parents) { Property = prop, IsAttributeSet = true, ColumnType = attr.ColumnType };
                        }
                    } else {
                        Properties.Add(attr.ColumnType, new PropRegister(Parents) { Property = prop, IsAttributeSet = true, ColumnType = attr.ColumnType });
                    }
                } else {
                    TEnum cType;
                    if (Enum.TryParse<TEnum>(prop.Name, true, out cType)) {
                        if (Properties.ContainsKey(cType)) {
                            if (!Properties[cType].IsAttributeSet) {
                                Properties[cType] = new PropRegister(Parents) { Property = prop, IsAttributeSet = false, ColumnType = cType };
                            }
                        } else {
                            Properties.Add(cType, new PropRegister(Parents) { Property = prop, IsAttributeSet = false, ColumnType = cType });
                        }
                    }
                }

                var joinAttrs = prop.GetCustomAttributes(joinAttrType, true);
                if (joinAttrs != null && joinAttrs.Length > 0) {
                    var parents = Parents != null ? new List<PropertyInfo>(Parents) : new List<PropertyInfo>();
                    parents.Add(prop);
                    HirericalClassReader(prop.PropertyType, parents);

                    JoinProperties.Add(parents);
                    havechild = true;
                }
            }

            if (Parents != null && !havechild)
                Includes.Add(string.Join(".", Parents.Select(s => s.Name).ToArray()));
        }

        public Expression GetMemberAccess(ParameterExpression Parameter, TEnum ColumnType) {
            if (Properties.ContainsKey(ColumnType)) {
                var prop = Properties[ColumnType];
                if (prop == null) return null;
                if (prop.IsRootProperty)
                    return Expression.Property(Parameter, prop.Property);

                Expression exp = Parameter;
                foreach (var parent in prop.Parents)
                    exp = Expression.Property(exp, parent);

                return Expression.Property(exp, prop.Property);
            }

            return null;
        }

        public IEnumerable<Expression<Func<T, object>>> GetIncludes() {
            foreach (var item in JoinProperties) {
                var param = Expression.Parameter(ClassType);

                Expression exp = param;
                foreach (var parent in item)
                    exp = Expression.Property(exp, parent);

                yield return Expression.Lambda<Func<T, object>>(exp, param);
            }
        }

        public T New() {
            var t = Activator.CreateInstance<T>();

            foreach (var Join in JoinProperties) {
                var param1 = Expression.Parameter(ClassType, "p1");

                Expression exp = param1;
                foreach (var parent in Join)
                    exp = Expression.Property(exp, parent);

                var param2 = Expression.Parameter(exp.Type, "p2");
                var lambda = Expression.Lambda(Expression.Assign(exp, param2), param1, param2);
                lambda.Compile().DynamicInvoke(t, Activator.CreateInstance(exp.Type)); //.Invoke(t, Activator.CreateInstance(exp.Type));
            }

            return t;
        }

        public PropertyInfo Get(TEnum ColumnType) {
            if (Properties.ContainsKey(ColumnType))
                return Properties[ColumnType].Property;

            return null;
        }

        public IEnumerable<PropRegister> List {
            get { return Properties.Values; }
        }

    }
}
