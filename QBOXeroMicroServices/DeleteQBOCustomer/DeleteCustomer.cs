using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using BusinessAccessLayer.Common;
using static BusinessAccessLayer.Common.Enums;
using BusinessAccessLayer.ViewModel;
using DataAccessLayer.Interfaces;
using QBOCore;
using Intuit.Ipp.QueryFilter;

namespace DeleteQBOCustomer
{
    public class DeleteCustomer
    {
        private readonly IConnectionService _connectionService;
        public readonly QBOAuthClient _qoAuthClient;
        public readonly QBOClients _qboClients;
        public DeleteCustomer(IConnectionService connectionService, QBOAuthClient qboAuthClient, QBOClients qboClients)
        {
            _connectionService = connectionService;
            _qoAuthClient = qboAuthClient;
            _qboClients = qboClients;
        }

        [FunctionName("DeleteCustomer")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");
            var objResultPT = new DownloadResultPT();

            try
            {
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                if (!string.IsNullOrEmpty(requestBody))
                {
                    var requestModel = JsonConvert.DeserializeObject<QBOCustomerDeleteReqViewModel>(requestBody);
                    if (!string.IsNullOrEmpty(requestModel.AccountingCompanyId))
                    {
                        if (!string.IsNullOrEmpty(requestModel.QBOCustomerId))
                        {
                            var objConnectionData = await _connectionService.GetQBOAuthConfigureByCompanyId(requestModel.AccountingCompanyId.ToString());
                            if (objConnectionData != null)
                            {
                                var accessToken = await _qoAuthClient.GetAccessToken(objConnectionData.ERPCompanyId);
                                if (accessToken != null)
                                {
                                    var customer = _qboClients.DeleteCustomerById(requestModel.QBOCustomerId, objConnectionData.ERPCompanyId, accessToken);
                                    if(customer != null)
                                    {
                                        objResultPT.ResultMsg = Message.CustomerDeleteMessage;
                                        objResultPT.TransactionStatus = ResponseStatus.Success;
                                        objResultPT.ReturnObject = customer.Id;
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
                            objResultPT.ResultMsg = Message.CustomerIdMessage;
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
