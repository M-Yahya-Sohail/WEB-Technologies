using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Project.Models;
namespace Project.Areas.Identity.Pages.Account;
[AllowAnonymous]
public class RegisterModel(UserManager<ApplicationUser> users, SignInManager<ApplicationUser> signIn) : PageModel {
 [BindProperty] public InputModel Input { get; set; } = new();
 public string? ReturnUrl { get; set; }
 public class InputModel {
  [Required, StringLength(100), Display(Name="Full name")] public string FullName { get; set; } = "";
  [Required, EmailAddress] public string Email { get; set; } = "";
  [Required, StringLength(100, MinimumLength=8), DataType(DataType.Password)] public string Password { get; set; } = "";
  [Required, DataType(DataType.Password), Compare("Password"), Display(Name="Confirm password")] public string ConfirmPassword { get; set; } = "";
 }
 public void OnGet(string? returnUrl=null) => ReturnUrl=returnUrl;
 public async Task<IActionResult> OnPostAsync(string? returnUrl=null) {
  ReturnUrl=returnUrl;
  if (!ModelState.IsValid) return Page();
  var user=new ApplicationUser { UserName=Input.Email, Email=Input.Email, FullName=Input.FullName.Trim() };
  var result=await users.CreateAsync(user,Input.Password);
  if (result.Succeeded) {
   var roleResult=await users.AddToRoleAsync(user,"Customer");
   if (!roleResult.Succeeded) { await users.DeleteAsync(user); ModelState.AddModelError("","Unable to create account. Try again."); return Page(); }
   await signIn.SignInAsync(user,false);
   return LocalRedirect(Url.IsLocalUrl(returnUrl) ? returnUrl! : "/Home/Customer");
  }
  foreach (var error in result.Errors) ModelState.AddModelError("",error.Description);
  return Page();
 }
}
