using BusinessAccessLayer.Common;
using BusinessAccessLayer.Model;
using DataAccessLayer.Interfaces;
using RestSharp;
using System.Net;
using Xero.NetStandard.OAuth2.Client;
using Xero.NetStandard.OAuth2.Config;

namespace XeroCore
{
    public class XeroAuthClient
    {
        private readonly IConnectionService _connectionService;
        public XeroAuthClient(IConnectionService connectionService)
        {
            _connectionService = connectionService;
        }

        public static XeroClient XeroConfiguration()
        {
            XeroConfiguration xconfig = new XeroConfiguration()
            {
                ClientId = AppConfiguration.XeroClientID,
                ClientSecret = AppConfiguration.XeroClientSecret,
                CallbackUri = new Uri(AppConfiguration.XeroRedirectUrl),
                Scope = "openid profile email offline_access accounting.transactions accounting.settings accounting.contacts accounting.contacts.read accounting.attachments accounting.transactions accounting.budgets.read payroll.employees accounting.reports.read accounting.journals.read",
            };
            var client = new XeroClient(xconfig);
            return client;
        }

        public async Task<string> GetAccessToken(string tenantId)
        {
            XeroAuth authConfig = new XeroAuth();
            try
            {
                authConfig = await _connectionService.GetXeroConnectionByTenantId(tenantId);
                TimeSpan span = DateTime.UtcNow.Subtract((DateTime)authConfig.AccessTokenExpiryDate);
                if (span.TotalMinutes > 25 || DateTime.UtcNow > authConfig.AccessTokenExpiryDate)
                {
                    await RefreshTokenAsync(authConfig);
                    authConfig = await _connectionService.GetXeroConnectionByTenantId(tenantId);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return authConfig.AccessToken;
        }

        private async System.Threading.Tasks.Task RefreshTokenAsync(XeroAuth xeroAuth)
        {
            try
            {
                var objRefreshToken = ReGenerateTokenFromRefreshToken(xeroAuth.RefreshToken);
                if (!string.IsNullOrEmpty(objRefreshToken.access_token) && !string.IsNullOrEmpty(objRefreshToken.refresh_token))
                {
                    XeroAuth objXeroAuth = new XeroAuth()
                    {
                        TenantId = xeroAuth.TenantId,
                        TenantName = xeroAuth.TenantName,
                        IdToken = objRefreshToken.id_token,
                        AccessToken = objRefreshToken.access_token,
                        RefreshToken = objRefreshToken.refresh_token,
                        AccessTokenExpiryDate = DateTime.UtcNow.AddMinutes(objRefreshToken.expires_in),
                        IsCompanyConnected = true,
                    };
                    _connectionService.InsertOrUpdateAuthDetails(objXeroAuth);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static XeroAccessToken ReGenerateTokenFromRefreshToken(string refreshToken)
        {
            byte[] data = System.Text.ASCIIEncoding.ASCII.GetBytes(AppConfiguration.XeroClientID + ":" + AppConfiguration.XeroClientSecret);
            var basicAuthorizeString = "Basic " + System.Convert.ToBase64String(data);
            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            var client = new RestClient("https://identity.xero.com/connect/token");
            var request = new RestRequest(Method.POST);
            request.AddHeader("cache-control", "no-cache");
            request.AddParameter("grant_type", "refresh_token");
            request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
            request.AddHeader("Authorization", basicAuthorizeString);
            request.AddParameter("refresh_token", refreshToken);
            var response = client.Execute<XeroAccessToken>(request);
            if (response.StatusCode == HttpStatusCode.OK)
            {
                return response.Data;
            }
            else
            {
                return null;
            }
        }
    }
}