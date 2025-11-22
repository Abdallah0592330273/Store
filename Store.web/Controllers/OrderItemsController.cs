using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Domain.Interfaces;
using Domain.Entities;
using AutoMapper;
using Store.Api.DTOs; // Assuming OrderItem DTOs are here
using System.Net;

namespace Store.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]




    public class OrderItemsController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public OrderItemsController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var entities = await _unitOfWork.OrderItems.GetAllAsync();
            return Ok(_mapper.Map<IEnumerable<OrderItemDto>>(entities));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var entity = await _unitOfWork.OrderItems.GetByIdAsync(id);
            if (entity == null) return NotFound();
            return Ok(_mapper.Map<OrderItemDto>(entity));
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateOrderItemDto dto)
        {
            var entity = _mapper.Map<OrderItem>(dto);

            await _unitOfWork.OrderItems.AddAsync(entity);
            await _unitOfWork.CompleteAsync();

            var responseDto = _mapper.Map<OrderItemDto>(entity);
            return CreatedAtAction(nameof(GetById), new { id = entity.orderItemId }, responseDto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] OrderItemDto dto) // Note: DTO for update is OrderItemDto in your profile
        {
            if (id != dto.OrderItemId) return BadRequest();
            var entity = await _unitOfWork.OrderItems.GetByIdAsync(id);
            if (entity == null) return NotFound();

            _mapper.Map(dto, entity);

            _unitOfWork.OrderItems.Update(entity);
            await _unitOfWork.CompleteAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var entity = await _unitOfWork.OrderItems.GetByIdAsync(id);
            if (entity == null) return NotFound();

            _unitOfWork.OrderItems.Remove(entity);
            await _unitOfWork.CompleteAsync();
            return NoContent();
        }
    }
   
}
