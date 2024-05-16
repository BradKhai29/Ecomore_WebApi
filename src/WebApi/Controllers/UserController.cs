using BusinessLogic.Services.Core.Base;
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

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet("profile")]
        [Authorize(AuthenticationSchemes = CustomAuthenticationSchemes.UserAccountScheme)]
        public async Task<IActionResult> GetUserProfileAsync(CancellationToken cancellationToken)
        {
            var result = HttpContextHelper.GetUserId(context: HttpContext);

            if (!result.IsSuccess)
            {
                return BadRequest(ApiResponse.Failed("User is not authenticated"));
            }

            var userId = result.Value;

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
            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponse.Failed());
            }

            var result = HttpContextHelper.GetUserId(context: HttpContext);

            if (!result.IsSuccess)
            {
                return BadRequest(ApiResponse.Failed("User is not authenticated"));
            }

            // Missing the api for uploading avatar to cloudinary.

            var userId = result.Value;

            if (userId != userProfileDto.Id)
            {
                return BadRequest(ApiResponse.Failed("The target userId and the update userId is not matched."));
            }

            var userProfileToUpdate = new UserEntity
            {
                Id = userProfileDto.Id,
                FullName = UserEntity.GetFullName(userProfileDto.FirstName, userProfileDto.LastName),
                PhoneNumber = userProfileDto.PhoneNumber,
            };

            var isSuccess = await _userService.UpdateUserProfileAsync(
                userProfile: userProfileToUpdate,
                cancellationToken: cancellationToken);

            if (!isSuccess)
            {
                return StatusCode(
                    statusCode: StatusCodes.Status500InternalServerError,
                    value: ApiResponse.Failed(ApiResponse.DefaultMessage.DatabaseError));
            }

            return Ok(ApiResponse.Success(default));
        }
    }
}
