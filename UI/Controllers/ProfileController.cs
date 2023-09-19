using Application.DTO.Helpers;
using Application.RequestsEventsHandlers.MedRHUI.ProfileRH.Commands.ChangePassword;
using Application.RequestsEventsHandlers.MedRHUI.ProfileRH.Commands.ConnectionKeyDelete;
using Application.RequestsEventsHandlers.MedRHUI.ProfileRH.Commands.GenerateConnectionKey;
using Application.RequestsEventsHandlers.MedRHUI.ProfileRH.Commands.ProfileUpdate;
using Application.RequestsEventsHandlers.MedRHUI.ProfileRH.Queries.Profile;
using Core.DTO.Helpers;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using UI.Filters;
using UI.Models;

namespace UI.Controllers;

public class ProfileController : Controller
{  
    private readonly IMediator _mediator;

    public ProfileController(IMediator mediator)
    {
        _mediator = mediator;
    }


    [HttpGet, Authorize(ModuleHelper.Page)]
    public IActionResult Index([FromRoute] GetRequestModel model) => View(model);

    [HttpPost, ValidateAntiForgeryToken, Authorize(ModuleHelper.Feature)]
    public async Task<IActionResult> GetProfileFeature([FromBody] ProfileRequest model) => Ok(await _mediator.Send(model));
     
      
    [HttpPost, ValidateAntiForgeryToken, Authorize(ModuleHelper.Feature)]
    public async Task<IActionResult> UpdateFeature([FromBody] ProfileUpdateRequest model) => Ok(await _mediator.Send(model));
     

    [HttpPost, ValidateAntiForgeryToken, Authorize(ModuleHelper.Feature)]
    public async Task<IActionResult> PasswordChangeFeature([FromBody] ChangePasswordRequest model) => Ok(await _mediator.Send(model));

    [HttpPost, ValidateAntiForgeryToken, Authorize(ModuleHelper.Feature)]
    public async Task<IActionResult> GenerateConnectionKeyFeature([FromBody] GenerateConnectionKeyRequest model) => Ok(await _mediator.Send(model));
    [HttpPost, ValidateAntiForgeryToken, Authorize(ModuleHelper.Feature)]
    public async Task<IActionResult> ConnectionKeyDeleteFeature([FromBody] ConnectionKeyDeleteRequest model) => Ok(await _mediator.Send(model));

}