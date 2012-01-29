using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity;

namespace OmidID.Web.Security.DataContext {
    class InternalRoleContext<TRole, TUserRole, TRoleKey> : DbContext
        where TRole : class
        where TUserRole : class
        where TRoleKey : struct {

        public DefaultRoleContext<TRole, TUserRole, TRoleKey> Context { get; set; }
        public InternalRoleContext(DefaultRoleContext<TRole, TUserRole, TRoleKey> Context)
            : base(Context.Provider.ConnectionString) {
            this.Context = Context;
            this.Configuration.AutoDetectChangesEnabled = false;
        }

        private class SelectHelper<T1,T2> {
            public T1 RoleID { get; set; }
            public T1 UserID { get; set; }
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder) {
            var type = typeof(DbModelBuilder);
            var method = type.GetMethod("Entity", new Type[] { });
            if (Context.Provider.SupportApplication) {
                var output = method.MakeGenericMethod(Context.ApplicationType).Invoke(modelBuilder, null);
                output.GetType().InvokeMember("ToTable", System.Reflection.BindingFlags.InvokeMethod, null, output, new object[] { Context.ApplicationTableName.Name, Context.ApplicationTableName.Schema });
            }

            foreach (var item in Context.RoleHelper.TableNames) {
                var output = method.MakeGenericMethod(item.Key).Invoke(modelBuilder, null);
                output.GetType().InvokeMember("ToTable", System.Reflection.BindingFlags.InvokeMethod, null, output, new object[] { item.Value.Name, item.Value.Schema });
            }
            foreach (var item in Context.UserRoleHelper.TableNames) {
                var output = method.MakeGenericMethod(item.Key).Invoke(modelBuilder, null);
                output.GetType().InvokeMember("ToTable", System.Reflection.BindingFlags.InvokeMethod, null, output, new object[] { item.Value.Name, item.Value.Schema });
            }
        }

        public System.Data.Entity.DbSet<TRole> Roles { get; set; }
        public System.Data.Entity.DbSet<TUserRole> UserRoles { get; set; }

        Models.Application application;
        public Models.Application GetApplication() {
            if (!Context.Provider.SupportApplication) return null;
            if (application != null) return application;

            var myType = this.GetType();
            var method = myType.GetMethod("Set", new Type[] { }).MakeGenericMethod(Context.ApplicationType);
            var obj = method.Invoke(this, null);

            var dbType = obj.GetType();
            var Application = ((IEnumerable<Models.Application>)obj).FirstOrDefault(f => f.Name == Context.Provider.ApplicationName);
            if (Application == null) {
                Application = Activator.CreateInstance(Context.ApplicationType) as Models.Application;
                Application.Name = Context.Provider.ApplicationName;

                dbType.InvokeMember("Add", System.Reflection.BindingFlags.InvokeMethod, null, obj, new object[] { Application });
                SaveChanges();
            }

            application = Application;
            return application;
        }

    }
}
