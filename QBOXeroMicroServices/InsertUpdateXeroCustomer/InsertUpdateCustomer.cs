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
using static BusinessAccessLayer.Common.Enums;
using BusinessAccessLayer.ViewModel;
using System.Collections.Generic;
using Xero.NetStandard.OAuth2.Model.Accounting;

namespace InsertUpdateXeroCustomer
{
    public class InsertUpdateCustomer
    {
        private readonly IConnectionService _connectionService;
        public readonly XeroAuthClient _xeroAuthClient;
        public readonly XeroClients _xeroClients;
        public InsertUpdateCustomer(IConnectionService connectionService, XeroAuthClient xeroAuthClient, XeroClients xeroClients)
        {
            _connectionService = connectionService;
            _xeroAuthClient = xeroAuthClient;
            _xeroClients = xeroClients;
        }
        [FunctionName("InsertUpdateCustomer")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");
            var objResultPT = new XeroCustomerSyncResult();
            try
            {
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                if (!string.IsNullOrEmpty(requestBody))
                {
                    var requestModel = JsonConvert.DeserializeObject<XeroCustomerInsertUpdateReqViewModel>(requestBody);
                    if (!string.IsNullOrEmpty(requestModel.TenantId))
                    {
                        var objConnectionData = await _connectionService.GetXeroConnectionByTenantId(requestModel.TenantId);
                        if (objConnectionData != null)
                        {
                            foreach (var customer in requestModel.Contacts)
                            {
                                var accessToken = await _xeroAuthClient.GetAccessToken(objConnectionData.TenantId);
                                if (accessToken != null)
                                {
                                    var customerMapping = XeroCustomerMapping(customer);
                                    var objcustomers = await _xeroClients.CreateUpdateCustomer(customerMapping, objConnectionData.TenantId, accessToken);

                                    objResultPT.ResultMsg = Message.CustomerInsertUpdateMessage;
                                    objResultPT.TransactionStatus = ResponseStatus.Success;
                                    objResultPT.ReturnObject = objcustomers;
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

        private Contact XeroCustomerMapping(XeroContactModel model)
        {
            Contact contact = new Contact();
            contact.ContactID = model.ContactID;
            contact.Name = model.Name;
            contact.FirstName = model.FirstName;
            contact.LastName = model.LastName;
            contact.EmailAddress = model.EmailAddress;
            contact.Phones = new List<Phone>()
            {
                new Phone()
                {
                    PhoneNumber = !string.IsNullOrEmpty(model.PhoneNumber) ? model.PhoneNumber : ""
                },
            };
            contact.Addresses = new List<Address>() 
            {
                new Address()
                {
                    AddressLine1 = model.Address != null && !string.IsNullOrEmpty(model.Address.AddressLine1) ? model.Address.AddressLine1 : "",
                    AddressLine2 = model.Address != null && !string.IsNullOrEmpty(model.Address.AddressLine2) ? model.Address.AddressLine2 : "",
                    AddressLine3 = model.Address != null && !string.IsNullOrEmpty(model.Address.AddressLine3) ? model.Address.AddressLine3 : "",
                    AddressLine4 = model.Address != null && !string.IsNullOrEmpty(model.Address.AddressLine4) ? model.Address.AddressLine4 : "",
                    City = model.Address != null && !string.IsNullOrEmpty(model.Address.City) ? model.Address.City : "",
                    Region = model.Address != null && !string.IsNullOrEmpty(model.Address.Region) ? model.Address.Region : "",
                    PostalCode = model.Address != null && !string.IsNullOrEmpty(model.Address.PostalCode) ? model.Address.PostalCode : "",
                    Country = model.Address != null && !string.IsNullOrEmpty(model.Address.Country) ? model.Address.Country : "",
                }
            };
            return contact;
        }
    }
}
