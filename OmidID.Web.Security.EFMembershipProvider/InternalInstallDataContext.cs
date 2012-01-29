using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace OmidID.Web.Security {
    class InternalInstallDataContext : DbContext {

        internal InternalInstallDataContext(string cnnStr)
            : base(cnnStr) {
            this.Configuration.AutoDetectChangesEnabled = false;
            this.Configuration.LazyLoadingEnabled = false;
        }

        internal Dictionary<Type, TableAttribute> TableNames { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder) {
            var type = typeof(DbModelBuilder);
            var method = type.GetMethod("Entity", new Type[] { });

            foreach (var item in TableNames) {
                var output = method.MakeGenericMethod(item.Key).Invoke(modelBuilder, null);
                output.GetType().InvokeMember("ToTable", System.Reflection.BindingFlags.InvokeMethod, null, output, new object[] { item.Value.Name, item.Value.Schema });
            }
            
            modelBuilder.Ignore<EdmMetadata>();
            modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();
        }


    }
}
