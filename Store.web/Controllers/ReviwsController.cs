using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using Domain.Interfaces;
using Domain.Entities;
using AutoMapper;
using Store.Api.DTOs.Reviws;
using Store.Api.DTOs;
using System.Net;
namespace Store.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewsController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ReviewsController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var entities = await _unitOfWork.Reviews.GetAllAsync();
            return Ok(_mapper.Map<IEnumerable<ReviewDto>>(entities));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var entity = await _unitOfWork.Reviews.GetByIdAsync(id);
            if (entity == null) return NotFound();
            return Ok(_mapper.Map<ReviewDto>(entity));
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateReviwDto dto)
        {
            // Map to Reviw entity
            var entity = _mapper.Map<Reviw>(dto);

            await _unitOfWork.Reviews.AddAsync(entity);
            await _unitOfWork.CompleteAsync();

            // Map the new entity back to ReviewDto
            var responseDto = _mapper.Map<ReviewDto>(entity);
            return CreatedAtAction(nameof(GetById), new { id = entity.reviewId }, responseDto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateReviwDto dto)
        {
            if (id != dto.ReviwId) return BadRequest();
            var entity = await _unitOfWork.Reviews.GetByIdAsync(id);
            if (entity == null) return NotFound();

            _mapper.Map(dto, entity);

            _unitOfWork.Reviews.Update(entity);
            await _unitOfWork.CompleteAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var entity = await _unitOfWork.Reviews.GetByIdAsync(id);
            if (entity == null) return NotFound();

            _unitOfWork.Reviews.Remove(entity);
            await _unitOfWork.CompleteAsync();
            return NoContent();
        }
    }
    public class ReviwsController : ControllerBase
    {
    }
}
