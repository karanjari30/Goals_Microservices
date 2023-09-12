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
using System.Collections.Generic;
using Intuit.Ipp.Data;
using DataAccessLayer.Interfaces;
using QBOCore;

namespace InsertUpdateQBOCustomer
{
    public class InsertUpdateCustomer
    {
        private readonly IConnectionService _connectionService;
        public readonly QBOAuthClient _qoAuthClient;
        public readonly QBOClients _qboClients;
        public InsertUpdateCustomer(IConnectionService connectionService, QBOAuthClient qboAuthClient, QBOClients qboClients)
        {
            _connectionService = connectionService;
            _qoAuthClient = qboAuthClient;
            _qboClients = qboClients;
        }

        [FunctionName("InsertUpdateCustomer")]
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
                    var requestModel = JsonConvert.DeserializeObject<QBOCustomerInsertUpdateReqViewModel>(requestBody);
                    if (!string.IsNullOrEmpty(requestModel.AccountingCompanyId))
                    {
                        var objConnectionData = await _connectionService.GetQBOAuthConfigureByCompanyId(requestModel.AccountingCompanyId.ToString());
                        if (objConnectionData != null)
                        {
                            List<Customer> customerList = new List<Customer>();
                            foreach(var customer in requestModel.customers)
                            {
                                var accessToken = await _qoAuthClient.GetAccessToken(objConnectionData.ERPCompanyId);
                                if (accessToken != null)
                                {
                                    var objCustomer = _qboClients.CreateUpdateCustomer(customer, objConnectionData.ERPCompanyId, accessToken);
                                    if(objCustomer != null)
                                    {
                                        customerList.Add(objCustomer);
                                    }
                                }
                                else
                                {
                                    objResultPT.ResultMsg = Message.QBOTokenExpire;
                                    objResultPT.TransactionStatus = ResponseStatus.Error;
                                }
                            }
                            objResultPT.ResultMsg = Message.CustomerInsertUpdateMessage;
                            objResultPT.TransactionStatus = ResponseStatus.Success;
                            objResultPT.ReturnObject = customerList;
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
