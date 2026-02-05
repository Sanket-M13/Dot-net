using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace EVChargerAPI.Controllers
{
    [ApiController]
    [Route("api/payment")]
    [Authorize]
    public class PaymentController : ControllerBase
    {
        [HttpPost("create-order")]
        public IActionResult CreateOrder([FromBody] dynamic orderData)
        {
            var orderId = Guid.NewGuid().ToString();
            
            return Ok(new
            {
                orderId = orderId,
                amount = orderData.amount,
                currency = "INR",
                status = "created"
            });
        }

        [HttpPost("verify")]
        public IActionResult VerifyPayment([FromBody] dynamic paymentData)
        {
            return Ok(new
            {
                status = "success",
                paymentId = paymentData.paymentId,
                verified = true
            });
        }
    }
}