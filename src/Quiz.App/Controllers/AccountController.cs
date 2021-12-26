﻿using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Quiz.App.Infrastructure.Repositories;
using Quiz.App.InputModels;
using Quiz.App.Mappings;
using Quiz.App.Models;
using Quiz.App.Services;

namespace Quiz.App.Controllers
{
    public class AccountController : Controller
    {
        private readonly IRepository<User> _repository;
        private readonly ITokenService _tokenService;

        public AccountController(IRepository<User> repository, ITokenService tokenService)
        {
            _repository = repository;
            _tokenService = tokenService;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
        
        [HttpPost]
        public async Task<IActionResult> Login(LoginInputModel inputModel)
        {
            if (!ModelState.IsValid)
            {
                return View(inputModel);
            }
            
            var user = await _repository.FirstAsync(x => x.Login == inputModel.Login);

            if (user is null)
            {
                ModelState.AddModelError("invalid", "login or password is invalid");

                return View(inputModel);
            }
                
            _tokenService.GenerateToken(user);

            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }
        
        [HttpPost]
        public async Task<IActionResult> Register(RegisterUserInputModel inputModel)
        {
            if (!ModelState.IsValid)
            {
                return View(inputModel);
            }
            
            var model = inputModel.ToModel();
            
            _repository.Add(model);

            await _repository.SaveAsync();
            
            return View();
        }
    }
}