#if USE_WEBMATRIX
namespace OmidID.Web.Security.Mapper {
    public interface IOAuthMembershipMapper<TOAuthMembership, TUserKey> : IMapper<TOAuthMembership, OAuthMembershipColumnType> {

        TUserKey UserID(TOAuthMembership oauth);
        IOAuthMembershipMapper<TOAuthMembership, TUserKey> UserID(TOAuthMembership oauth, TUserKey value);

        string ProviderToken(TOAuthMembership oauth);
        IOAuthMembershipMapper<TOAuthMembership, TUserKey> ProviderToken(TOAuthMembership oauth, string value);

        string ProviderName(TOAuthMembership oauth);
        IOAuthMembershipMapper<TOAuthMembership, TUserKey> ProviderName(TOAuthMembership oauth, string value);


    }
}
#endif