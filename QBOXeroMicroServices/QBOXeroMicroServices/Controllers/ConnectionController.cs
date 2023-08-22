using BusinessAccessLayer.Common;
using Microsoft.AspNetCore.Mvc;
using static BusinessAccessLayer.Common.Enums;

namespace QBOXeroMicroServices.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ConnectionController : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> Index(Guid companyId)
        {
            var objResponse = new ResultPT();
            try
            {
                if(companyId == Guid.Empty)
                {
                    objResponse.ResponseStatus = ResponseStatus.Error;
                    objResponse.Message = Message.CompanyIdMessage;
                    return Ok(objResponse);
                }
                else
                {

                }
            }
            catch (Exception ex)
            {
                objResponse.ResponseStatus = ResponseStatus.Error;
                objResponse.Message = GetCommonMessage.GetExceptionMessage(ex);
            }
            return Ok(objResponse);
        }
    }
}
