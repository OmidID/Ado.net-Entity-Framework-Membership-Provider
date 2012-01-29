using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity;

namespace OmidID.Web.Security.DataContext {
    public interface IUserContext<TUser, TKey>
        where TUser : class
        where TKey : struct {

        void Initialize(EFMembershipProvider<TUser, TKey> Provider);

        TUser GetUser(TKey Key);
        TUser GetUser(string Username);
        TUser GetUserByEmail(string Email);

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
