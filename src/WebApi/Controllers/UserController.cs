using BusinessLogic.Services.Core.Base;
using BusinessLogic.Services.External.Base;
using DataAccess.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApi.DTOs.Implementation.Users.Incomings;
using WebApi.DTOs.Implementation.Users.Outgoings;
using WebApi.Helpers;
using WebApi.Models;
using WebApi.Shared.AppConstants;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IUserAuthService _userAuthService;
        private readonly IDistributedFileStorageService _fileService;

        public UserController(
            IUserService userService,
            IUserAuthService userAuthService,
            IDistributedFileStorageService fileService)
        {
            _userService = userService;
            _userAuthService = userAuthService;
            _fileService = fileService;
        }

        [HttpGet("profile")]
        [Authorize(AuthenticationSchemes = CustomAuthenticationSchemes.UserAccountScheme)]
        public async Task<IActionResult> GetUserProfileAsync(CancellationToken cancellationToken)
        {
            var result = HttpContextHelper.GetUserId(context: HttpContext);

            var userId = result.Value;

            // Check if the input userId is existed or not.
            var isExisted = await _userService.IsUserExistedByIdAsync(
                userId: userId,
                cancellationToken: cancellationToken);

            if (!isExisted)
            {
                return NotFound(ApiResponse.Failed($"The user with Id[{userId}] is not found."));
            }

            var profile = await _userService.FindUserByIdForProfileDisplayAsync(
                userId: userId,
                cancellationToken: cancellationToken);

            var profileDto = new UserProfileDto
            {
                Id = userId,
                UserName = profile.UserName,
                FirstName = profile.FirstName(),
                LastName = profile.LastName(),
                Gender = profile.Gender,
                AvatarUrl = profile.AvatarUrl,
                Email = profile.Email,
                PhoneNumber = profile.PhoneNumber,
                CreatedAt = profile.CreatedAt,
                UpdatedAt = profile.UpdatedAt,
            };

            return Ok(ApiResponse.Success(body: profileDto));
        }

        [HttpPatch("profile")]
        [Authorize(AuthenticationSchemes = CustomAuthenticationSchemes.UserAccountScheme)]
        public async Task<IActionResult> UpdateUserProfileAsync(
            UpdateUserProfileDto userProfileDto,
            CancellationToken cancellationToken)
        {
            var result = HttpContextHelper.GetUserId(context: HttpContext);

            var userId = result.Value;

            // Check if the target userId and the current userId are matched or not.
            var isSameUserId = userId == userProfileDto.UserId;

            if (!isSameUserId)
            {
                return BadRequest(ApiResponse.Failed("The update userId and the current userId are not matched."));
            }

            // Check if the input userId is existed or not.
            var isExisted = await _userService.IsUserExistedByIdAsync(
                userId: userId,
                cancellationToken: cancellationToken);

            if (!isExisted)
            {
                return NotFound(ApiResponse.Failed($"The user with Id[{userId}] is not found."));
            }

            // Process to update the user profile.
            userProfileDto.NormalizeAllProperties();

            var userProfileToUpdate = new UserEntity
            {
                Id = userProfileDto.UserId,
                FullName = UserEntity.GetFullName(userProfileDto.FirstName, userProfileDto.LastName),
                Gender = userProfileDto.Gender,
                PhoneNumber = userProfileDto.PhoneNumber,
            };

            var updateResult = await _userService.UpdateUserProfileAsync(
                userProfile: userProfileToUpdate,
                cancellationToken: cancellationToken);

            if (!updateResult)
            {
                return StatusCode(
                    statusCode: StatusCodes.Status500InternalServerError,
                    value: ApiResponse.Failed(ApiResponse.DefaultMessage.DatabaseError));
            }

            return Ok(ApiResponse.Success(default));
        }

        [HttpPatch("avatar")]
        [Authorize(AuthenticationSchemes = CustomAuthenticationSchemes.UserAccountScheme)]
        public async Task<IActionResult> UpdateUserAvatarAsync(
            [FromForm] UpdateUserAvatarDto userAvatarDto,
            CancellationToken cancellationToken)
        {
            var result = HttpContextHelper.GetUserId(context: HttpContext);

            var userId = result.Value;

            // Check if the target userId and the current userId are matched or not.
            var isSameUserId = userId == userAvatarDto.UserId;

            if (!isSameUserId)
            {
                return BadRequest(ApiResponse.Failed("The update userId and the current userId are not matched."));
            }

            // Check if the input userId is existed or not.
            var isExisted = await _userService.IsUserExistedByIdAsync(
                userId: userId,
                cancellationToken: cancellationToken);

            if (!isExisted)
            {
                return NotFound(ApiResponse.Failed($"The user with Id[{userId}] is not found."));
            }

            // Upload image to cloudinary.
            var fileExtension = FormFileHelper.GetFileExtension(userAvatarDto.AvatarImageFile);
            var avatarFileName = $"{userId}.{fileExtension}";
            var fileDataStream = FormFileHelper.GetFileDataStream(userAvatarDto.AvatarImageFile);

            var fileUploadResult = await _fileService.OverwriteImageFileAsync(
                fileId: userId,
                fileName: avatarFileName,
                fileDataStream: fileDataStream);

            if (!fileUploadResult.IsSuccess)
            {
                return StatusCode(
                    statusCode: StatusCodes.Status500InternalServerError,
                    value: ApiResponse.Failed(ApiResponse.DefaultMessage.ServiceError));
            }

            var userToUpdate = new UserEntity
            {
                Id = userId,
                AvatarUrl = fileUploadResult.StorageUrl,
            };

            var updateResult = await _userService.UpdateUserAvatarAsync(
                userToUpdate: userToUpdate,
                cancellationToken: cancellationToken);

            if (!updateResult)
            {
                return StatusCode(
                    statusCode: StatusCodes.Status500InternalServerError,
                    value: ApiResponse.Failed(ApiResponse.DefaultMessage.DatabaseError));
            }

            return Ok(ApiResponse.Success(new UserAvatarDto
            {
                UserId = userToUpdate.Id,
                AvatarUrl = userToUpdate.AvatarUrl,
            }));
        }

        [HttpPatch("password")]
        [Authorize(AuthenticationSchemes = CustomAuthenticationSchemes.UserAccountScheme)]
        public async Task<IActionResult> UpdateUserPasswordAsync(
            UpdateUserPasswordDto userPasswordDto,
            CancellationToken cancellationToken)
        {
            var result = HttpContextHelper.GetUserId(context: HttpContext);

            var userId = result.Value;

            // Check if the target userId and the current userId are matched or not.
            var isSameUserId = userId == userPasswordDto.UserId;

            if (!isSameUserId)
            {
                return BadRequest(ApiResponse.Failed("The update userId and the current userId are not matched."));
            }

            // Check if the input userId is existed or not.
            var isExisted = await _userService.IsUserExistedByIdAsync(
                userId: userId,
                cancellationToken: cancellationToken);

            if (!isExisted)
            {
                return NotFound(ApiResponse.Failed($"The user with Id[{userId}] is not found."));
            }

            var checkPasswordResult = await _userService.CheckPasswordByUserIdAsync(
                userId: userId,
                password: userPasswordDto.OldPassword);

            if (!checkPasswordResult.IsSuccess)
            {
                return BadRequest(ApiResponse.Failed(checkPasswordResult.ErrorMessages));
            }

            // Process to update the user password.
            var foundUser = checkPasswordResult.Value;

            // Validate if the password is conform the format or not.
            var passwordValidationResult = await _userAuthService.ValidatePasswordAsync(
                user: foundUser,
                password: userPasswordDto.NewPassword);

            if (!passwordValidationResult.Succeeded)
            {
                var messages = passwordValidationResult.Errors.Select(error => error.Description);

                return BadRequest(ApiResponse.Failed(messages));
            }

            var updateResult = await _userService.UpdateUserPasswordAsync(
                user: foundUser,
                newPassword: userPasswordDto.NewPassword,
                cancellationToken: cancellationToken);

            if (!updateResult)
            {
                return StatusCode(
                    statusCode: StatusCodes.Status500InternalServerError,
                    value: ApiResponse.Failed(ApiResponse.DefaultMessage.DatabaseError));
            }

            return Ok(ApiResponse.Success(default));
        }
    }
}
