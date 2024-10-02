using API.Dtos;
using API.Errors;
using API.Extensions;
using AutoMapper;
using Core.Entities.Identity;
using Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class AccountController : BaseApiController
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly ITokenService _tokenService;
        private readonly IMapper mapper;

        public AccountController(UserManager<AppUser> userManager,
        SignInManager<AppUser> signInManager,ITokenService tokenService,
        IMapper mapper)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _tokenService = tokenService;
            this.mapper = mapper;
        }

        [Authorize]
        [HttpGet]
        public async Task<ActionResult<UserDto>> GetCurrentUser()
        {
            var user = await _userManager.FindByEmailFromClaimsPrincipal(User);

            if (user == null)
            {
                return NotFound(new ApiResponse(404, "User not found"));
            }

            return new UserDto
            {
                Email = user.Email,
                Token = _tokenService.CreateToken(user),
                DisplayName = user.DisplayName
            };
        }

        [HttpGet("emailexists")]
        public async Task<ActionResult<bool>> CheckEmailExistsAsync([FromQuery] string email)
        {
            return await _userManager.FindByEmailAsync(email) != null;
        }
        [Authorize]
        [HttpGet("address")]
        public async Task<ActionResult<AddressDto>> GetUserAddress(){

            var user = await _userManager.FindUserByClaimsPrincipleWithAddress(User);

            return mapper.Map<Address,AddressDto>(user.Address);
        }

        [Authorize]
        [HttpPut("address")]
        public async Task<ActionResult<AddressDto>> UpdateUserAddress(AddressDto address)
        {
            var user = await _userManager.FindUserByClaimsPrincipleWithAddress(HttpContext.User);

            user.Address = mapper.Map<AddressDto,Address>(address);

            var result = await _userManager.UpdateAsync(user);
            if(result.Succeeded) return Ok(mapper.Map<Address,AddressDto>(user.Address));

            return BadRequest("Problem updating the user");
        }

        [HttpPost("login")]
        public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
        {
            var user = await _userManager.FindByEmailAsync(loginDto.Email);

            if(user == null) return Unauthorized(new ApiResponse(401));

            var result = await _signInManager.CheckPasswordSignInAsync(user,loginDto.Password,false);
        
            if(!result.Succeeded) return Unauthorized(new ApiResponse(401));

            return new UserDto{
                Email = user.Email,
                Token = _tokenService.CreateToken(user),
                DisplayName = user.DisplayName
            };
        }
        [HttpPost("register")]
        public async Task<ActionResult<UserDto>>Regjister(RegjisterDto regjisterDto){

            if(CheckEmailExistsAsync(regjisterDto.Email).Result.Value){
                return new BadRequestObjectResult(new ApiValidationErrorResponse{Errors = new []{
                    "Email address is already in use"
                }});
            }
            
            var user = new AppUser
            {
                DisplayName = regjisterDto.DisplayName,
                Email = regjisterDto.Email,
                UserName = regjisterDto.Email
            };

            var result = await _userManager.CreateAsync(user,regjisterDto.Password);

            if(!result.Succeeded) return BadRequest(new ApiResponse(400));

            return new UserDto
            {
                DisplayName = user.DisplayName,
                Token = _tokenService.CreateToken(user),
                Email = user.Email
            };
        }
        
        [Authorize]
        [HttpPut("update-account")]
        public async Task<ActionResult<UserDto>> UpdateAccount(UpdateAccountDto updateAccountDto)
        {
            var user = await _userManager.FindByEmailFromClaimsPrincipal(User);

            if (user == null)
            {
                return NotFound(new ApiResponse(404, "User not found"));
            }

            user.DisplayName = updateAccountDto.DisplayName ?? user.DisplayName;
            user.Email = updateAccountDto.Email ?? user.Email;
            user.UserName = updateAccountDto.Email ?? user.UserName;

            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded) return BadRequest(new ApiResponse(400, "Problem updating the user"));

            return new UserDto
            {
                DisplayName = user.DisplayName,
                Email = user.Email,
                Token = _tokenService.CreateToken(user)
            };
        }
        
        [Authorize]
        [HttpDelete("delete-account")]
        public async Task<ActionResult> DeleteAccount()
        {
            var user = await _userManager.FindByEmailFromClaimsPrincipal(User);

            if (user == null)
            {
                return NotFound(new ApiResponse(404, "User not found"));
            }

            var result = await _userManager.DeleteAsync(user);

            if (!result.Succeeded) return BadRequest(new ApiResponse(400, "Problem deleting the user"));

            return Ok(new { message = "Account deleted successfully" });
        }

    }
}