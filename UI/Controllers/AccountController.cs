using Application.DTO.Helpers;
using Application.RequestsEventsHandlers.MedRHUI.AccountRH.Commands.Activation;
using Application.RequestsEventsHandlers.MedRHUI.AccountRH.Commands.ForgetPassword;
using Application.RequestsEventsHandlers.MedRHUI.AccountRH.Commands.Login;
using Application.RequestsEventsHandlers.MedRHUI.AccountRH.Commands.Logout;
using Application.RequestsEventsHandlers.MedRHUI.AccountRH.Commands.Register;
using Application.RequestsEventsHandlers.MedRHUI.AccountRH.Commands.ResetPassword;
using Application.RequestsEventsHandlers.MedRHUI.AccountRH.Commands.ResetPasswordPage;
using Core.Services.CacheService.MicrosoftInMemory;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace UI.Controllers;

public class AccountController : Controller
{
    private readonly IMediator _mediator;

    private readonly IMemoryCacheService _cacheService;

    public AccountController(IMediator mediator, IMemoryCacheService cacheService)
    {
        _mediator = mediator;
        _cacheService = cacheService;
    }

    
    public IActionResult Login()
    {
        if (User == null || !User.Identity.IsAuthenticated)
            return View();
        try
        {
            var authId = User.Claims.First(w => w.Type == "AuthID").Value;
            var authInfo = _cacheService.Get<CacheAuthInfoHelper>(authId);
            if(authInfo == null)
                return View(); 

            if (!string.IsNullOrEmpty(Request.Query["returnURL"]))
                return Redirect(Request.Query["returnURL"]);   

            if (!string.IsNullOrEmpty(authInfo?.RoleRoute))
                return Redirect(authInfo.RoleRoute);

            authInfo = null;
        }
        catch (Exception)
        {
            return View();
        }
        return Redirect("/Admin/Index");
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Login([FromBody] LoginRequest model) => Ok(await _mediator.Send(model));

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout([FromBody] LogoutRequest model) => Ok(await _mediator.Send(model));



    // ===========  Register işlemi ===========
    [HttpGet("/Register")]
    public IActionResult Register()
    {
        if (User == null || !User.Identity.IsAuthenticated)
            return View();

        var authId = User.Claims.First(w => w.Type == "AuthID").Value;
        var authInfo = _cacheService.Get<CacheAuthInfoHelper>(authId);
        if (authInfo == null)
            return View();

        if (!string.IsNullOrEmpty(Request.Query["returnURL"]))
            return Redirect(Request.Query["returnURL"]);

        return Redirect("/admin/index");
    }
    [HttpPost("/Register"), ValidateAntiForgeryToken]
    public async Task<IActionResult> Register([FromBody] RegisterRequest model) => Ok(await _mediator.Send(model));



    // ===========  Register işlemi sonrası hesap aktivasyon işlemleri ===========
    [HttpGet("/Activation/{token}")]
    public async Task Activation([FromRoute] ActivationRequest model) => await _mediator.Send(model);
    [HttpGet("/ActivationResult")]
    public IActionResult ActivationResult() => View();



    // ===========  Şifremi unuttum işlemleri ===========
    [HttpGet("/ForgetPassword")]
    public IActionResult ForgetPassword() => View();
    [HttpPost("/ForgetPassword"), ValidateAntiForgeryToken]
    public async Task<IActionResult> ForgetPassword([FromBody] ForgetPasswordRequest model) => Ok(await _mediator.Send(model));


    // ===========  Şifremi unuttum işlemi sonrası emailden şifre resetleme işlemi ===========
    [HttpGet("/ResetPassword/{token}")]
    public async Task ResetPassword([FromRoute] ResetPasswordRequest model) => await _mediator.Send(model);
    [HttpGet("/ResetPasswordFailedPage")]
    public IActionResult ResetPasswordFailedPage() => View();


    // ===========  Şifremi unuttum işlemi sonrası emailden şifre resetleme işlemi (yeni şifre girme sayfası) ===========
    [HttpGet("/ResetPasswordPage")]
    public IActionResult ResetPasswordPage() => View();
    [HttpPost("/ResetPasswordPage"), ValidateAntiForgeryToken]
    public async Task<IActionResult> ResetPasswordPage([FromBody] ResetPasswordPageRequest model) => Ok(await _mediator.Send(model));
 

}