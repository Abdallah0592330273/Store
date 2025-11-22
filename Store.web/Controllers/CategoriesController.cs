using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using Domain.Entities;
using Domain.Interfaces;
using Store.Api.DTOs; // Assuming Category DTOs are here
using Store.Api.DTOs.Caregories;
using System.Net;

namespace Store.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CategoriesController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var entities = await _unitOfWork.Categories.GetAllAsync();
            return Ok(_mapper.Map<IEnumerable<CategoryDto>>(entities));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var entity = await _unitOfWork.Categories.GetByIdAsync(id);
            if (entity == null) return NotFound();
            return Ok(_mapper.Map<CategoryDto>(entity));
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateCategoryDto dto)
        {
            var entity = _mapper.Map<Category>(dto);

            await _unitOfWork.Categories.AddAsync(entity);
            await _unitOfWork.CompleteAsync();

            var responseDto = _mapper.Map<CategoryDto>(entity);
            return CreatedAtAction(nameof(GetById), new { id = entity.categoryId }, responseDto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateCategoryDto dto)
        {
            if (id != dto.CategoryId) return BadRequest();
            var entity = await _unitOfWork.Categories.GetByIdAsync(id);
            if (entity == null) return NotFound();

            _mapper.Map(dto, entity);

            _unitOfWork.Categories.Update(entity);
            await _unitOfWork.CompleteAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var entity = await _unitOfWork.Categories.GetByIdAsync(id);
            if (entity == null) return NotFound();

            _unitOfWork.Categories.Remove(entity);
            await _unitOfWork.CompleteAsync();
            return NoContent();
        }
    }
}
