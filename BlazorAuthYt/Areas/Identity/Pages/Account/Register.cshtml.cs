using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace BlazorAuthYt.Areas.Identity.Pages.Account
{
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public RegisterModel(SignInManager<IdentityUser> signInManager, UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        [BindProperty]
        public InputModel Input { get; set; }
        public string ReturnUrl { get; set; }

        public void OnGet()
        {
            ReturnUrl = Url.Content("~/");
        }

        public async Task<IActionResult> OnPostAsync()
        {
            ReturnUrl = Url.Content("~/");
            if (ModelState.IsValid)
            {
                var identity = new IdentityUser { UserName = Input.Email, Email = Input.Email };
                var result = await _userManager.CreateAsync(identity, Input.Password);

                var claim = new Claim("city", Input.City.ToLower());
                var claimsResult = await _userManager.AddClaimAsync(identity, claim);

                var role = new IdentityRole(Input.Role);
                var addRoleResult = await _roleManager.CreateAsync(role);

                var addUserRoleResult = await _userManager.AddToRoleAsync(identity, Input.Role);


                if (result.Succeeded && claimsResult.Succeeded && addRoleResult.Succeeded 
                    && addUserRoleResult.Succeeded) 
                {
                    await _signInManager.SignInAsync(identity, isPersistent: false);
                    return LocalRedirect(ReturnUrl);
                }
            }

            return Page();
        }

        public class InputModel
        {
            [Required]
            [EmailAddress]
            public string Email { get; set;}

            [Required]
            [DataType(DataType.Password)]
            public string Password { get; set;}

            [Required]
            public string City { get; set; }

            [Required]
            public string Role { get; set; }
        }
    }
}
