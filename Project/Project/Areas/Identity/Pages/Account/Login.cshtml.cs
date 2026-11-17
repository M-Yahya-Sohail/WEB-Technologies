using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Project.Models;
namespace Project.Areas.Identity.Pages.Account;
[AllowAnonymous]
public class LoginModel(SignInManager<ApplicationUser> signIn, UserManager<ApplicationUser> users) : PageModel {
 [BindProperty] public InputModel Input { get; set; } = new();
 public string? ReturnUrl { get; set; }
 public class InputModel {
  [Required, EmailAddress] public string Email { get; set; } = "";
  [Required, DataType(DataType.Password)] public string Password { get; set; } = "";
  [Display(Name="Remember me")] public bool RememberMe { get; set; }
 }
 public void OnGet(string? returnUrl=null) => ReturnUrl=returnUrl;
 public async Task<IActionResult> OnPostAsync(string? returnUrl=null) {
  ReturnUrl=returnUrl;
  if (!ModelState.IsValid) return Page();
  var result=await signIn.PasswordSignInAsync(Input.Email,Input.Password,Input.RememberMe,lockoutOnFailure:true);
  if (result.Succeeded) {
   var user=await users.FindByEmailAsync(Input.Email);
   var destination=await users.IsInRoleAsync(user!,"Admin") ? "/Admin" : "/Home/Customer";
   return LocalRedirect(Url.IsLocalUrl(returnUrl) ? returnUrl! : destination);
  }
  ModelState.AddModelError("",result.IsLockedOut ? "Account temporarily locked. Try again later." : "Invalid email or password.");
  return Page();
 }
}
