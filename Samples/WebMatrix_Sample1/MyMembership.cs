using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebMatrix_Sample1 {
    public class MyMembership :
        OmidID.Web.Security.EFMembershipProvider<
            OmidID.Web.Security.Default.DefaultUser,
            OmidID.Web.Security.Default.DefaultOAuthMembership,
            int> {
    }
}