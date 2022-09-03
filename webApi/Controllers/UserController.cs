﻿using API.Controllers.Base;
using AutoMapper;
using Domain;
using Domain.Dto.Account;
using Microsoft.AspNetCore.Mvc;
using MyBlog.Service.Auth;
using Service.Abstract;

namespace webApi.Controllers
{
    [Route("api/users")]
    public class UserController : AppBaseController
    {
        private readonly IUserService _userService;
        private readonly IMapper _mapper;

        public UserController(IUserService userService, IMapper mapper)
        {
            _mapper = mapper;
            _userService = userService;
        }

        [HttpGet]
        public async Task<IEnumerable<UserModel>> GetAllUsers()
        {
            var users = await _userService.GetAll();

            var userModels = users.Select(e => _mapper.Map<UserModel>(e));

            return userModels;
        }

        [HttpGet("current")]
        public async Task<UserModel> GetAuthenticatedUser()
        {
            var currentId = GetCurrentUserId();
            
            var user = await _userService.GetById(currentId);

            if (user != null)
            {
                return _mapper.Map<UserModel>(user);
            };

            return null;
        }
    }
}
