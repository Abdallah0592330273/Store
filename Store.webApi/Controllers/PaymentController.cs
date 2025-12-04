using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Store.DataAccess.Entities;
using Store.DataAccess.UnitOfWork;
using Store.StoreWebApi.Controllers;
using System.Linq;
namespace Store.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PaymentController : BaseApiController
    {
        private readonly IUnitOfWork _unitOfWork;

        public PaymentController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // GET: api/Payment/byOrder/5
        [HttpGet("byOrder/{orderId}")]
        public async Task<ActionResult<Payment>> GetPaymentByOrder(int orderId)
        {
            var userId = GetCurrentUserId(); // 🔑 Get User ID

            // WORKFLOW: Check if the order exists and belongs to the user.
            var order = await _unitOfWork.OrderRepo.GetByPropertyAsync(o => o.Id == orderId && o.UserId == userId);

            if (order == null) return NotFound("Order not found or unauthorized.");

            // STORE LOGIC: Retrieve the payment record linked to that order.
            var payment = await _unitOfWork.PaymentRepo.GetByPropertyAsync(p => p.OrderId == orderId);

            if (payment == null) return NotFound("Payment record not found for this order.");

            return Ok(payment);
        }

        // NOTE: A real-world payment system would use a secure, unauthenticated webhook 
        // endpoint for status updates, not a simple PUT from the user.
    }
}