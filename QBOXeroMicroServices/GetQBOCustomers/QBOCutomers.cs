using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using BusinessAccessLayer.Common;
using static BusinessAccessLayer.Common.Enums;
using System;
using Newtonsoft.Json;
using BusinessAccessLayer.ViewModel;
using QBOCore;
using DataAccessLayer.Interfaces;
using Intuit.Ipp.QueryFilter;
using System.Collections.Generic;
using System.Linq;

namespace GetQBOCustomers
{
    public class QBOCutomers
    {
        private readonly IConnectionService _connectionService;
        public readonly QBOAuthClient _qoAuthClient;
        public readonly QBOClients _qboClients;
        public QBOCutomers(IConnectionService connectionService, QBOAuthClient qboAuthClient, QBOClients qboClients)
        {
            _connectionService = connectionService;
            _qoAuthClient = qboAuthClient;
            _qboClients = qboClients;
        }

        [FunctionName("QBOCutomers")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");
            var objResultPT = new CustomerManaualSyncResult();

            try
            {
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                if (!string.IsNullOrEmpty(requestBody))
                {
                    var requestModel = JsonConvert.DeserializeObject<QBOCustomerReqViewModel>(requestBody);
                    if (!string.IsNullOrEmpty(requestModel.AccountingCompanyId))
                    {
                        var objConnectionData = await _connectionService.GetQBOAuthConfigureByCompanyId(requestModel.AccountingCompanyId.ToString());
                        if(objConnectionData != null)
                        {
                            var accessToken = await _qoAuthClient.GetAccessToken(objConnectionData.ERPCompanyId);
                            if (accessToken != null)
                            {
                                Intuit.Ipp.Core.ServiceContext serviceContext = QBOAuthClient.Service(objConnectionData.ERPCompanyId, accessToken);
                                var qboQueryService = new QueryService<Intuit.Ipp.Data.Customer>(serviceContext);

                                var lstCustomers = _qboClients.GetQBOCustomers(new List<Intuit.Ipp.Data.Customer>(), objConnectionData.ERPCompanyId, qboQueryService, requestModel);
                                if (lstCustomers != null && lstCustomers.Any())
                                {
                                    objResultPT.ResultMsg = Message.CustomersFoundMessage;
                                    objResultPT.TransactionStatus = ResponseStatus.Success;
                                    objResultPT.ReturnObject = lstCustomers;
                                }
                                else
                                {
                                    objResultPT.ResultMsg = Message.CustomersFoundMessage;
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
