﻿using System.Threading.Tasks;

namespace Orion.UWP.Models.Clients
{
    public abstract class BaseClientWrapper
    {
        protected Provider Provider { get; }
        public Account Account { get; }

        public BaseClientWrapper(Provider provider)
        {
            Provider = provider;
            Account = new Account {Provider = provider, Credential = new Credential()};
        }

        public BaseClientWrapper(Account account)
        {
            Provider = account.Provider;
            Account = account;
        }

        /// <summary>
        ///     認証用 URL を取得します。
        /// </summary>
        /// <returns></returns>
        public abstract Task<string> GetAuthorizeUrlAsync();

        /// <summary>
        ///     アカウントを認証します。
        /// </summary>
        /// <returns></returns>
        public abstract Task<bool> AuthorizeAsync(string verifier);

        /// <summary>
        ///     アカウント情報を更新します。
        /// </summary>
        /// <returns></returns>
        public abstract Task<bool> RefreshAccountAsync();
    }
}