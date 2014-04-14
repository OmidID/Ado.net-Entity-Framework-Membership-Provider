#if USE_WEBMATRIX

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OmidID.Web.Security.Default {
    [Table("OAuthMembership")]
    public class DefaultOAuthMembership {

        [Key]
        [Column(Order = 1)]
        [OAuthMembership(Mapper.OAuthMembershipColumnType.ProviderToken)]
        public string ProviderToken { get; set; }

        [Key]
        [Column(Order = 2)]
        [OAuthMembership(Mapper.OAuthMembershipColumnType.ProviderName)]
        public string ProviderName { get; set; }

        [OAuthMembership(Mapper.OAuthMembershipColumnType.UserID)]
        public int UserID { get; set; }


    }
}

#endif