using BusinessAccessLayer.ViewModel;
using Xero.NetStandard.OAuth2.Api;
using Xero.NetStandard.OAuth2.Model.Accounting;

namespace XeroCore
{
    public class XeroClients
    {
        #region Contact
        public async Task<List<Contact>> GetContactsData(XeroReqViewModel objRequestModel, string accessToken, string tenantId)
        {
            var xeroAPI = new AccountingApi();
            try
            {
                List<Contact> contactList = new List<Contact>();
                if (objRequestModel != null && objRequestModel.Data != null)
                {
                    int count = 0;
                    bool isContinue = true;
                    do
                    {
                        var checkPageCount = await xeroAPI.GetContactsAsync(accessToken, tenantId, objRequestModel.Data.LastModifiedDate, objRequestModel.Data.Where, objRequestModel.Data.OrderBy, objRequestModel.Data.ContactIDs, objRequestModel.Data.Page, objRequestModel.Data.IncludeArchived, objRequestModel.Data.SummaryOnly, objRequestModel.Data.SearchTerm);
                        if (checkPageCount != null && checkPageCount._Contacts != null && checkPageCount._Contacts.Any())
                        {
                            contactList.AddRange(checkPageCount._Contacts);
                            count = checkPageCount._Contacts.Count;
                            if (count < 100)
                            {
                                isContinue = false;
                            }
                        }
                        else
                        {
                            isContinue = false;
                        }
                    } while (isContinue);
                }
                else
                {
                    var objContacts = await xeroAPI.GetContactsAsync(accessToken, tenantId);
                    contactList = objContacts._Contacts;
                }

                return contactList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<List<Contact>> CreateUpdateCustomer(Contact contact, string tenantId, string accessToken)
        {
            var xeroAPI = new AccountingApi();
            var contacts = new Contacts();
            var contactsList = new List<Contact>();
            contactsList.Add(contact);
            contacts._Contacts = contactsList;

            var objContact = await xeroAPI.UpdateOrCreateContactsAsync(accessToken, tenantId, contacts);

            return objContact._Contacts;
        }

        public async Task<Contact> DeleteCustomerById(string contactId, string tenantId, string accessToken)
        {
            var xeroAPI = new AccountingApi();
            var objContact = await xeroAPI.GetContactAsync(accessToken, tenantId, new Guid(contactId));
            if (objContact != null)
            {
                objContact._Contacts[0].ContactStatus = Contact.ContactStatusEnum.ARCHIVED;
                await xeroAPI.UpdateContactAsync(accessToken, tenantId, new Guid(contactId), objContact);
            }
            else
            {
                return null;
            }
            return objContact._Contacts.FirstOrDefault();
        }
        #endregion

        #region Invoice
        public async Task<List<Invoice>> GetInvoicesData(XeroReqViewModel objRequestModel, string accessToken, string tenantId)
        {
            var xeroAPI = new AccountingApi();
            try
            {
                List<Invoice> invoiceList = new List<Invoice>();
                if (objRequestModel != null && objRequestModel.Data != null)
                {
                    int count = 0;
                    bool isContinue = true;
                    do
                    {
                        var checkPageCount = await xeroAPI.GetInvoicesAsync(accessToken, tenantId, objRequestModel.Data.LastModifiedDate, objRequestModel.Data.Where, objRequestModel.Data.OrderBy,
                            null, null, objRequestModel.Data.ContactIDs, null, objRequestModel.Data.Page, objRequestModel.Data.IncludeArchived, objRequestModel.Data.SummaryOnly);
                        if (checkPageCount != null && checkPageCount._Invoices != null && checkPageCount._Invoices.Any())
                        {
                            invoiceList.AddRange(checkPageCount._Invoices);
                            count = checkPageCount._Invoices.Count;
                            if (count < 100)
                            {
                                isContinue = false;
                            }
                        }
                        else
                        {
                            isContinue = false;
                        }
                    } while (isContinue);
                }
                else
                {
                    var objInvoices = await xeroAPI.GetInvoicesAsync(accessToken, tenantId);
                    invoiceList = objInvoices._Invoices;
                }

                return invoiceList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<List<Invoice>> CreateUpdateInvoice(Invoice invoice, string tenantId, string accessToken)
        {
            var xeroAPI = new AccountingApi();
            var invoices = new Invoices();
            var invoicesList = new List<Invoice>();
            invoicesList.Add(invoice);
            invoices._Invoices = invoicesList;

            var objInvoice = await xeroAPI.UpdateOrCreateInvoicesAsync(accessToken, tenantId, invoices);

            return objInvoice._Invoices;
        }

        public async Task<Invoice> DeleteInvoiceById(string invoiceId, string tenantId, string accessToken)
        {
            var xeroAPI = new AccountingApi();
            var objInvoice = await xeroAPI.GetInvoiceAsync(accessToken, tenantId, new Guid(invoiceId));
            if (objInvoice != null)
            {
                objInvoice._Invoices[0].Status = Invoice.StatusEnum.DELETED;
                await xeroAPI.UpdateInvoiceAsync(accessToken, tenantId, new Guid(invoiceId), objInvoice);
            }
            else
            {
                return null;
            }
            return objInvoice._Invoices.FirstOrDefault();
        }
        #endregion

        #region Payment
        public async Task<List<Payment>> GetPaymentsData(XeroReqViewModel objRequestModel, string accessToken, string tenantId)
        {
            var xeroAPI = new AccountingApi();
            try
            {
                List<Payment> paymentList = new List<Payment>();
                if (objRequestModel != null && objRequestModel.Data != null)
                {
                    int count = 0;
                    bool isContinue = true;
                    do
                    {
                        var checkPageCount = await xeroAPI.GetPaymentsAsync(accessToken, tenantId, objRequestModel.Data.LastModifiedDate, objRequestModel.Data.Where, objRequestModel.Data.OrderBy, objRequestModel.Data.Page);
                        if (checkPageCount != null && checkPageCount._Payments != null && checkPageCount._Payments.Any())
                        {
                            paymentList.AddRange(checkPageCount._Payments);
                            count = checkPageCount._Payments.Count;
                            if (count < 100)
                            {
                                isContinue = false;
                            }
                        }
                        else
                        {
                            isContinue = false;
                        }
                    } while (isContinue);
                }
                else
                {
                    var objPayments = await xeroAPI.GetPaymentsAsync(accessToken, tenantId);
                    paymentList = objPayments._Payments;
                }

                return paymentList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<List<Payment>> CreateUpdatePayment(Payment payment, string tenantId, string accessToken)
        {
            var xeroAPI = new AccountingApi();
            var payments = new Payments();
            var paymentsList = new List<Payment>();
            paymentsList.Add(payment);
            payments._Payments = paymentsList;

            var objPayment = await xeroAPI.CreatePaymentsAsync(accessToken, tenantId, payments);

            return objPayment._Payments;
        }

        public async Task<Payment> DeletePaymentById(string paymentId, string tenantId, string accessToken)
        {
            var xeroAPI = new AccountingApi();

            var objPayment = await xeroAPI.GetPaymentAsync(accessToken, tenantId, new Guid(paymentId));
            if (objPayment != null)
            {
                var paymentDelete = new PaymentDelete();
                paymentDelete.Status = Payment.StatusEnum.DELETED.ToString();

                await xeroAPI.DeletePaymentAsync(accessToken, tenantId, new Guid(paymentId), paymentDelete);
            }
            else
            {
                return null;
            }
            return objPayment._Payments.FirstOrDefault();
        }
        #endregion
    }
}
