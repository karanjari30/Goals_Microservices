using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using XeroCore;
using DataAccessLayer.Interfaces;
using BusinessAccessLayer.Common;
using BusinessAccessLayer.ViewModel;
using static BusinessAccessLayer.Common.Enums;
using Xero.NetStandard.OAuth2.Model.Accounting;

namespace InsertUpdateXeroPayment
{
    public class InsertUpdatePayment
    {
        private readonly IConnectionService _connectionService;
        public readonly XeroAuthClient _xeroAuthClient;
        public readonly XeroClients _xeroClients;
        public InsertUpdatePayment(IConnectionService connectionService, XeroAuthClient xeroAuthClient, XeroClients xeroClients)
        {
            _connectionService = connectionService;
            _xeroAuthClient = xeroAuthClient;
            _xeroClients = xeroClients;
        }

        [FunctionName("InsertUpdatePayment")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");
            var objResultPT = new XeroPaymentSyncResult();
            try
            {
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                if (!string.IsNullOrEmpty(requestBody))
                {
                    var requestModel = JsonConvert.DeserializeObject<XeroPaymentInsertUpdateReqViewModel>(requestBody);
                    if (!string.IsNullOrEmpty(requestModel.TenantId))
                    {
                        var objConnectionData = await _connectionService.GetXeroConnectionByTenantId(requestModel.TenantId);
                        if (objConnectionData != null)
                        {
                            foreach (var payment in requestModel.Payments)
                            {
                                var accessToken = await _xeroAuthClient.GetAccessToken(objConnectionData.TenantId);
                                if (accessToken != null)
                                {
                                    var paymentMapping = XeroPaymentMapping(payment);
                                    var objpayments = await _xeroClients.CreateUpdatePayment(paymentMapping, objConnectionData.TenantId, accessToken);

                                    objResultPT.ResultMsg = Message.PaymentInsertUpdateMessage;
                                    objResultPT.TransactionStatus = ResponseStatus.Success;
                                    objResultPT.ReturnObject = objpayments;
                                }
                                else
                                {
                                    objResultPT.ResultMsg = Message.XeroTokenExpire;
                                    objResultPT.TransactionStatus = ResponseStatus.Error;
                                }
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

        private Xero.NetStandard.OAuth2.Model.Accounting.Payment XeroPaymentMapping(XeroPaymentModel xeroPayment)
        {
            var payment = new Payment();
            payment.Account = new Account()
            {
                AccountID = !string.IsNullOrEmpty(xeroPayment.AccountId) ? new Guid(xeroPayment.AccountId) : new Guid()
            };
            payment.Invoice = new Invoice()
            {
                InvoiceID = !string.IsNullOrEmpty(xeroPayment.InvoiceId) ? new Guid(xeroPayment.InvoiceId) : new Guid()
            };
            payment.Date = xeroPayment.Date;
            payment.Reference = xeroPayment.Reference ?? "";
            payment.Amount = xeroPayment.Amount;

            return payment;
        }
    }
}
