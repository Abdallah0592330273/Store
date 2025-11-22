using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Domain.Interfaces;
using Domain.Entities;
using AutoMapper;
using Store.Api.DTOs.Users;
using System.Net;

namespace Store.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public UsersController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        // GET: api/users
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<UserDto>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetAll()
        {
            var entities = await _unitOfWork.Users.GetAllAsync();
            // Map List<User> to List<UserDto>
            return Ok(_mapper.Map<IEnumerable<UserDto>>(entities));
        }

        // GET: api/users/5
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(UserDto), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> GetById(int id)
        {
            var entity = await _unitOfWork.Users.GetByIdAsync(id);
            if (entity == null) return NotFound();
            // Map User to UserDto
            return Ok(_mapper.Map<UserDto>(entity));
        }

        // POST: api/users
        [HttpPost]
        [ProducesResponseType(typeof(UserDto), (int)HttpStatusCode.Created)]
        public async Task<IActionResult> Create([FromBody] CreateUserDto dto)
        {
            // Map CreateUserDto to User (creates a new entity)
            var entity = _mapper.Map<User>(dto);

            await _unitOfWork.Users.AddAsync(entity);
            await _unitOfWork.CompleteAsync();

            // Map the new entity back to UserDto for the response
            var responseDto = _mapper.Map<UserDto>(entity);
            return CreatedAtAction(nameof(GetById), new { id = entity.userId }, responseDto);
        }

        // PUT: api/users/5
        [HttpPut("{id}")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateUserDto dto)
        {
            if (id != dto.UserId) return BadRequest();
            var entity = await _unitOfWork.Users.GetByIdAsync( id);
            if (entity == null) return NotFound();

            // Map UpdateUserDto onto the existing User entity
            _mapper.Map(dto, entity);

            _unitOfWork.Users.Update(entity);
            await _unitOfWork.CompleteAsync();
            return NoContent();
        }

        // DELETE: api/users/5
        [HttpDelete("{id}")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> Delete(int id)
        {
            var entity = await _unitOfWork.Users.GetByIdAsync(id);
            if (entity == null) return NotFound();

            _unitOfWork.Users.Remove(entity);
            await _unitOfWork.CompleteAsync();
            return NoContent();
        }
    } 
}
