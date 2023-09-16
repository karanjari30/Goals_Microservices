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
    public class InvoiceController : ControllerBase
    {
        [HttpPost]
        [Route("/GetQBOInvoice")]
        public async Task<IActionResult> GetQBOInvoice([FromBody] QBOInvoiceReqViewModel model)
        {
            var objResponse = new InvoiceSyncResult();
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
                    var response = await httpClient.PostAsync(AppConfiguration.QBOInvoiceGet, content);

                    if (response.IsSuccessStatusCode)
                    {
                        var result = await response.Content.ReadAsStringAsync();
                        objResponse = JsonConvert.DeserializeObject<InvoiceSyncResult>(result);
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
        [Route("/InsertUpdateQBOInvoice")]
        public async Task<IActionResult> InsertUpdateQBOInvoice([FromBody] QBOInvoiceInsertUpdateReqViewModel model)
        {
            var objResponse = new InvoiceSyncResult();
            try
            {
                if (model == null)
                {
                    objResponse.TransactionStatus = ResponseStatus.BadRequest;
                    objResponse.ResultMsg = string.Format(Message.InvalidData);
                    return BadRequest(objResponse);
                }

                if (model.Invoices == null && model.Invoices.Count == 0)
                {
                    objResponse.TransactionStatus = ResponseStatus.Success;
                    objResponse.ResultMsg = string.Format(Message.InvoiceIdMessage);
                    return Ok(objResponse);
                }

                using (var httpClient = new HttpClient())
                {
                    var json = JsonConvert.SerializeObject(model);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");
                    var response = await httpClient.PostAsync(AppConfiguration.QBOInvoiceInsertUpdate, content);

                    if (response.IsSuccessStatusCode)
                    {
                        var result = await response.Content.ReadAsStringAsync();
                        objResponse = JsonConvert.DeserializeObject<InvoiceSyncResult>(result);
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
        [Route("/DeleteQBOInvoice")]
        public async Task<IActionResult> DeleteQBOInvoice([FromBody] QBOInvoiceDeleteReqViewModel model)
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
                    var response = await httpClient.PostAsync(AppConfiguration.QBOInvoiceDelete, content);

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
