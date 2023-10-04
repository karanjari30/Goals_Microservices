using BusinessAccessLayer.Common;
using BusinessAccessLayer.ViewModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static BusinessAccessLayer.Common.Enums;
using System.Text;
using Newtonsoft.Json;

namespace QBOXeroMicroServices.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        #region QuickBooks api
        [HttpPost]
        [Route("/GetQBOPayment")]
        public async Task<IActionResult> GetQBOPayment([FromBody] QBOPaymentReqViewModel model)
        {
            var objResponse = new PaymentSyncResult();
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
                    var response = await httpClient.PostAsync(AppConfiguration.QBOPaymentGet, content);

                    if (response.IsSuccessStatusCode)
                    {
                        var result = await response.Content.ReadAsStringAsync();
                        objResponse = JsonConvert.DeserializeObject<PaymentSyncResult>(result);
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
        [Route("/InsertUpdateQBOPayment")]
        public async Task<IActionResult> InsertUpdateQBOPayment([FromBody] QBOPaymentInsertUpdateReqViewModel model)
        {
            var objResponse = new PaymentSyncResult();
            try
            {
                if (model == null)
                {
                    objResponse.TransactionStatus = ResponseStatus.BadRequest;
                    objResponse.ResultMsg = string.Format(Message.InvalidData);
                    return BadRequest(objResponse);
                }

                if (model.Payments == null && model.Payments.Count == 0)
                {
                    objResponse.TransactionStatus = ResponseStatus.Success;
                    objResponse.ResultMsg = string.Format(Message.InvoiceIdMessage);
                    return Ok(objResponse);
                }

                using (var httpClient = new HttpClient())
                {
                    var json = JsonConvert.SerializeObject(model);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");
                    var response = await httpClient.PostAsync(AppConfiguration.QBOPaymentInsertUpdate, content);

                    if (response.IsSuccessStatusCode)
                    {
                        var result = await response.Content.ReadAsStringAsync();
                        objResponse = JsonConvert.DeserializeObject<PaymentSyncResult>(result);
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
        [Route("/DeleteQBOPayment")]
        public async Task<IActionResult> DeleteQBOPayment([FromBody] QBOPaymentDeleteReqViewModel model)
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
                    var response = await httpClient.PostAsync(AppConfiguration.QBOPaymentDelete, content);

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
        #endregion

        #region Xero api
        [HttpPost]
        [Route("/GetXeroPayment")]
        public async Task<IActionResult> GetXeroPayment([FromBody] XeroReqViewModel model)
        {
            var objResponse = new XeroPaymentSyncResult();
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
                    var response = await httpClient.PostAsync(AppConfiguration.XeroPaymentGet, content);

                    if (response.IsSuccessStatusCode)
                    {
                        var result = await response.Content.ReadAsStringAsync();
                        objResponse = JsonConvert.DeserializeObject<XeroPaymentSyncResult>(result);
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
        [Route("/InsertUpdateXeroPayment")]
        public async Task<IActionResult> InsertUpdateXeroPayment([FromBody] XeroPaymentInsertUpdateReqViewModel model)
        {
            var objResponse = new XeroPaymentSyncResult();
            try
            {
                if (model == null)
                {
                    objResponse.TransactionStatus = ResponseStatus.BadRequest;
                    objResponse.ResultMsg = string.Format(Message.InvalidData);
                    return BadRequest(objResponse);
                }

                if (model.Payments == null && model.Payments.Count == 0)
                {
                    objResponse.TransactionStatus = ResponseStatus.Success;
                    objResponse.ResultMsg = string.Format(Message.InvoiceIdMessage);
                    return Ok(objResponse);
                }

                using (var httpClient = new HttpClient())
                {
                    var json = JsonConvert.SerializeObject(model);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");
                    var response = await httpClient.PostAsync(AppConfiguration.XeroPaymentInsertUpdate, content);

                    if (response.IsSuccessStatusCode)
                    {
                        var result = await response.Content.ReadAsStringAsync();
                        objResponse = JsonConvert.DeserializeObject<XeroPaymentSyncResult>(result);
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
        [Route("/DeleteXeroPayment")]
        public async Task<IActionResult> DeleteXeroPayment([FromBody] XeroPaymentDeleteReqViewModel model)
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
                    var response = await httpClient.PostAsync(AppConfiguration.XeroPaymentDelete, content);

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
        #endregion
    }
}
