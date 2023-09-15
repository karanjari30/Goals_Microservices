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

namespace InsertUpdateQBOInvoice
{
    public class InsertUpdateInvoice
    {
        private readonly IConnectionService _connectionService;
        public readonly QBOAuthClient _qoAuthClient;
        public readonly QBOClients _qboClients;
        public InsertUpdateInvoice(IConnectionService connectionService, QBOAuthClient qboAuthClient, QBOClients qboClients)
        {
            _connectionService = connectionService;
            _qoAuthClient = qboAuthClient;
            _qboClients = qboClients;
        }

        [FunctionName("InsertUpdateInvoice")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");
            var objResultPT = new InvoiceSyncResult();
            try
            {
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                if (!string.IsNullOrEmpty(requestBody))
                {
                    var requestModel = JsonConvert.DeserializeObject<QBOInvoiceInsertUpdateReqViewModel>(requestBody);
                    if (!string.IsNullOrEmpty(requestModel.AccountingCompanyId))
                    {
                        var objConnectionData = await _connectionService.GetQBOAuthConfigureByCompanyId(requestModel.AccountingCompanyId.ToString());
                        if (objConnectionData != null)
                        {
                            List<Invoice> invoiceList = new List<Invoice>();
                            foreach (var invoice in requestModel.invoices)
                            {
                                var accessToken = await _qoAuthClient.GetAccessToken(objConnectionData.ERPCompanyId);
                                if (accessToken != null)
                                {
                                    var invoiceMapping = QBOInvoiceMapping(invoice);
                                    var objinvoice = _qboClients.CreateUpdateInvoice(invoiceMapping, objConnectionData.ERPCompanyId, accessToken);
                                    if (objinvoice != null)
                                    {
                                        invoiceList.Add(objinvoice);
                                    }
                                }
                                else
                                {
                                    objResultPT.ResultMsg = Message.QBOTokenExpire;
                                    objResultPT.TransactionStatus = ResponseStatus.Error;
                                }
                            }
                            objResultPT.ResultMsg = Message.InvoiceInsertUpdateMessage;
                            objResultPT.TransactionStatus = ResponseStatus.Success;
                            objResultPT.ReturnObject = invoiceList;
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

        private Invoice QBOInvoiceMapping(InvoiceReqViewModel reqViewModel)
        {
            Invoice invoice = new Invoice();
            List<Line> lines = new List<Line>();

            if (!string.IsNullOrEmpty(reqViewModel.Id))
            {
                invoice.Id = reqViewModel.Id ?? "";
            }
            invoice.DocNumber = reqViewModel.DocNumber ?? "";
            invoice.TxnDate = reqViewModel.TxnDate;
            invoice.CustomerRef = new ReferenceType()
            {
                Value = reqViewModel.CustomerRefValue ?? ""
            };
            
            foreach(var objLine in reqViewModel.Line) 
            {
                SalesItemLineDetail salesItemLineDetail = new SalesItemLineDetail()
                {
                    ItemRef = new ReferenceType()
                    {
                        Value = objLine.SalesItemLineDetail != null && !string.IsNullOrEmpty(objLine.SalesItemLineDetail.ItemRefValue) ? objLine.SalesItemLineDetail.ItemRefValue : ""
                    }
                };

                lines.Add(
                    new Line()
                    {
                        DetailType = LineDetailTypeEnum.SalesItemLineDetail,
                        DetailTypeSpecified = true,
                        Amount = (decimal)(objLine.Qty * objLine.UnitPrice),
                        AmountSpecified = true,
                        AnyIntuitObject = salesItemLineDetail,
                        Description = objLine.Description ?? "",
                    });
            }
            invoice.Line = lines.ToArray();

            return invoice;
        }
    }
}
