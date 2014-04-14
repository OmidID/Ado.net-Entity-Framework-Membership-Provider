using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;
using System.Web.Security;

namespace OmidID.Web.Security {
    public class Installer {

        public MembershipProvider MembershipProvider { get; set; }
        public RoleProvider RoleProvider { get; set; }

        public bool CreateIfNotExist() {
            var TableNames = new Dictionary<Type, TableAttribute>();
            var cnn = "";
            if (MembershipProvider != null) {
                //if (MembershipProvider.GetType().GetGenericTypeDefinition() != typeof(EFMembershipProvider<,,>))
                //    throw new ArgumentException("INSTALL_Invalid_membership_provider_type".Resource());
                    
                dynamic tablePrefix = ((dynamic)MembershipProvider).TablePrefix;
                dynamic tableSchema = ((dynamic)MembershipProvider).TableSchema;
                dynamic helper = ((dynamic)MembershipProvider).Helper;
#if USE_WEBMATRIX
                dynamic helper_oauth = ((dynamic)MembershipProvider).Helper_OAuth;
                
                foreach (var item in (Dictionary<Type, TableAttribute>)helper_oauth.TableNames)
                    TableNames[item.Key] = item.Value;

#endif
                cnn = ((dynamic)MembershipProvider).ConnectionString;
                foreach (var item in (Dictionary<Type, TableAttribute>)helper.TableNames)
                    TableNames[item.Key] = item.Value;

                var propAppType = (PropertyInfo)helper.Get(Mapper.UserColumnType.Application);
                if (propAppType != null) {
                    var appType = propAppType.PropertyType;

                    var tblNameAttr = appType.GetCustomAttributes(typeof(TableAttribute), true);
                    var noPrefixAttr = appType.GetCustomAttributes(typeof(NoPrefixAttribute), true);

                    var ApplicationTableNoPrefix = noPrefixAttr != null && noPrefixAttr.Length > 0;
                    var first = tblNameAttr.FirstOrDefault() as TableAttribute;

                    TableAttribute ApplicationTableName;
                    if (first != null)
                        ApplicationTableName = first;
                    else
                        ApplicationTableName = new TableAttribute(appType.Name.Trim().Replace(" ", ""));
                    if (!string.IsNullOrEmpty(tableSchema)) ApplicationTableName.Schema = tableSchema;

                    if (!ApplicationTableNoPrefix && !string.IsNullOrWhiteSpace(tablePrefix)) {
                        var schema = ApplicationTableName.Schema;
                        ApplicationTableName = new TableAttribute(tablePrefix + ApplicationTableName.Name);
                        if (schema != null) ApplicationTableName.Schema = schema;
                    }

                    TableNames[appType] = ApplicationTableName;
                }
            }

            if (RoleProvider != null) {
                //if (RoleProvider.GetType().GetGenericTypeDefinition() != typeof(EFRoleProvider<,,>))
                //    throw new ArgumentException("INSTALL_Invalid_role_provider_type".Resource());

                dynamic tablePrefix = ((dynamic)RoleProvider).TablePrefix;
                dynamic tableSchema = ((dynamic)RoleProvider).TableSchema;
                dynamic helper1 = ((dynamic)RoleProvider).RoleHelper;
                if (string.IsNullOrEmpty(cnn))
                    cnn = ((dynamic)RoleProvider).ConnectionString;

                foreach (var item in (Dictionary<Type, TableAttribute>)helper1.TableNames)
                    TableNames[item.Key] = item.Value;

                dynamic helper2 = ((dynamic)RoleProvider).UserRoleHelper;
                foreach (var item in (Dictionary<Type, TableAttribute>)helper2.TableNames)
                    TableNames[item.Key] = item.Value;

                var propAppType = (PropertyInfo)helper1.Get(Mapper.RoleColumnType.Application);
                if (propAppType != null) {
                    var appType = propAppType.PropertyType;

                    var tblNameAttr = appType.GetCustomAttributes(typeof(TableAttribute), true);
                    var noPrefixAttr = appType.GetCustomAttributes(typeof(NoPrefixAttribute), true);

                    var ApplicationTableNoPrefix = noPrefixAttr != null && noPrefixAttr.Length > 0;
                    var first = tblNameAttr.FirstOrDefault() as TableAttribute;

                    TableAttribute ApplicationTableName;
                    if (first != null)
                        ApplicationTableName = first;
                    else
                        ApplicationTableName = new TableAttribute(appType.Name.Trim().Replace(" ", ""));
                    if (!string.IsNullOrEmpty(tableSchema)) ApplicationTableName.Schema = tableSchema;

                    if (!ApplicationTableNoPrefix && !string.IsNullOrWhiteSpace(tablePrefix)) {
                        var schema = ApplicationTableName.Schema;
                        ApplicationTableName = new TableAttribute(tablePrefix + ApplicationTableName.Name);
                        if (schema != null) ApplicationTableName.Schema = schema;
                    }

                    TableNames[appType] = ApplicationTableName;
                }
            }

            var internalContext = new InternalInstallDataContext(cnn);
            internalContext.TableNames = TableNames;

            
                //System.Data.Entity.Migrations.DbMigrator mig = new System.Data.Entity.Migrations.DbMigrator()
                //mig.Update();

            return internalContext.Database.CreateIfNotExists();
        }

        //public class UpgradeDb : System.Data.Entity.Migrations.DbMigration {

        //    public override void Up() {
        //        var cnf = new System.Data.Entity.Migrations.
        //        System.Data.Entity.Migrations.DbMigrator mig = new System.Data.Entity.Migrations.DbMigrator();
        //        mig.Update();
        //    }

        //}

    }
}
