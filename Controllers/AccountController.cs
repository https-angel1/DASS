using Azure.Core;
using Azure.Identity;
using DASS.Models;
using DASS.ViewModels;
using DASS.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using static System.Runtime.InteropServices.JavaScript.JSType;


namespace DASS.Controllers
{
	public class AccountController : Controller
	{
		private readonly SignInManager<Users> signInManager;
		private readonly UserManager<Users> userManager;

		public AccountController(SignInManager<Users> signInManager, UserManager<Users> userManager)
		{
			this.signInManager = signInManager;
			this.userManager = userManager;
		}

		public IActionResult Login()
		{
			return View();
		}

		[HttpPost]
		public async Task<IActionResult> Login(LoginViewModel model)
		{
			if (ModelState.IsValid)
			{
				var result = await signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, false);

				if (result.Succeeded)
				{
					return RedirectToAction("Index", "Home");
				}
				else
				{
					ModelState.AddModelError("", "Email or password is incorrect.");
					return View(model);
				}
			}
			return View(model);
		}


		public IActionResult Register()
		{
			return View();
		}

		[HttpPost]
		public async Task<IActionResult> Register(RegisterViewModel model)
		{
			if (ModelState.IsValid)
			{
				var user = new Users
				{
					FirstName = model.FirstName,
					MiddleName = model.MiddleName,
					LastName = model.LastName,
					Email = model.Email,
					UserName = model.Email,
				};

				var result = await userManager.CreateAsync(user, model.Password);

				if (result.Succeeded)
				{
					var token = await userManager.GenerateEmailConfirmationTokenAsync(user);
					var confirmationLink = Url.Action("ConfirmationEmail", "Account", new { userId = user.Id, token = token }, Request.Scheme);

					var emailService = HttpContext.RequestServices.GetRequiredService<EmailService>();
					await emailService.SendEmailAsync(user.Email, "Verify your email", $"Please confirm your email by clicking <a href='{confirmationLink}'>here</a>");

					return RedirectToAction("Login", "Account");
				}
				else
				{
					foreach (var error in result.Errors)
					{
						ModelState.AddModelError("", error.Description);
					}
					return View(model);
				}
			}
			return View(model);
		}

		// GET:ConfirmEMail
		[HttpGet]
		public async Task<IActionResult> ConfirmEmail(string userId, string token)
		{
			if (userId == null || token == null)
			{
				return BadRequest("Invalid email confirmation request.");
			}

			var user = await userManager.FindByIdAsync(userId);
			if (user == null)
			{
				return NotFound("User not found.");
			}

			var result = await userManager.ConfirmEmailAsync(user, token);
			if (result.Succeeded)
			{
				return RedirectToAction("Login", "Account");
			}

			return BadRequest("Email confirmation failed.");
		}

        // verify email action
		public IActionResult VerifyEmail()
		{
			return View();
		}

        // POST: VerifyEmail
        [HttpPost]
        public async Task<IActionResult> VerifyEmail(VerifyEmailViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await userManager.FindByNameAsync(model.Email);

                if (user == null)
                {
                    ModelState.AddModelError("", "something is wrong!");
                    return View(model);
                }
                else
                {
                    return RedirectToAction("ChangePassword", "Account", new { username = user.UserName });
                }
            }
            return View(model);
        }

        // change password action
        public IActionResult ChangePassword(string username)
        {
            if (string.IsNullOrEmpty(username))
            {
                return RedirectToAction("VerifyEmail", "Account");
            }
            return View(new ChangePasswordViewModel { Email = username });
        }

		// POST: ChangePassword
		[HttpPost]
		public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
		{
			if (ModelState.IsValid)
			{
				var user = await userManager.FindByNameAsync(model.Email);
				if (user != null)
				{
					var result = await userManager.RemovePasswordAsync(user);
					if (result.Succeeded)
					{
						result = await userManager.AddPasswordAsync(user, model.NewPassword);
						return RedirectToAction("Login", "Account");
					}
					else
					{

						foreach (var error in result.Errors)
						{
							ModelState.AddModelError("", error.Description);
						}

						return View(model);
					}
				}
				else
				{
					ModelState.AddModelError("", "Email not found!");
					return View(model);
				}
			}
			else
			{
				ModelState.AddModelError("", "Something went wrong. try again.");
				return View(model);
			}
		}


		public async Task<IActionResult> Logout()
		{
			await signInManager.SignOutAsync();
			return RedirectToAction("Index", "Home");
		}
	}
}
