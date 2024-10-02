using API.Dtos;
using API.Errors;
using AutoMapper;
using Core.Entities;
using Core.Interfaces;
using Core.Specifications;
using Microsoft.AspNetCore.Mvc;
using API.Helpers;

namespace API.Controllers
{

    public class ProductsController : BaseApiController
    {
        private readonly IGenericRepository<Product> _productsRepo;
        private readonly IGenericRepository<ProductBrand> _productsBrandRepo;
        private readonly IGenericRepository<ProductType> _productsTypeRepo;
        private readonly IMapper _mapper;

        public ProductsController(IGenericRepository<Product> productsRepo,
        IGenericRepository<ProductBrand> productsBrandRepo,
        IGenericRepository<ProductType> productsTypeRepo,
        IMapper mapper)
        {
            _productsRepo = productsRepo;
            _productsBrandRepo = productsBrandRepo;
            _productsTypeRepo = productsTypeRepo;
            _mapper = mapper;
        }
        
        [Cached(600)]
        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<ProductToReturnDto>>> GetProducts
        ([FromQuery] ProductSpecParams productParams)
        {
            var spec = new ProductsWithTypesAndBrandsSpecification(productParams);
            var countSpec = new ProductsWithFiltersForCountSpecifications(productParams);
            var totalItems = await _productsRepo.CountAsync(countSpec);
            var products = await _productsRepo.ListAsync(spec);
            var data = _mapper.Map<IReadOnlyList<Product>, IReadOnlyList<ProductToReturnDto>>(products);

            return Ok(new Pagination<ProductToReturnDto>(productParams.PageIndex, productParams.PageSize, totalItems, data));
        }
        [Cached(600)]
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ProductToReturnDto>> GetProduct(int id)
        {
            var spec = new ProductsWithTypesAndBrandsSpecification(id);
            var product = await _productsRepo.GetEntityWithSpec(spec);

            if (product == null)
            {
                return NotFound(new ApiResponse(404));
            }

            return _mapper.Map<Product, ProductToReturnDto>(product);
        }
        
        [Cached(600)]
        [HttpGet("brands")]
        public async Task<ActionResult<IReadOnlyList<ProductBrand>>> GetProductBrands()
        {
            return Ok(await _productsBrandRepo.ListAllAsync());
        }
        
        [Cached(600)]
        [HttpGet("types")]
        public async Task<ActionResult<IReadOnlyList<ProductType>>> GetProductTypes()
        {
            return Ok(await _productsTypeRepo.ListAllAsync());
        }
        
        [HttpPost]
        public async Task<ActionResult<ProductToReturnDto>> CreateProduct(ProductCreateDto productDto)
        {
            // Check if the ProductBrand exists
            var brandExists = await _productsBrandRepo.GetByIdAsync(productDto.ProductBrandId);
            if (brandExists == null)
            {
                return BadRequest(new ApiResponse(400, "Invalid Product Brand ID"));
            }

            // Check if the ProductType exists
            var typeExists = await _productsTypeRepo.GetByIdAsync(productDto.ProductTypeId);
            if (typeExists == null)
            {
                return BadRequest(new ApiResponse(400, "Invalid Product Type ID"));
            }

            var product = _mapper.Map<ProductCreateDto, Product>(productDto);

            _productsRepo.Add(product);
            var result = await _productsRepo.SaveAllAsync();

            if (result <= 0)
            {
                return BadRequest(new ApiResponse(400, "Problem creating product"));
            }

            var productToReturn = _mapper.Map<Product, ProductToReturnDto>(product);

            return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, productToReturn);
        }

        
        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateProduct(int id, ProductUpdateDto productDto)
        {
            var product = await _productsRepo.GetByIdAsync(id);

            if (product == null) 
                return NotFound(new ApiResponse(404, "Product not found"));

            _mapper.Map(productDto, product);

            _productsRepo.Update(product);
            var result = await _productsRepo.SaveAllAsync();

            if (result <= 0)
                return BadRequest(new ApiResponse(400, "Problem updating product"));

            return Ok(new { message = "Product updated successfully" });
        }
        
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteProduct(int id)
        {
            var product = await _productsRepo.GetByIdAsync(id);

            if (product == null)
                return NotFound(new ApiResponse(404, "Product not found"));

            _productsRepo.Delete(product);
            var result = await _productsRepo.SaveAllAsync();

            if (result <= 0)
                return BadRequest(new ApiResponse(400, "Problem deleting product"));

            return Ok(new { message = "Product deleted successfully" });
        }


    }
}