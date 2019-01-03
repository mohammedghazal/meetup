using System.Threading.Tasks;
using DatingApp.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

using Microsoft.Extensions.Configuration;
using DatingApp.API.Dtos;
using System.Net.Http;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using static DatingApp.API.Models.FacebookApiResponses;
using System.IdentityModel.Tokens.Jwt;
using System;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Collections.Generic;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.API.Controllers
{
    [AllowAnonymous]
    [Route("api/[controller]")]
    [ApiController]
    public class FacebookAuthController : ControllerBase
    {
        
        private readonly IConfiguration _config;
        
        private readonly IMapper _mapper;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private static readonly HttpClient Client = new HttpClient();
        
        private readonly FacebookAuthSettings _fbAuthSettings;

        
        public FacebookAuthController(
            IConfiguration config,
            IMapper mapper,
            IOptions<FacebookAuthSettings> fbAuthSettingsAccessor,
            UserManager<User> userManager,
            SignInManager<User> signInManager)
        {
            _mapper = mapper;
            _config = config;
      _fbAuthSettings = fbAuthSettingsAccessor.Value;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        // POST api/externalauth/facebook
    [HttpPost("facebook")]
    public async Task<IActionResult> FacebookAsync([FromBody]FacebookAuthDto model)
    {
        var appAccessTokenResponse = await Client.GetStringAsync($"https://graph.facebook.com/oauth/access_token?client_id={_fbAuthSettings.AppId}&client_secret={_fbAuthSettings.AppSecret}&grant_type=client_credentials");
      var appAccessToken = JsonConvert.DeserializeObject<FacebookAppAccessToken>(appAccessTokenResponse);
      

      
      var userAccessTokenValidationResponse = await Client.GetStringAsync($"https://graph.facebook.com/debug_token?input_token={model.AccessToken}&access_token={appAccessToken.AccessToken}");
      var userAccessTokenValidation = JsonConvert.DeserializeObject<FacebookUserAccessTokenValidation>(userAccessTokenValidationResponse);



      // 3. we've got a valid token so we can request user data from fb
      var userInfoResponse = await Client.GetStringAsync($"https://graph.facebook.com/v2.8/me?fields=id,email,first_name,last_name,name,gender,locale,birthday,picture&access_token={model.AccessToken}");
      var userInfo = JsonConvert.DeserializeObject<FacebookUserData>(userInfoResponse);


         var user = await _userManager.FindByEmailAsync(userInfo.Email);

      if (user == null)
      {
        var userToCreate = new User
        {
          FacebookId = userInfo.Id,
          Email = userInfo.Email,
          UserName = userInfo.Email
        };
      
      var roll = await _userManager.CreateAsync(userToCreate, Convert.ToBase64String(Guid.NewGuid().ToByteArray()).Substring(0, 8));
      
            var userToReturn = _mapper.Map<UserForDetailedDto>(userToCreate);
      
            if (roll.Succeeded)
            {
                return CreatedAtRoute("GetUser", 
                    new { controller = "Users", id = userToCreate.Id }, userToReturn);
            }

            return BadRequest(roll.Errors);
      }
      var localUser = await _userManager.FindByNameAsync(userInfo.Email);


                return Ok(new
                {
                    token = GenerateJwtToken(localUser).Result
                });
          

    }
    
            private async Task<string> GenerateJwtToken(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email)
            };

            var roles = await _userManager.GetRolesAsync(user);

            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8
                .GetBytes(_config.GetSection("AppSettings:Token").Value));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials = creds
            };

            var tokenHandler = new JwtSecurityTokenHandler();

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }
    }
}