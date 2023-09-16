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
using QBOCore;
using BusinessAccessLayer.Common;
using BusinessAccessLayer.ViewModel;
using Intuit.Ipp.Data;
using static BusinessAccessLayer.Common.Enums;
using System.Collections.Generic;
using System.Linq;

namespace InsertUpdateQBOPayment
{
    public class InsertUpdatePayment
    {
        private readonly IConnectionService _connectionService;
        public readonly QBOAuthClient _qoAuthClient;
        public readonly QBOClients _qboClients;
        public InsertUpdatePayment(IConnectionService connectionService, QBOAuthClient qboAuthClient, QBOClients qboClients)
        {
            _connectionService = connectionService;
            _qoAuthClient = qboAuthClient;
            _qboClients = qboClients;
        }

        [FunctionName("InsertUpdatePayment")]
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
                    var requestModel = JsonConvert.DeserializeObject<QBOPaymentInsertUpdateReqViewModel>(requestBody);
                    if (!string.IsNullOrEmpty(requestModel.AccountingCompanyId))
                    {
                        var objConnectionData = await _connectionService.GetQBOAuthConfigureByCompanyId(requestModel.AccountingCompanyId.ToString());
                        if (objConnectionData != null)
                        {
                            List<Payment> paymentList = new List<Payment>();
                            foreach (var payment in requestModel.Payments)
                            {
                                var accessToken = await _qoAuthClient.GetAccessToken(objConnectionData.ERPCompanyId);
                                if (accessToken != null)
                                {
                                    var paymentMapping = QBOpaymentMapping(payment);
                                    var objpayment = _qboClients.CreateUpdatePayment(paymentMapping, objConnectionData.ERPCompanyId, accessToken);
                                    if (objpayment != null)
                                    {
                                        paymentList.Add(objpayment);
                                    }
                                }
                                else
                                {
                                    objResultPT.ResultMsg = Message.QBOTokenExpire;
                                    objResultPT.TransactionStatus = ResponseStatus.Error;
                                }
                            }
                            objResultPT.ResultMsg = Message.PaymentInsertUpdateMessage;
                            objResultPT.TransactionStatus = ResponseStatus.Success;
                            objResultPT.ReturnObject = paymentList;
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

        private Payment QBOpaymentMapping(PaymentReqViewModel reqViewModel)
        {
            var objPayment = new Payment();
            objPayment.CustomerRef = new ReferenceType { Value = reqViewModel.CustomerId };
            objPayment.PaymentRefNum = reqViewModel.PaymentNo ?? "";
            objPayment.UnappliedAmt = 0;
            objPayment.UnappliedAmtSpecified = true;
            objPayment.TotalAmt = reqViewModel.invoices.Sum(x => x.amountPaid) ?? 0;
            objPayment.TotalAmtSpecified = true;
            objPayment.CurrencyRef = new ReferenceType { Value = "USD" };
            objPayment.PaymentMethodRef = new ReferenceType { Value = reqViewModel.PaymentMethodId ?? "" };
            objPayment.DepositToAccountRef = new ReferenceType { Value = reqViewModel.DepositToAccountId ?? "" };

            List<Line> lines = new List<Line>();
            foreach (var invoice in reqViewModel.invoices)
            {
                LinkedTxn[] objLinkedTxn = new LinkedTxn[] { };
                objLinkedTxn = new LinkedTxn[]
                {
                        new LinkedTxn()
                        {
                            TxnId = invoice.erpInvoiceId,
                            TxnType = "Invoice",
                        }
                };
                lines.Add(
                        new Line()
                        {
                            LinkedTxn = objLinkedTxn,
                            DetailType = LineDetailTypeEnum.PaymentLineDetail,
                            DetailTypeSpecified = true,
                            Amount = invoice.amountPaid ?? 0,
                            AmountSpecified = true,
                        });
            }
            objPayment.Line = lines.ToArray();

            return objPayment;
        }
    }
}
