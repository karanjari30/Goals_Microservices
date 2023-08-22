using BusinessAccessLayer.Common;
using Intuit.Ipp.OAuth2PlatformClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QBOCore
{
    public class QBOAuthClient
    {
        public static OAuth2Client auth2Client = new OAuth2Client(AppConfiguration.QBOClientID, AppConfiguration.QBOClientSecret, AppConfiguration.QBORedirectUrl, AppConfiguration.QBOEnvironment); // environment is “sandbox” or “production”

        public static string GetAuthorizationURL(Guid companyId)
        {
            string scopes = "com.intuit.quickbooks.accounting profile openid phone email";
            string authorizationEndpoint = "https://appcenter.intuit.com/connect/oauth2";

            var authorizationUrl = new StringBuilder(authorizationEndpoint);
            authorizationUrl.AppendFormat("?client_id={0}", Uri.EscapeDataString(AppConfiguration.QBOClientID));
            authorizationUrl.Append("&response_type=code");
            authorizationUrl.AppendFormat("&scope={0}", Uri.EscapeDataString(scopes));
            authorizationUrl.AppendFormat("&redirect_uri={0}", Uri.EscapeDataString(AppConfiguration.QBORedirectUrl));
            authorizationUrl.AppendFormat("&state={0}", companyId);

            return authorizationUrl.ToString();
        }
    }
}
