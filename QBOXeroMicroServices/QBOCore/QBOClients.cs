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

        #region Invoice
        public List<Invoice> GetQBOInvoices(List<Invoice> invoices, string companyAccountID, QueryService<Invoice> qboQueryService, QBOInvoiceReqViewModel qBOInvoiceReq, int startPosition = 1, int maxresults = 500)
        {
            try
            {
                string query = "";
                if (qBOInvoiceReq != null && qBOInvoiceReq.ToDate != null && qBOInvoiceReq.FromDate != null && Convert.ToDateTime(qBOInvoiceReq.ToDate).Year > 1970 && Convert.ToDateTime(qBOInvoiceReq.FromDate).Year > 1970)
                {
                    query = string.Format("SELECT * FROM Invoice where MetaData.CreateTime >= '" + Convert.ToDateTime(qBOInvoiceReq.FromDate).ToString("yyyy-MM-dd") + "' AND MetaData.LastUpdatedTime <= '" + Convert.ToDateTime(qBOInvoiceReq.ToDate).ToString("yyyy-MM-dd") + "' startPosition " + startPosition + " maxresults " + maxresults);
                }
                else if (qBOInvoiceReq != null && qBOInvoiceReq.ToDate != null && Convert.ToDateTime(qBOInvoiceReq.ToDate).Year > 1970)
                {
                    query = string.Format("SELECT * FROM Invoice where MetaData.LastUpdatedTime >= '" + Convert.ToDateTime(qBOInvoiceReq.ToDate).ToString("yyyy-MM-dd") + "' startPosition " + startPosition + " maxresults " + maxresults);
                }
                else
                {
                    query = string.Format("SELECT * FROM Invoice order by id asc startPosition " + startPosition + " maxresults 500");
                }

                var lstcustomers = qboQueryService.ExecuteIdsQuery(query, QueryOperationType.query).ToList();
                invoices.AddRange(lstcustomers);

                if (lstcustomers.Count == maxresults)
                {
                    GetQBOInvoices(invoices, companyAccountID, qboQueryService, qBOInvoiceReq, startPosition: startPosition + maxresults, maxresults: maxresults);
                }
                return invoices;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public Invoice CreateUpdateInvoice(Invoice invoice, string realmId, string accessToken)
        {
            try
            {
                var objDataService = new DataService(QBOAuthClient.Service(realmId, accessToken));
                if (!string.IsNullOrEmpty(invoice.Id))
                {
                    var objInvoice = (new QueryService<Invoice>(QBOAuthClient.Service(realmId, accessToken))).ExecuteIdsQuery("SELECT * FROM Invoice WHERE Id ='" + (!string.IsNullOrEmpty(invoice.Id) ? invoice.Id.Replace("'", "\\'") : "") + "'", QueryOperationType.query).FirstOrDefault();
                    if (objInvoice != null)
                    {
                        invoice = objDataService.Update(invoice);
                    }
                }
                else
                {
                    invoice = objDataService.Add(invoice);
                }
                return invoice;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public Invoice DeleteInvoiceById(string invoiceId, string realmId, string accessToken)
        {
            try
            {
                Invoice invoice = null;
                var objDataService = new DataService(QBOAuthClient.Service(realmId, accessToken));
                if (!string.IsNullOrEmpty(invoiceId))
                {
                    var objInvoice = (new QueryService<Invoice>(QBOAuthClient.Service(realmId, accessToken))).ExecuteIdsQuery("SELECT * FROM Invoice WHERE Id ='" + invoiceId + "'", QueryOperationType.query).FirstOrDefault();
                    if (objInvoice != null)
                    {
                        invoice = objDataService.Delete(objInvoice);
                    }
                }
                return invoice;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region Payment
        public List<Payment> GetQBOPayments(List<Payment> payments, string companyAccountID, QueryService<Payment> qboQueryService, QBOPaymentReqViewModel qBOPaymentReq, int startPosition = 1, int maxresults = 500)
        {
            try
            {
                string query = "";
                if (qBOPaymentReq != null && qBOPaymentReq.ToDate != null && qBOPaymentReq.FromDate != null && Convert.ToDateTime(qBOPaymentReq.ToDate).Year > 1970 && Convert.ToDateTime(qBOPaymentReq.FromDate).Year > 1970)
                {
                    query = string.Format("SELECT * FROM Payment where MetaData.CreateTime >= '" + Convert.ToDateTime(qBOPaymentReq.FromDate).ToString("yyyy-MM-dd") + "' AND MetaData.LastUpdatedTime <= '" + Convert.ToDateTime(qBOPaymentReq.ToDate).ToString("yyyy-MM-dd") + "' startPosition " + startPosition + " maxresults " + maxresults);
                }
                else if (qBOPaymentReq != null && qBOPaymentReq.ToDate != null && Convert.ToDateTime(qBOPaymentReq.ToDate).Year > 1970)
                {
                    query = string.Format("SELECT * FROM Payment where MetaData.LastUpdatedTime >= '" + Convert.ToDateTime(qBOPaymentReq.ToDate).ToString("yyyy-MM-dd") + "' startPosition " + startPosition + " maxresults " + maxresults);
                }
                else
                {
                    query = string.Format("SELECT * FROM Payment order by id asc startPosition " + startPosition + " maxresults 500");
                }

                var lstPayments = qboQueryService.ExecuteIdsQuery(query, QueryOperationType.query).ToList();
                payments.AddRange(lstPayments);

                if (lstPayments.Count == maxresults)
                {
                    GetQBOPayments(payments, companyAccountID, qboQueryService, qBOPaymentReq, startPosition: startPosition + maxresults, maxresults: maxresults);
                }
                return payments;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public Payment CreateUpdatePayment(Payment payment, string realmId, string accessToken)
        {
            try
            {
                var objDataService = new DataService(QBOAuthClient.Service(realmId, accessToken));
                if (!string.IsNullOrEmpty(payment.Id))
                {
                    var objPayment = (new QueryService<Payment>(QBOAuthClient.Service(realmId, accessToken))).ExecuteIdsQuery("SELECT * FROM Payment WHERE Id ='" + (!string.IsNullOrEmpty(payment.Id) ? payment.Id.Replace("'", "\\'") : "") + "'", QueryOperationType.query).FirstOrDefault();
                    if (objPayment != null)
                    {
                        payment = objDataService.Update(payment);
                    }
                }
                else
                {
                    payment = objDataService.Add(payment);
                }
                return payment;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public Payment DeletePaymentById(string paymentId, string realmId, string accessToken)
        {
            try
            {
                Payment payment = null;
                var objDataService = new DataService(QBOAuthClient.Service(realmId, accessToken));
                if (!string.IsNullOrEmpty(paymentId))
                {
                    var objPayment = (new QueryService<Payment>(QBOAuthClient.Service(realmId, accessToken))).ExecuteIdsQuery("SELECT * FROM Payment WHERE Id ='" + paymentId + "'", QueryOperationType.query).FirstOrDefault();
                    if (objPayment != null)
                    {
                        payment = objDataService.Delete(objPayment);
                    }
                }
                return payment;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion
    }
}
