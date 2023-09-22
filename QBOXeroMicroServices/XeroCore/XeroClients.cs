using BusinessAccessLayer.ViewModel;
using Xero.NetStandard.OAuth2.Api;
using Xero.NetStandard.OAuth2.Model.Accounting;

namespace XeroCore
{
    public class XeroClients
    {
        #region Contact
        public async Task<List<Contact>> GetContactsData(XeroCustomerReqViewModel objRequestModel, string accessToken, string tenantId)
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
    }
}
