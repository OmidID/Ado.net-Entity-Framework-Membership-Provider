using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebMatrix_Sample1 {
    public class MyRole :
        OmidID.Web.Security.EFRoleProvider<
            OmidID.Web.Security.Default.DefaultRole,
            OmidID.Web.Security.Default.DefaultUserRole,
            int> {
    }
}