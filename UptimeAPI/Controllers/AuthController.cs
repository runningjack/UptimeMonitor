﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Data.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using UptimeAPI.Services;
using UptimeAPI.Settings;

namespace UptimeAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IMapper _mapper;
        private readonly JwtSettings _jwtSettings;

        public AuthController(
            UserManager<IdentityUser> userManager
            , IMapper mapper
            , IOptionsSnapshot<JwtSettings> jwtSettings)
        {
            _mapper = mapper;
            _userManager = userManager;
            _jwtSettings = jwtSettings.Value;
        }
        [HttpPost("SignUp")]
        public async Task<IActionResult> SignUp(WebUserDTO userDTO)
        {
            var userManager = await _userManager.FindByNameAsync(userDTO.Username);
            if (Object.Equals(userManager,null))
            {
                var user = _mapper.Map<WebUserDTO,IdentityUser>(userDTO);
                var userCreateResult = await _userManager.CreateAsync(user, userDTO.Password);
                if (userCreateResult.Succeeded)
                {
                    return Created(String.Empty, String.Empty);
                }
                return Problem(userCreateResult.Errors.First().Description, null, 500);
            }
            return Conflict("Username already exists");
        }

        [HttpPost("SignIn")]
        public async Task<IActionResult> SignIn(WebUserDTO userDTO)
        {
            IdentityUser user = await _userManager.FindByNameAsync(userDTO.Username);
           var passwordIsValid = await  _userManager.CheckPasswordAsync(user, userDTO.Password);
            if (passwordIsValid)
            {
                return Ok(new JwtTokenService().GenerateToken(user,_jwtSettings));
            }
            
            return BadRequest("username or password incorrect");
        }

    }
}

