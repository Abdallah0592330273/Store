using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using Microsoft.AspNetCore.Mvc;
using Domain.Interfaces;
using Domain.Entities;
using AutoMapper;
using Store.Api.DTOs; // Assuming Order DTOs are here
using System.Net;

namespace Store.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public OrdersController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var entities = await _unitOfWork.Orders.GetAllAsync();
            return Ok(_mapper.Map<IEnumerable<OrderDto>>(entities));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var entity = await _unitOfWork.Orders.GetByIdAsync(id);
            if (entity == null) return NotFound();
            return Ok(_mapper.Map<OrderDto>(entity));
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] OrderDto dto) // Note: Using OrderDto for Create based on your profile
        {
            var entity = _mapper.Map<Order>(dto);

            await _unitOfWork.Orders.AddAsync(entity);
            await _unitOfWork.CompleteAsync();

            var responseDto = _mapper.Map<OrderDto>(entity);
            return CreatedAtAction(nameof(GetById), new { id = entity.orderId }, responseDto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] OrderDto dto) // Note: Using OrderDto for Update based on your profile
        {
            if (id != dto.OrderId) return BadRequest();
            var entity = await _unitOfWork.Orders.GetByIdAsync(id);
            if (entity == null) return NotFound();

            _mapper.Map(dto, entity);

            _unitOfWork.Orders.Update(entity);
            await _unitOfWork.CompleteAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var entity = await _unitOfWork.Orders.GetByIdAsync(id);
            if (entity == null) return NotFound();

            _unitOfWork.Orders.Remove(entity);
            await _unitOfWork.CompleteAsync();
            return NoContent();
        }
    }
}
