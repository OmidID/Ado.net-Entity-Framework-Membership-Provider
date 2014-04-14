#if USE_WEBMATRIX
using OmidID.Web.Security.Mapper;

namespace System.ComponentModel.DataAnnotations {
    public class OAuthMembershipAttribute : Attribute, IColumnAttribute<OAuthMembershipColumnType> {
        public OAuthMembershipAttribute(OAuthMembershipColumnType Type) {
            ColumnType = Type;
        }

        public OAuthMembershipColumnType ColumnType {
            get;
            private set;
        }

    }
}
#endif