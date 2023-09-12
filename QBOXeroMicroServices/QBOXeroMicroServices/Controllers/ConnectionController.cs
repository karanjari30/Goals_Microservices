using BusinessAccessLayer.Common;
using BusinessAccessLayer.Model;
using BusinessAccessLayer.ViewModel;
using DataAccessLayer.Interfaces;
using Intuit.Ipp.Data;
using Intuit.Ipp.OAuth2PlatformClient;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using QBOCore;
using System.ComponentModel.DataAnnotations;
using System.Text;
using static BusinessAccessLayer.Common.Enums;

namespace QBOXeroMicroServices.Controllers
{
    [Route("api/")]
    [ApiController]
    public class ConnectionController : ControllerBase
    {
        private readonly IConnectionService _connectionService;
        public ConnectionController(IConnectionService connectionService)
        {
            _connectionService = connectionService;
        }

        [HttpGet]
        [Route("[Controller]")]
        public async Task<IActionResult> Index([Required] Guid companyId)
        {
            var objResponse = new ResultPT();
            try
            {
                if(companyId == Guid.Empty)
                {
                    objResponse.ResponseStatus = ResponseStatus.Error;
                    objResponse.Message = Message.CompanyIdMessage;
                    return Ok(objResponse);
                }
                else
                {
                    var getConnectionData = await _connectionService.GetQBOAuthConfigureByCompanyId(companyId.ToString());
                    if(getConnectionData == null)
                    {
                        objResponse.ResponseStatus = ResponseStatus.Error;
                        objResponse.Message = Message.OrganizationNotFoundMessage;
                        objResponse.Result = QBOAuthClient.GetAuthorizationURL(companyId);
                    }
                    else
                    {
                        if (getConnectionData.IsCompanyConnected)
                        {
                            objResponse.ResponseStatus = ResponseStatus.Success;
                            objResponse.Message = Message.OrganizationFoundMessage;
                            objResponse.Result = getConnectionData;
                        }
                        else
                        {
                            objResponse.ResponseStatus = ResponseStatus.Error;
                            objResponse.Message = Message.OrganizationNotFoundMessage;
                            objResponse.Result = QBOAuthClient.GetAuthorizationURL(companyId);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                objResponse.ResponseStatus = ResponseStatus.Error;
                objResponse.Message = GetCommonMessage.GetExceptionMessage(ex);
            }
            return Ok(objResponse);
        }

        [Route("Callback")]
        [AllowAnonymous]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> Callback([Required] string code, [Required] Guid state, [Required] string realmId)
        {
            var objResponse = new ResultPT();
            if (!string.IsNullOrEmpty(code))
            {
                var tokenResponse = QBOAuthClient.GetBearerTokenAsync(code);
                if (tokenResponse != null && !string.IsNullOrEmpty(tokenResponse.AccessToken))
                {
                    var qboCompanyInfo = QBOAuthClient.QuickBookCompanyInfo(realmId, tokenResponse.AccessToken);
                    var quickBooksConnection = await _connectionService.GetQBOAuthConfigureByCompanyId(state.ToString());
                    if (quickBooksConnection != null)
                    {
                        if (quickBooksConnection.AccountingCompanyId == state.ToString() && quickBooksConnection.ERPCompanyId == realmId)
                        {
                            QBOAuth qBOAuth = new QBOAuth()
                            {
                                AccountingCompanyId = state.ToString(),
                                ERPCompanyName = qboCompanyInfo.CompanyName,
                                ERPCompanyId = realmId,
                                AccessToken = tokenResponse.AccessToken,
                                RefreshToken = tokenResponse.RefreshToken,
                                AccessTokenExpiryDate = (DateTime.UtcNow.ToEpochTime() + tokenResponse.AccessTokenExpiresIn).ToDateTimeFromEpoch(),
                                RefreshTokenExpiryDate = (DateTime.UtcNow.ToEpochTime() + tokenResponse.RefreshTokenExpiresIn).ToDateTimeFromEpoch(),
                                IsCompanyConnected = true,
                            };
                            _connectionService.InsertOrUpdateAuthDetails(qBOAuth);

                            objResponse.ResponseStatus = ResponseStatus.Success;
                            objResponse.Message = Message.QuickBookCompanyConnected;
                        }
                        else
                        {
                            objResponse.ResponseStatus = ResponseStatus.Success;
                            objResponse.Message = Message.QuickBookCompanyAlreadyConnected;
                        }
                    }
                    else
                    {
                        var connection = await _connectionService.GetQuickBooksConnectionByRealmId(realmId);
                        if (connection == null)
                        {
                            //await _connectionsServices.CreateUpdateConnection(realmId, state, qboCompanyInfo.CompanyName, tokenResponse);
                            QBOAuth qBOAuth = new QBOAuth()
                            {
                                AccountingCompanyId = state.ToString(),
                                ERPCompanyName = qboCompanyInfo.CompanyName,
                                ERPCompanyId = realmId,
                                AccessToken = tokenResponse.AccessToken,
                                RefreshToken = tokenResponse.RefreshToken,
                                AccessTokenExpiryDate = (DateTime.UtcNow.ToEpochTime() + tokenResponse.AccessTokenExpiresIn).ToDateTimeFromEpoch(),
                                RefreshTokenExpiryDate = (DateTime.UtcNow.ToEpochTime() + tokenResponse.RefreshTokenExpiresIn).ToDateTimeFromEpoch(),
                                IsCompanyConnected = true,
                            };
                            _connectionService.InsertOrUpdateAuthDetails(qBOAuth);

                            objResponse.ResponseStatus = ResponseStatus.Success;
                            objResponse.Message = Message.QuickBookCompanyConnected;
                        }
                        else
                        {
                            objResponse.ResponseStatus = ResponseStatus.Success;
                            objResponse.Message = Message.QuickBookCompanyAlreadyConnected;
                        }
                    }
                }
            }
            return Ok(objResponse);
        }

        [HttpPost]
        [Route("[Controller]/Disconnect")]
        public async Task<IActionResult> Disconnect([Required] Guid companyId)
        {
            var objResponse = new ResultPT();
            try
            {
                var objConnectionDetails = await _connectionService.GetQBOAuthConfigureByCompanyId(companyId.ToString());
                if (objConnectionDetails != null && objConnectionDetails.RefreshToken != null)
                {
                    QBOAuthClient.RevokeToken(objConnectionDetails.RefreshToken);
                    _connectionService.DisconnectQuickBookCompany(objConnectionDetails);

                    objResponse.ResponseStatus = ResponseStatus.Success;
                    objResponse.Message = string.Format(Message.OrganizationDisconnectMessage);
                    objResponse.Result = objConnectionDetails;
                }
                else
                {
                    objResponse.ResponseStatus = ResponseStatus.Error;
                    objResponse.Message = string.Format(Message.OrganizationAlreadyDisconnectMessage);
                }
            }
            catch (Exception ex)
            {
                objResponse.ResponseStatus = ResponseStatus.Error;
                objResponse.Message = GetCommonMessage.GetExceptionMessage(ex);
            }
            return Ok(objResponse);
        }
    }
}
