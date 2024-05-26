using BusinessLogic.Services.Core.Base;
using BusinessLogic.Services.External.Base;
using DataAccess.DataSeedings;
using DataAccess.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApi.DTOs.Implementation.ProductImages.Incomings;
using WebApi.DTOs.Implementation.Products.Incomings;
using WebApi.DTOs.Implementation.Products.Outgoings;
using WebApi.Helpers;
using WebApi.Models;
using WebApi.Shared.AppConstants;
using WebApi.Shared.Enums;
using WebApi.Shared.ValidationAttributes;

namespace WebApi.Areas.SystemAccount
{
    [Route("api/admin/product")]
    [ApiController]
    [Authorize(AuthenticationSchemes = CustomAuthenticationSchemes.SystemAccountScheme)]
    public class AdminProductController : ControllerBase
    {
        private readonly ICategoryService _categoryService;
        private readonly ISystemAccountService _systemAccountService;
        private readonly ISystemAccountProductService _productService;
        private readonly IDistributedFileStorageService _fileService;

        public AdminProductController(
            ISystemAccountProductService productService,
            ICategoryService categoryService,
            IDistributedFileStorageService fileService,
            ISystemAccountService systemAccountService)
        {
            _productService = productService;
            _categoryService = categoryService;
            _fileService = fileService;
            _systemAccountService = systemAccountService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllProductsAsync(CancellationToken cancellationToken)
        {
            var products = await _productService.GetAllProductsAsync(cancellationToken);

            var productDtos = products.Select(selector: item => new GetProductForGeneralDisplayDto
            {
                Id = item.Id,
                Category = item.Category.Name,
                Name = item.Name,
                Description = item.Description,
                QuantityInStock = item.QuantityInStock,
                Status = item.ProductStatus.Name,
                UnitPrice = item.UnitPrice,
                ImageUrls = item.ProductImages.Select(image => image.StorageUrl)
            });

            return Ok(value: ApiResponse.Success(body: productDtos));
        }

        [HttpGet("detail/{productId:guid}")]
        public async Task<IActionResult> GetProductByIdForDetailDisplayAsync(
            [FromRoute] Guid productId,
            CancellationToken cancellationToken)
        {
            var isProductExisted = await _productService.IsProductExistedByIdAsync(
                productId: productId,
                cancellationToken: cancellationToken);

            if (!isProductExisted)
            {
                return NotFound(
                    ApiResponse.Failed($"The product with Id [{productId}] is not found"));
            }

            var foundProduct = await _productService.FindProductByIdForDetailDisplayAsync(
                productId: productId,
                cancellationToken: cancellationToken);

            var productDto = new GetProductByIdForDetailDisplayDto
            {
                Id = productId,
                Name = foundProduct.Name,
                Category = new DTOs.Implementation.Categories.Outgoings.GetCategoryByIdForDetailDisplayDto
                {
                    Id = foundProduct.Category.Id,
                    Name = foundProduct.Category.Name
                },
                SellingCount = foundProduct.SellingCount,
                Description = foundProduct.Description,
                QuantityInStock = foundProduct.QuantityInStock,
                Status = foundProduct.ProductStatus.Name,
                UnitPrice = foundProduct.UnitPrice,
                ImageUrls = foundProduct.ProductImages.Select(image => image.StorageUrl)
            };

            return Ok(ApiResponse.Success(productDto));
        }

        [HttpGet("update/{productId:guid}")]
        public async Task<IActionResult> GetProductByIdForUpdateAsync(
            [FromRoute] Guid productId,
            CancellationToken cancellationToken)
        {
            var isProductExisted = await _productService.IsProductExistedByIdAsync(
                productId: productId,
                cancellationToken: cancellationToken);

            if (!isProductExisted)
            {
                return NotFound(
                    ApiResponse.Failed($"The product with Id [{productId}] is not found"));
            }

            var foundProduct = await _productService.FindProductByIdForUpdateAsync(
                productId: productId,
                cancellationToken: cancellationToken);

            var productDto = new GetProductForUpdateDto
            {
                ProductId = productId,
                Name = foundProduct.Name,
                Category = new DTOs.Implementation.Categories.Outgoings.GetCategoryByIdForDetailDisplayDto
                {
                    Id = foundProduct.Category.Id,
                    Name = foundProduct.Category.Name
                },
                Description = foundProduct.Description,
                QuantityInStock = foundProduct.QuantityInStock,
                Status = new DTOs.Implementation.ProductStatuses.Outgoings.ProductStatusDetailDto
                {
                    Id = foundProduct.ProductStatus.Id,
                    Name = foundProduct.ProductStatus.Name,
                },
                UnitPrice = foundProduct.UnitPrice,
                ProductImages = foundProduct.ProductImages.Select(image => new DTOs.Implementation.ProductImages.Outgoings.ProductImageDetailDto
                {
                    ImageId = image.Id,
                    StorageUrl = image.StorageUrl,
                    UploadOrder = image.UploadOrder,
                })
            };

            return Ok(ApiResponse.Success(productDto));
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateNewProductAsync(
            [FromForm] CreateProductDto productDto,
            CancellationToken cancellationToken)
        {
            var systemAccountIdResult = HttpContextHelper.GetSystemAccountId(HttpContext);

            if (!systemAccountIdResult.IsSuccess)
            {
                return Unauthorized(
                    ApiResponse.Failed(ApiResponse.DefaultMessage.InvalidAccessToken));
            }
            
            // Check if the systemAccountId that encapsulated
            // in access-token is available or not.
            var accountId = systemAccountIdResult.Value;

            var isAccountAvailable = await _systemAccountService.IsAccountAvailableByIdAsync(
                accountId: accountId,
                cancellationToken: cancellationToken);

            if (!isAccountAvailable)
            {
                return Unauthorized(
                    ApiResponse.Failed("This system account is not permitted to access to this resource."));
            }

            // Check if the input categoryId is existed or not.
            var isCategoryExisted = await _categoryService.IsCategoryExistedByIdAsync(
                categoryId: productDto.CategoryId,
                cancellationToken: cancellationToken);

            if (!isCategoryExisted)
            {
                return BadRequest(ApiResponse.Failed("Invalid input category."));
            }

            // Create the produdt entity instance.
            var dateTimeUtcNow = DateTime.UtcNow;
            var productToCreate = new ProductEntity
            {
                Id = Guid.NewGuid(),
                CategoryId = productDto.CategoryId,
                Name = productDto.ProductName,
                Description = productDto.Description,
                UnitPrice = productDto.UnitPrice,
                ProductStatusId = ProductStatuses.InStock.Id,
                QuantityInStock = productDto.QuantityInStock,
                CreatedBy = accountId,
                CreatedAt = dateTimeUtcNow,
                UpdatedBy = accountId,
                UpdatedAt = dateTimeUtcNow,
                SellingCount = 0,
            };

            var productImages = new List<ProductImageEntity>(
                capacity: productDto.ProductImageFiles.Length);

            // Get the product image files and upload to the cloud file storage service.
            int uploadOrder = 0;
            foreach (var imageFile in productDto.ProductImageFiles)
            {
                var fileId = Guid.NewGuid();
                var fileExtension = FormFileHelper.GetFileExtension(imageFile);
                var fileName = $"{fileId}.{fileExtension}";
                var fileDataStream = FormFileHelper.GetFileDataStream(imageFile);

                var fileUploadResult = await _fileService.UploadImageFileAsync(
                    fileId: fileId,
                    fileName: fileName,
                    fileDataStream: fileDataStream);

                if (!fileUploadResult.IsSuccess)
                {
                    // Clear the list of image due to error in uploading file.
                    productImages.Clear();

                    return BadRequest(
                        ApiResponse.Failed(ApiResponse.DefaultMessage.ServiceError));
                }

                var productImage = new ProductImageEntity
                {
                    Id = fileId,
                    FileName = fileName,
                    UploadOrder = uploadOrder,
                    ProductId = productToCreate.Id,
                    StorageUrl = fileUploadResult.StorageUrl,
                };

                productImages.Add(productImage);

                uploadOrder++;
            }

            var createResult = await _productService.CreateNewProductAsync(
                product: productToCreate,
                productImages: productImages,
                cancellationToken: cancellationToken);

            if (!createResult)
            {
                return StatusCode(
                    statusCode: StatusCodes.Status500InternalServerError,
                    value: ApiResponse.Failed(ApiResponse.DefaultMessage.ServerError));
            }

            var responseDto = new CreateProductResponseDto
            {
                ProductId = productToCreate.Id,
                ResourceUrl = $"api/admin/product/{productToCreate.Id}"
            };

            return Created(
                uri: nameof(GetProductByIdForDetailDisplayAsync),
                value: ApiResponse.Success(responseDto));
        }

        [HttpPut("update")]
        public async Task<IActionResult> UpdateProductAsync(
            [FromForm] UpdateProductDto productDto,
            CancellationToken cancellationToken)
        {
            var systemAccountIdResult = HttpContextHelper.GetSystemAccountId(HttpContext);

            if (!systemAccountIdResult.IsSuccess)
            {
                return Unauthorized(
                    ApiResponse.Failed(ApiResponse.DefaultMessage.InvalidAccessToken));
            }
            
            // Check if the systemAccountId that encapsulated
            // in access-token is available or not.
            var accountId = systemAccountIdResult.Value;

            var isAccountAvailable = await _systemAccountService.IsAccountAvailableByIdAsync(
                accountId: accountId,
                cancellationToken: cancellationToken);

            if (!isAccountAvailable)
            {
                return Unauthorized(
                    ApiResponse.Failed("This system account is not permitted to access to this resource."));
            }

            // Check if the productId is existed or not.
            var isProductExisted = await _productService.IsProductExistedByIdAsync(
                productId: productDto.Id,
                cancellationToken: cancellationToken);

            if (!isProductExisted)
            {
                return NotFound(
                    ApiResponse.Failed($"The product with Id [{productDto.Id}] is not found"));
            }

            // Check if the input categoryId is existed or not.
            var isCategoryExisted = await _categoryService.IsCategoryExistedByIdAsync(
                categoryId: productDto.CategoryId,
                cancellationToken: cancellationToken);

            if (!isCategoryExisted)
            {
                return BadRequest(ApiResponse.Failed("Invalid input category."));
            }

            // Check if the product-status-id is existed or not.
            var isStatusExisted = ProductStatuses.IsStatusExistedById(productDto.StatusId);

            if (!isStatusExisted)
            {
                return NotFound(
                    ApiResponse.Failed($"The product with Id [{productDto.Id}] is not found"));
            }

            // Seperate the upload UpdateProductImageDto items
            // into 2 groups to have independent operation on each.
            var updateImageItems = new List<UpdateProductImageDto>();
            var deleteImageIds = new List<Guid>();

            // Decode the metadata to get the product image items information
            // to conduct updating operation properly.
            var decodedResult = productDto.DecodeMetaData();
            
            if (!decodedResult.IsSuccess)
            {
                return BadRequest(ApiResponse.Failed("Invalid metadata format."));
            }

            var lastOrder = 0;
            var productImageItems = decodedResult.Value;
            foreach(var productImageDtoItem in productImageItems)
            {
                // If status is update, then add to updateItem list for later operation.
                if (productImageDtoItem.Status == ImageUploadActionCode.Update)
                {
                    // Get the form file in the collection that has similar
                    // name to this productImageDtoItem's file name for later processing.
                    var formFileForLaterProcess = FormFileHelper.GetFormFileByName(
                        formFiles: productDto.ProductImageFiles,
                        fileName: productImageDtoItem.FileName);

                    // Get the last order of the last image this product currently has.
                    if (lastOrder < productImageDtoItem.UploadOrder)
                    {
                        lastOrder = productImageDtoItem.UploadOrder;
                    }

                    productImageDtoItem.SetFileToProcess(formFileForLaterProcess);

                    updateImageItems.Add(productImageDtoItem);
                }
                // If status is delete, then add to deleteItem list for later operation.
                else if (productImageDtoItem.Status == ImageUploadActionCode.Delete)
                {
                    deleteImageIds.Add(productImageDtoItem.ImageId);
                }
            }

            // Create the list of form files that need to be added new.
            var imageFilesToAddNew = new List<IFormFile>();
            
            foreach(var imageFile in productDto.ProductImageFiles)
            {
                var isInUpdateList = updateImageItems.Any(
                    predicate: item => item.FileName.Equals(imageFile.FileName));

                if (!isInUpdateList)
                {
                    imageFilesToAddNew.Add(imageFile);
                }
            }

            // Check if the numbers of upload image is exceed the limit or not.
            var currentImageCount = await _productService.CountProductImagesByIdAsync(
                productId: productDto.Id,
                cancellationToken: cancellationToken);

            var isExceed = ProductEntity.MetaData.MaxImages < 
                currentImageCount + imageFilesToAddNew.Count - deleteImageIds.Count;

            if (isExceed)
            {
                return BadRequest(
                    ApiResponse.Failed($"The number of upload images is exceed the max limit [{ProductEntity.MetaData.MaxImages}]."));
            }

            // Upload both new files and updated files to the cloud file service.
            var updateProductImages = new List<ProductImageEntity>(updateImageItems.Count);

            foreach(var imageItem in updateImageItems)
            {
                var fileId = imageItem.ImageId;
                var fileExtension = FormFileHelper.GetFileExtension(imageItem.GetFileToProcess());
                var fileName = $"{fileId}.{fileExtension}";
                var fileDataStream = FormFileHelper.GetFileDataStream(imageItem.GetFileToProcess());

                var fileUploadResult = await _fileService.OverwriteImageFileAsync(
                    fileId: fileId,
                    fileName: fileName,
                    fileDataStream: fileDataStream);

                if (!fileUploadResult.IsSuccess)
                {
                    ClearAllList(updateImageItems, deleteImageIds, imageFilesToAddNew);

                    return BadRequest(
                        ApiResponse.Failed(ApiResponse.DefaultMessage.ServiceError));
                }

                var updateProductImage = new ProductImageEntity
                {
                    Id = fileId,
                    FileName = fileName,
                    StorageUrl = fileUploadResult.StorageUrl,
                    UploadOrder = imageItem.UploadOrder,
                };

                updateProductImages.Add(updateProductImage);
            }

            // Process to update product information.
            var productToUpdate = new ProductEntity
            {
                Id = productDto.Id,
                Name = productDto.ProductName,
                ProductStatusId = productDto.StatusId,
                CategoryId = productDto.CategoryId,
                Description = productDto.Description,
                QuantityInStock = productDto.QuantityInStock,
                UnitPrice = productDto.UnitPrice,
                UpdatedAt = DateTime.UtcNow,
                UpdatedBy = accountId,
            };

            // Update the product images and the related information.
            var updateResult = await _productService.UpdateProductDetailAsync(
                productToUpdate: productToUpdate,
                listOfUpdatedImages: updateProductImages,
                listOfImageIdToDelete: deleteImageIds,
                cancellationToken: cancellationToken);

            if (!updateResult)
            {
                ClearAllList(updateImageItems, deleteImageIds, imageFilesToAddNew);

                return StatusCode(
                    statusCode: StatusCodes.Status500InternalServerError,
                    value: ApiResponse.Failed(ApiResponse.DefaultMessage.ServerError));
            }

            // Upload the new images and add to the database.
            var addNewProductImages = new List<ProductImageEntity>();
            foreach (var imageFile in imageFilesToAddNew)
            {
                var fileId = Guid.NewGuid();
                var fileExtension = FormFileHelper.GetFileExtension(imageFile);
                var fileName = $"{fileId}.{fileExtension}";
                var fileDataStream = FormFileHelper.GetFileDataStream(imageFile);

                var fileUploadResult = await _fileService.UploadImageFileAsync(
                    fileId: fileId,
                    fileName: fileName,
                    fileDataStream: fileDataStream);

                if (!fileUploadResult.IsSuccess)
                {
                    // Clear the list of image due to error in uploading file.
                    ClearAllList(updateImageItems, deleteImageIds, imageFilesToAddNew);

                    return BadRequest(
                        ApiResponse.Failed(ApiResponse.DefaultMessage.ServiceError));
                }

                lastOrder++;
                var productImage = new ProductImageEntity
                {
                    Id = fileId,
                    FileName = fileName,
                    UploadOrder = lastOrder,
                    ProductId = productDto.Id,
                    StorageUrl = fileUploadResult.StorageUrl,
                };

                addNewProductImages.Add(productImage);
            }

            var addResult = await _productService.AddProductImagesAsync(
                productImages: addNewProductImages,
                cancellationToken: cancellationToken);

            if (!addResult)
            {
                return BadRequest(
                    ApiResponse.Failed(ApiResponse.DefaultMessage.ServiceError));
            }

            foreach(var deleteImageId in deleteImageIds)
            {
                // Asynchronously remove the image files without blocking the thread.
                _ = _fileService.RemoveImageFileByIdAsync(
                    fileId: deleteImageId.ToString());
            }

            // Clear all list after processing done.
            ClearAllList(updateImageItems, deleteImageIds, imageFilesToAddNew);

            return Ok(ApiResponse.Success(default));
            
            void ClearAllList(
                List<UpdateProductImageDto> updateImageItems,
                List<Guid> deleteImageIds,
                List<IFormFile> imageFilesToAddNew)
            {
                // Clear the list of image due to error in uploading file.
                imageFilesToAddNew.Clear();
                updateImageItems.Clear();
                deleteImageIds.Clear();
            }
        }

        [HttpDelete("{productId:guid}")]
        public async Task<IActionResult> DeleteProductByIdAsync(
            [IsValidGuid] Guid productId,
            CancellationToken cancellationToken)
        {
            var isProductExisted = await _productService.IsProductExistedByIdAsync(
                productId: productId,
                cancellationToken: cancellationToken);

            if (!isProductExisted)
            {
                return NotFound(
                    ApiResponse.Failed($"The product with Id [{productId}] is not found"));
            }

            var productHasAnyOrder = await _productService.CheckProductHasAnyOrderByIdAsync(
                productId: productId,
                cancellationToken: cancellationToken);

            if (productHasAnyOrder)
            {
                return BadRequest(
                    ApiResponse.Failed("Cannot remove this product because other customers had ordered it."));
            }

            var deleteResult = await _productService.DeleteProductByIdAsync(
                productId: productId,
                cancellationToken: cancellationToken);

            if (!deleteResult)
            {
                return StatusCode(
                    statusCode: StatusCodes.Status500InternalServerError,
                    value: ApiResponse.Failed(ApiResponse.DefaultMessage.ServerError));
            }

            return Ok(ApiResponse.Success(default));
        }
    }
}
