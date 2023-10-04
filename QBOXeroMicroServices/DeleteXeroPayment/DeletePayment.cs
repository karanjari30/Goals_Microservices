using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using DataAccessLayer.Interfaces;
using XeroCore;
using BusinessAccessLayer.Common;
using BusinessAccessLayer.ViewModel;
using static BusinessAccessLayer.Common.Enums;

namespace DeleteXeroPayment
{
    public class DeletePayment
    {
        private readonly IConnectionService _connectionService;
        public readonly XeroAuthClient _xeroAuthClient;
        public readonly XeroClients _xeroClients;
        public DeletePayment(IConnectionService connectionService, XeroAuthClient xeroAuthClient, XeroClients xeroClients)
        {
            _connectionService = connectionService;
            _xeroAuthClient = xeroAuthClient;
            _xeroClients = xeroClients;
        }
        [FunctionName("DeletePayment")]
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
                    var requestModel = JsonConvert.DeserializeObject<XeroPaymentDeleteReqViewModel>(requestBody);
                    if (!string.IsNullOrEmpty(requestModel.TenantId))
                    {
                        if (!string.IsNullOrEmpty(requestModel.PaymentId))
                        {
                            var objConnectionData = await _connectionService.GetXeroConnectionByTenantId(requestModel.TenantId);
                            if (objConnectionData != null)
                            {
                                var accessToken = await _xeroAuthClient.GetAccessToken(objConnectionData.TenantId);
                                if (accessToken != null)
                                {
                                    var payment = await _xeroClients.DeletePaymentById(requestModel.PaymentId, objConnectionData.TenantId, accessToken);
                                    if (payment != null)
                                    {
                                        objResultPT.ResultMsg = Message.PaymentDeleteMessage;
                                        objResultPT.TransactionStatus = ResponseStatus.Success;
                                        objResultPT.ReturnObject = payment.PaymentID;
                                    }
                                    else
                                    {
                                        objResultPT.ResultMsg = Message.PaymentsNotFoundMessage;
                                        objResultPT.TransactionStatus = ResponseStatus.Success;
                                    }
                                }
                                else
                                {
                                    objResultPT.ResultMsg = Message.XeroTokenExpire;
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
                            objResultPT.ResultMsg = Message.InvoiceIdMessage;
                            objResultPT.TransactionStatus = ResponseStatus.Error;
                        }
                    }
                    else
                    {
                        objResultPT.ResultMsg = Message.CompanyIdMessage;
                        objResultPT.TransactionStatus = ResponseStatus.Error;
                    }
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
