using BusinessAccessLayer.Common;
using BusinessAccessLayer.ViewModel;
using Microsoft.AspNetCore.Mvc;
using static BusinessAccessLayer.Common.Enums;
using System.Text;
using Newtonsoft.Json;

namespace QBOXeroMicroServices.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        [HttpPost]
        [Route("/GetQBOCustomer")]
        public async Task<IActionResult> GetQBOCustomer([FromBody] QBOCustomerReqViewModel model)
        {
            var objResponse = new CustomerManaualSyncResult();
            try
            {
                if (model == null)
                {
                    objResponse.TransactionStatus = ResponseStatus.BadRequest;
                    objResponse.ResultMsg = string.Format(Message.InvalidData);
                    return BadRequest(objResponse);
                }

                using (var httpClient = new HttpClient())
                {
                    var json = JsonConvert.SerializeObject(model);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");
                    var response = await httpClient.PostAsync(AppConfiguration.QBOCutomerGet, content);

                    if (response.IsSuccessStatusCode)
                    {
                        var result = await response.Content.ReadAsStringAsync();
                        objResponse = JsonConvert.DeserializeObject<CustomerManaualSyncResult>(result);
                    }
                    else
                    {
                        return StatusCode((int)response.StatusCode);
                    }
                }
            }
            catch (Exception ex)
            {
                objResponse.TransactionStatus = ResponseStatus.Error;
                objResponse.ResultMsg = GetCommonMessage.GetExceptionMessage(ex);
            }
            return Ok(objResponse);
        }

        [HttpPost]
        [Route("/InsertUpdateQBOCustomer")]
        public async Task<IActionResult> InsertUpdateQBOCustomer([FromBody] QBOCustomerInsertUpdateReqViewModel model)
        {
            var objResponse = new CustomerManaualSyncResult();
            try
            {
                if (model == null)
                {
                    objResponse.TransactionStatus = ResponseStatus.BadRequest;
                    objResponse.ResultMsg = string.Format(Message.InvalidData);
                    return BadRequest(objResponse);
                }

                if(model.customers == null && model.customers.Count == 0)
                {
                    objResponse.TransactionStatus = ResponseStatus.Success;
                    objResponse.ResultMsg = string.Format(Message.CustomerIdMessage);
                    return Ok(objResponse);
                }

                using (var httpClient = new HttpClient())
                {
                    var json = JsonConvert.SerializeObject(model);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");
                    var response = await httpClient.PostAsync(AppConfiguration.QBOCustomerInsertUpdate, content);

                    if (response.IsSuccessStatusCode)
                    {
                        var result = await response.Content.ReadAsStringAsync();
                        objResponse = JsonConvert.DeserializeObject<CustomerManaualSyncResult>(result);
                    }
                    else
                    {
                        return StatusCode((int)response.StatusCode);
                    }
                }
            }
            catch (Exception ex)
            {
                objResponse.TransactionStatus = ResponseStatus.Error;
                objResponse.ResultMsg = GetCommonMessage.GetExceptionMessage(ex);
            }
            return Ok(objResponse);
        }

        [HttpDelete]
        [Route("/DeleteQBOCustomer")]
        public async Task<IActionResult> DeleteQBOCustomer([FromBody] QBOCustomerDeleteReqViewModel model)
        {
            var objResponse = new DownloadResultPT();
            try
            {
                if (model == null)
                {
                    objResponse.TransactionStatus = ResponseStatus.BadRequest;
                    objResponse.ResultMsg = string.Format(Message.InvalidData);
                    return BadRequest(objResponse);
                }

                using (var httpClient = new HttpClient())
                {
                    var json = JsonConvert.SerializeObject(model);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");
                    var response = await httpClient.PostAsync(AppConfiguration.QBOCustomerDelete, content);

                    if (response.IsSuccessStatusCode)
                    {
                        var result = await response.Content.ReadAsStringAsync();
                        objResponse = JsonConvert.DeserializeObject<DownloadResultPT>(result);
                    }
                    else
                    {
                        return StatusCode((int)response.StatusCode);
                    }
                }
            }
            catch (Exception ex)
            {
                objResponse.TransactionStatus = ResponseStatus.Error;
                objResponse.ResultMsg = GetCommonMessage.GetExceptionMessage(ex);
            }
            return Ok(objResponse);
        }

        [HttpPost]
        [Route("/GetXeroCustomer")]
        public async Task<IActionResult> GetXeroCustomer([FromBody] XeroReqViewModel model)
        {
            var objResponse = new XeroCustomerSyncResult();
            try
            {
                if (model == null)
                {
                    objResponse.TransactionStatus = ResponseStatus.BadRequest;
                    objResponse.ResultMsg = string.Format(Message.InvalidData);
                    return BadRequest(objResponse);
                }

                using (var httpClient = new HttpClient())
                {
                    var json = JsonConvert.SerializeObject(model);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");
                    var response = await httpClient.PostAsync(AppConfiguration.XeroCutomerGet, content);

                    if (response.IsSuccessStatusCode)
                    {
                        var result = await response.Content.ReadAsStringAsync();
                        objResponse = JsonConvert.DeserializeObject<XeroCustomerSyncResult>(result);
                    }
                    else
                    {
                        return StatusCode((int)response.StatusCode);
                    }
                }
            }
            catch (Exception ex)
            {
                objResponse.TransactionStatus = ResponseStatus.Error;
                objResponse.ResultMsg = GetCommonMessage.GetExceptionMessage(ex);
            }
            return Ok(objResponse);
        }

        [HttpPost]
        [Route("/InsertUpdateXeroCustomer")]
        public async Task<IActionResult> InsertUpdateXeroCustomer([FromBody] XeroCustomerInsertUpdateReqViewModel model)
        {
            var objResponse = new XeroCustomerSyncResult();
            try
            {
                if (model == null)
                {
                    objResponse.TransactionStatus = ResponseStatus.BadRequest;
                    objResponse.ResultMsg = string.Format(Message.InvalidData);
                    return BadRequest(objResponse);
                }

                if (model.Contacts == null && model.Contacts.Count == 0)
                {
                    objResponse.TransactionStatus = ResponseStatus.Success;
                    objResponse.ResultMsg = string.Format(Message.CustomerIdMessage);
                    return Ok(objResponse);
                }

                using (var httpClient = new HttpClient())
                {
                    var json = JsonConvert.SerializeObject(model);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");
                    var response = await httpClient.PostAsync(AppConfiguration.XeroCustomerInsertUpdate, content);

                    if (response.IsSuccessStatusCode)
                    {
                        var result = await response.Content.ReadAsStringAsync();
                        objResponse = JsonConvert.DeserializeObject<XeroCustomerSyncResult>(result);
                    }
                    else
                    {
                        return StatusCode((int)response.StatusCode);
                    }
                }
            }
            catch (Exception ex)
            {
                objResponse.TransactionStatus = ResponseStatus.Error;
                objResponse.ResultMsg = GetCommonMessage.GetExceptionMessage(ex);
            }
            return Ok(objResponse);
        }

        [HttpDelete]
        [Route("/DeleteXeroCustomer")]
        public async Task<IActionResult> DeleteXeroCustomer([FromBody] XeroCustomerDeleteReqViewModel model)
        {
            var objResponse = new DownloadResultPT();
            try
            {
                if (model == null)
                {
                    objResponse.TransactionStatus = ResponseStatus.BadRequest;
                    objResponse.ResultMsg = string.Format(Message.InvalidData);
                    return BadRequest(objResponse);
                }
                if (!string.IsNullOrEmpty(model.ContactId)) {
                    objResponse.TransactionStatus = ResponseStatus.Error;
                    objResponse.ResultMsg = string.Format(Message.XeroCustomerIdMessage);
                    return Ok(objResponse);
                }

                using (var httpClient = new HttpClient())
                {
                    var json = JsonConvert.SerializeObject(model);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");
                    var response = await httpClient.PostAsync(AppConfiguration.XeroCustomerDelete, content);

                    if (response.IsSuccessStatusCode)
                    {
                        var result = await response.Content.ReadAsStringAsync();
                        objResponse = JsonConvert.DeserializeObject<DownloadResultPT>(result);
                    }
                    else
                    {
                        return StatusCode((int)response.StatusCode);
                    }
                }
            }
            catch (Exception ex)
            {
                objResponse.TransactionStatus = ResponseStatus.Error;
                objResponse.ResultMsg = GetCommonMessage.GetExceptionMessage(ex);
            }
            return Ok(objResponse);
        }
    }
}
