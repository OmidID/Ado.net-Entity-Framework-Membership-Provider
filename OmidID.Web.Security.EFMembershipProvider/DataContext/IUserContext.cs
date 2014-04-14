using System.Collections.Generic;

namespace OmidID.Web.Security.DataContext {
    public interface 
#if USE_WEBMATRIX
        IUserContext<TUser, TOAuthMembership, TKey>
        where TUser : class
        where TOAuthMembership : class
        where TKey : struct 
#else
        IUserContext<TUser, TKey>
        where TUser : class
        where TKey : struct 
#endif

 {

#if USE_WEBMATRIX
        void Initialize(EFMembershipProvider<TUser, TOAuthMembership, TKey> Provider);
#else
        void Initialize(EFMembershipProvider<TUser, TKey> Provider);
#endif

        TUser GetUser(TKey Key);
        TUser GetUser(string Username);
        TUser GetUserByEmail(string Email);

#if USE_WEBMATRIX

        TUser GetUserByPasswordToken(string token);

        TUser GetByConfirmationCode(string confirmToken);
        TUser GetByConfirmationCode(string username, string confirmToken);

        IEnumerable<TOAuthMembership> GetAccountsForUser(string username);
        TOAuthMembership GetOAuthMembership(string provider, string providerUserID);

        TOAuthMembership AddOAuth(TOAuthMembership Entity);
        TOAuthMembership UpdateOAuth(TOAuthMembership Entity);
        TOAuthMembership DeleteOAuth(TOAuthMembership Entity);
#endif

        IEnumerable<TUser> UserList(int PageSize, int PageIndex, out int TotalRecords);
        IEnumerable<TUser> FindByUsername(string Username, int PageSize, int PageIndex, out int TotalRecords);
        IEnumerable<TUser> FindByEmail(string EmailAddress, int PageSize, int PageIndex, out int TotalRecords);

        TUser Add(TUser Entity);
        TUser Update(TUser Entity);
        TUser Delete(TUser Entity);

        int NumberOfOnlineUsers(int TimeoutInMinute);

        TKey GetUserID(string Username);
        TKey[] GetUserID(string[] Username);

        string GetUsername(TKey UserID);
        string[] GetUsername(TKey[] UserID);

    }
}
