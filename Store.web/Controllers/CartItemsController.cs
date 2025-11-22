using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

    using Microsoft.AspNetCore.Mvc;
    using Domain.Interfaces;
    using Domain.Entities;
    using AutoMapper;
    using Store.Api.DTOs.CartItems;
    using System.Net;

namespace Store.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartItemsController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CartItemsController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var entities = await _unitOfWork.CartItems.GetAllAsync();
            return Ok(_mapper.Map<IEnumerable<CartItemDto>>(entities));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var entity = await _unitOfWork.CartItems.GetByIdAsync(id);
            if (entity == null) return NotFound();
            return Ok(_mapper.Map<CartItemDto>(entity));
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateCartItemDto dto)
        {
            var entity = _mapper.Map<CartItem>(dto);

            await _unitOfWork.CartItems.AddAsync(entity);
            await _unitOfWork.CompleteAsync();

            var responseDto = _mapper.Map<CartItemDto>(entity);
            return CreatedAtAction(nameof(GetById), new { id = entity.cartItemId }, responseDto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateCartItemDto dto)
        {
            if (id != dto.CartItemId) return BadRequest();
            var entity = await _unitOfWork.CartItems.GetByIdAsync(id);
            if (entity == null) return NotFound();

            _mapper.Map(dto, entity);

            _unitOfWork.CartItems.Update(entity);
            await _unitOfWork.CompleteAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var entity = await _unitOfWork.CartItems.GetByIdAsync(id);
            if (entity == null) return NotFound();

            _unitOfWork.CartItems.Remove(entity);
            await _unitOfWork.CompleteAsync();
            return NoContent();
        }
    }
   
}
