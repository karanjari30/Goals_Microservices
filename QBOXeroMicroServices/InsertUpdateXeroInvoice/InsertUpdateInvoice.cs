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
using XeroCore;
using Xero.NetStandard.OAuth2.Model.Accounting;
using System.Collections.Generic;

namespace InsertUpdateXeroInvoice
{
    public class InsertUpdateInvoice
    {
        private readonly IConnectionService _connectionService;
        public readonly XeroAuthClient _xeroAuthClient;
        public readonly XeroClients _xeroClients;
        public InsertUpdateInvoice(IConnectionService connectionService, XeroAuthClient xeroAuthClient, XeroClients xeroClients)
        {
            _connectionService = connectionService;
            _xeroAuthClient = xeroAuthClient;
            _xeroClients = xeroClients;
        }
        [FunctionName("InsertUpdateInvoice")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");
            var objResultPT = new XeroInvoiceSyncResult();
            try
            {
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                if (!string.IsNullOrEmpty(requestBody))
                {
                    var requestModel = JsonConvert.DeserializeObject<XeroInvoiceInsertUpdateReqViewModel>(requestBody);
                    if (!string.IsNullOrEmpty(requestModel.TenantId))
                    {
                        var objConnectionData = await _connectionService.GetXeroConnectionByTenantId(requestModel.TenantId);
                        if (objConnectionData != null)
                        {
                            foreach(var invoice in requestModel.Invoices)
                            {
                                var accessToken = await _xeroAuthClient.GetAccessToken(objConnectionData.TenantId);
                                if (accessToken != null)
                                {
                                    var invoiceMapping = XeroInvoiceMapping(invoice);
                                    var objInvoices = await _xeroClients.CreateUpdateInvoice(invoiceMapping, objConnectionData.TenantId, accessToken);

                                    objResultPT.ResultMsg = Message.InvoiceInsertUpdateMessage;
                                    objResultPT.TransactionStatus = ResponseStatus.Success;
                                    objResultPT.ReturnObject = objInvoices;
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

        private Xero.NetStandard.OAuth2.Model.Accounting.Invoice XeroInvoiceMapping(XeroInvoiceModel xeroInvoice)
        {
            var invoice = new Invoice();
            var lineItems = new List<LineItem>();

            invoice.Type = Invoice.TypeEnum.ACCREC;
            invoice.Contact = new Contact()
            {
                ContactID = xeroInvoice.ContactId != null ? new Guid(xeroInvoice.ContactId) : new Guid()
            };
            invoice.Date = xeroInvoice.InvoiceDate;
            invoice.Reference = xeroInvoice.Reference;
            invoice.Status = xeroInvoice.Status;
            
            foreach (var line in xeroInvoice.LineItems)
            {
                var lineItem = new LineItem();
                lineItem.Description = line.Description;
                lineItem.Quantity = line.Quantity;
                lineItem.UnitAmount = line.UnitAmount;
                lineItem.AccountCode = line.AccountCode;

                lineItems.Add(lineItem);
            }
            invoice.LineItems = lineItems;

            return invoice;
        }
    }
}
