using BusinessLogic.Services.Core.Base;
using DataAccess.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApi.DTOs.Implementation.Categories.Incomings;
using WebApi.DTOs.Implementation.Categories.Outgoings;
using WebApi.Helpers;
using WebApi.Models;
using WebApi.Shared.AppConstants;

namespace WebApi.Areas.SystemAccount
{
    [Route("api/system/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAsync(CancellationToken cancellationToken)
        {
            var categoryList = await _categoryService.GetAllCategoriesAsync(cancellationToken);

            var dtos = categoryList.Select(category => new GetCategoryByIdForDetailDisplayDto
            {
                Id = category.Id,
                Name = category.Name,
            });

            return Ok(ApiResponse.Success(dtos));
        }

        [HttpPost]
        [Authorize(AuthenticationSchemes = CustomAuthenticationSchemes.SystemAccountScheme)]
        public async Task<IActionResult> CreateAsync(
            [FromBody]
            CreateCategoryDto createCategoryDto,
            CancellationToken cancellationToken)
        {
            createCategoryDto.NormalizeAllProperties();

            var isCategoryNameExisted = await _categoryService.IsCategoryExistedByNameAsync(
                categoryName: createCategoryDto.Name,
                cancellationToken: cancellationToken);

            if (isCategoryNameExisted)
            {
                return Conflict(ApiResponse.Failed($"The category name [{createCategoryDto.Name}] is already existed."));
            }

            var extractResult = HttpContextHelper.GetSystemAccountId(HttpContext);

            if (!extractResult.IsSuccess)
            {
                return Unauthorized(ApiResponse.Failed(ApiResponse.DefaultMessage.InvalidAccessToken));
            }

            var newCategory = new CategoryEntity
            {
                Id = Guid.NewGuid(),
                Name = createCategoryDto.Name,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = extractResult.Value
            };

            var addResult = await _categoryService.AddCategoryAsync(
                category: newCategory,
                cancellationToken: cancellationToken);

            if (!addResult)
            {
                return StatusCode(
                    statusCode: StatusCodes.Status500InternalServerError,
                    value: ApiResponse.Failed(ApiResponse.DefaultMessage.DatabaseError));
            }

            return Ok(ApiResponse.Success(default));
        }

        [HttpPut]
        [Authorize(AuthenticationSchemes = CustomAuthenticationSchemes.SystemAccountScheme)]
        public async Task<IActionResult> UpdateAsync(
            UpdateCategoryDto updateCategoryDto,
            CancellationToken cancellationToken)
        {
            updateCategoryDto.NormalizeAllProperties();

            var isCategoryExisted = await _categoryService.IsCategoryExistedByIdAsync(
                categoryId: updateCategoryDto.Id,
                cancellationToken: cancellationToken);

            if (!isCategoryExisted)
            {
                return NotFound(ApiResponse.Failed($"Category with id [{updateCategoryDto.Id}] is not found."));
            }

            var updateCategory = new CategoryEntity
            {
                Id = updateCategoryDto.Id,
                Name = updateCategoryDto.Name,
            };

            var updateResult = await _categoryService.UpdateCategoryAsync(
                category: updateCategory,
                cancellationToken: cancellationToken);

            if (!updateResult)
            {
                return StatusCode(
                    statusCode: StatusCodes.Status500InternalServerError,
                    value: ApiResponse.Failed(ApiResponse.DefaultMessage.DatabaseError));
            }

            return Ok(ApiResponse.Success(default));
        }

        [HttpDelete("{categoryId:guid}")]
        [Authorize(AuthenticationSchemes = CustomAuthenticationSchemes.SystemAccountScheme)]
        public async Task<IActionResult> DeleteAsync(
            [FromRoute] Guid categoryId,
            CancellationToken cancellationToken)
        {
            var isCategoryExisted = await _categoryService.IsCategoryExistedByIdAsync(
                categoryId: categoryId,
                cancellationToken: cancellationToken);

            if (!isCategoryExisted)
            {
                return NotFound(ApiResponse.Failed($"Category with id [{categoryId}] is not found."));
            }

            var categoryHasProducts = await _categoryService.CheckCategoryHasProductByIdAsync(
                categoryId: categoryId,
                cancellationToken: cancellationToken);

            if (categoryHasProducts)
            {
                return BadRequest(ApiResponse.Failed("This category has products, cannot delete."));
            }

            var deleteResult = await _categoryService.DeleteCategoryByIdAsync(
                categoryId: categoryId,
                cancellationToken: cancellationToken);

            if (!deleteResult)
            {
                return StatusCode(
                    statusCode: StatusCodes.Status500InternalServerError,
                    value: ApiResponse.Failed(ApiResponse.DefaultMessage.DatabaseError));
            }

            return Ok(ApiResponse.Success(default));
        }
    }
}
