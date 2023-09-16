using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Collections.Generic;
using BusinessAccessLayer.Common;
using static BusinessAccessLayer.Common.Enums;
using DataAccessLayer.Interfaces;
using QBOCore;
using BusinessAccessLayer.ViewModel;
using Intuit.Ipp.QueryFilter;
using System.Linq;

namespace GetQBOPayments
{
    public class GetPayments
    {
        private readonly IConnectionService _connectionService;
        public readonly QBOAuthClient _qoAuthClient;
        public readonly QBOClients _qboClients;
        public GetPayments(IConnectionService connectionService, QBOAuthClient qboAuthClient, QBOClients qboClients)
        {
            _connectionService = connectionService;
            _qoAuthClient = qboAuthClient;
            _qboClients = qboClients;
        }

        [FunctionName("GetPayments")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");
            var objResultPT = new PaymentSyncResult();
            try
            {
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                if (!string.IsNullOrEmpty(requestBody))
                {
                    var requestModel = JsonConvert.DeserializeObject<QBOPaymentReqViewModel>(requestBody);
                    if (!string.IsNullOrEmpty(requestModel.AccountingCompanyId))
                    {
                        var objConnectionData = await _connectionService.GetQBOAuthConfigureByCompanyId(requestModel.AccountingCompanyId.ToString());
                        if (objConnectionData != null)
                        {
                            var accessToken = await _qoAuthClient.GetAccessToken(objConnectionData.ERPCompanyId);
                            if (accessToken != null)
                            {
                                Intuit.Ipp.Core.ServiceContext serviceContext = QBOAuthClient.Service(objConnectionData.ERPCompanyId, accessToken);
                                var qboQueryService = new QueryService<Intuit.Ipp.Data.Payment>(serviceContext);

                                var lstPayments = _qboClients.GetQBOPayments(new List<Intuit.Ipp.Data.Payment>(), objConnectionData.ERPCompanyId, qboQueryService, requestModel);
                                if (lstPayments != null && lstPayments.Any())
                                {
                                    objResultPT.ResultMsg = Message.PaymentsFoundMessage;
                                    objResultPT.TransactionStatus = ResponseStatus.Success;
                                    objResultPT.ReturnObject = lstPayments;
                                }
                                else
                                {
                                    objResultPT.ResultMsg = Message.PaymentsNotFoundMessage;
                                    objResultPT.TransactionStatus = ResponseStatus.Success;
                                }
                            }
                            else
                            {
                                objResultPT.ResultMsg = Message.QBOTokenExpire;
                                objResultPT.TransactionStatus = ResponseStatus.Error;
                            }
                        }
                        else
                        {
                            objResultPT.ResultMsg = Message.OrganizationNotFoundMessage;
                            objResultPT.TransactionStatus = ResponseStatus.Error;
                        }
                    }
                    else
                    {
                        objResultPT.ResultMsg = Message.CompanyIdMessage;
                        objResultPT.TransactionStatus = ResponseStatus.Error;
                    }
                }
                else
                {
                    objResultPT.ResultMsg = Message.QueueItemNotFound;
                    objResultPT.TransactionStatus = ResponseStatus.Error;
                }
            }
            catch (Exception ex)
            {
                objResultPT.TransactionStatus = ResponseStatus.Error;
                objResultPT.ResultMsg = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
            }
            return new OkObjectResult(objResultPT);
        }
    }
}
