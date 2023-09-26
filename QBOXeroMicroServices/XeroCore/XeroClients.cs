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
    }
}
