using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Store.Api.Controllers; // Assuming your DTOs are here
using Domain.Entities; // For Product entity
using Domain.Interfaces; // For IUnitOfWork
using Microsoft.AspNetCore.Mvc;
using Store.Api.DTOs.Products;
using System.Net;
using Domain.Interfaces;
using Domain.Entities;
using AutoMapper;
using Store.Api.DTOs.Products;
using System.Net;

// NOTE: We are assuming your IProductRepository property on IUnitOfWork is named 'Products'.

[Route("api/products")]
[ApiController]

public class ProductsController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public ProductsController(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var entities = await _unitOfWork.Products.GetAllAsync();
        return Ok(_mapper.Map<IEnumerable<ProductDto>>(entities));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var entity = await _unitOfWork.Products.GetByIdAsync(id);
        if (entity == null) return NotFound();
        return Ok(_mapper.Map<ProductDto>(entity));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateProductDto dto)
    {
        var entity = _mapper.Map<Product>(dto);

        await _unitOfWork.Products.AddAsync(entity);
        await _unitOfWork.CompleteAsync();

        var responseDto = _mapper.Map<ProductDto>(entity);
        return CreatedAtAction(nameof(GetById), new { id = entity.productId }, responseDto);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateProductDto dto)
    {
        if (id != dto.productId) return BadRequest();
        var entity = await _unitOfWork.Products.GetByIdAsync(id);
        if (entity == null) return NotFound();

        _mapper.Map(dto, entity);

        _unitOfWork.Products.Update(entity);
        await _unitOfWork.CompleteAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var entity = await _unitOfWork.Products.GetByIdAsync(id);
        if (entity == null) return NotFound();

        _unitOfWork.Products.Remove(entity);
        await _unitOfWork.CompleteAsync();
        return NoContent();
    }
}