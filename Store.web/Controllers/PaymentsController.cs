using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
    using Domain.Interfaces;
    using Domain.Entities;
    using AutoMapper;
    using Store.Api.DTOs.Payments;
    using System.Net;


namespace Store.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentsController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public PaymentsController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var entities = await _unitOfWork.Payments.GetAllAsync();
            return Ok(_mapper.Map<IEnumerable<PaymentDto>>(entities));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var entity = await _unitOfWork.Payments.GetByIdAsync(id);
            if (entity == null) return NotFound();
            return Ok(_mapper.Map<PaymentDto>(entity));
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreatePaymentDto dto)
        {
            var entity = _mapper.Map<Payment>(dto);

            await _unitOfWork.Payments.AddAsync(entity);
            await _unitOfWork.CompleteAsync();

            var responseDto = _mapper.Map<PaymentDto>(entity);
            return CreatedAtAction(nameof(GetById), new { id = entity.paymentId }, responseDto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdatePaymentDto dto)
        {
            if (id != dto.PaymentId) return BadRequest();
            var entity = await _unitOfWork.Payments.GetByIdAsync(id);
            if (entity == null) return NotFound();

            _mapper.Map(dto, entity);

            _unitOfWork.Payments.Update(entity);
            await _unitOfWork.CompleteAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var entity = await _unitOfWork.Payments.GetByIdAsync(id);
            if (entity == null) return NotFound();

            _unitOfWork.Payments.Remove(entity);
            await _unitOfWork.CompleteAsync();
            return NoContent();
        }
    }
}
