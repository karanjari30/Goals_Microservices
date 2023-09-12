using BusinessAccessLayer.ViewModel;
using Intuit.Ipp.Data;
using Intuit.Ipp.DataService;
using Intuit.Ipp.QueryFilter;

namespace QBOCore
{
    public class QBOClients
    {
        #region Customer
        public List<Customer> GetQBOCustomers(List<Customer> customers, string companyAccountID, QueryService<Customer> qboQueryService, QBOCustomerReqViewModel qBOCustomerReq, int startPosition = 1, int maxresults = 500)
        {
            try
            {
                string query = "";
                if (qBOCustomerReq != null && qBOCustomerReq.ToDate != null && qBOCustomerReq.FromDate != null && Convert.ToDateTime(qBOCustomerReq.ToDate).Year > 1970 && Convert.ToDateTime(qBOCustomerReq.FromDate).Year > 1970)
                {
                    query = string.Format("SELECT * FROM Customer where MetaData.CreateTime >= '" + qBOCustomerReq.FromDate + "' AND MetaData.LastUpdatedTime <= '" + qBOCustomerReq.ToDate + "' AND Active in(true,false) startPosition " + startPosition + " maxresults " + maxresults);
                }
                else if (qBOCustomerReq != null && qBOCustomerReq.ToDate != null && Convert.ToDateTime(qBOCustomerReq.ToDate).Year > 1970)
                {
                    query = string.Format("SELECT * FROM Customer where MetaData.LastUpdatedTime >= '" + qBOCustomerReq.ToDate + "' AND Active in(true,false) startPosition " + startPosition + " maxresults " + maxresults);
                }
                else
                {
                    query = string.Format("SELECT * FROM Customer where Active in(true,false) order by id asc startPosition " + startPosition + " maxresults 500");
                }

                var lstcustomers = qboQueryService.ExecuteIdsQuery(query, QueryOperationType.query).ToList();
                customers.AddRange(lstcustomers);

                if (lstcustomers.Count == maxresults)
                {
                    GetQBOCustomers(customers, companyAccountID, qboQueryService, qBOCustomerReq, startPosition: startPosition + maxresults, maxresults: maxresults);
                }
                return customers;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public Customer CreateUpdateCustomer(Customer customer, string realmId, string accessToken)
        {
            try
            {
                var objDataService = new DataService(QBOAuthClient.Service(realmId, accessToken));
                if (!string.IsNullOrEmpty(customer.Id))
                {
                    var objCustomer = (new QueryService<Customer>(QBOAuthClient.Service(realmId, accessToken))).ExecuteIdsQuery("SELECT * FROM Customer WHERE Id ='" + (!string.IsNullOrEmpty(customer.Id) ? customer.Id.Replace("'", "\\'") : "") + "'", QueryOperationType.query).FirstOrDefault();
                    if (objCustomer != null)
                    {
                        customer = objDataService.Update(customer);
                    }
                }
                else
                {
                    customer = objDataService.Add(customer);
                }
                return customer;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public Customer DeleteCustomerById(string customerId, string realmId, string accessToken)
        {
            try
            {
                Customer customer = new Customer();
                var objDataService = new DataService(QBOAuthClient.Service(realmId, accessToken));
                if (!string.IsNullOrEmpty(customerId))
                {
                    var objCustomer = (new QueryService<Customer>(QBOAuthClient.Service(realmId, accessToken))).ExecuteIdsQuery("SELECT * FROM Customer WHERE Id ='" + customerId + "'", QueryOperationType.query).FirstOrDefault();
                    if (objCustomer != null)
                    {
                        objCustomer.Active = false;
                        customer = objDataService.Update(objCustomer);
                    }
                }
                return customer;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion
    }
}
