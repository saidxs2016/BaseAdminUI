using Application.RequestsEventsHandlers.MedRHUI.AdminRH.Commands.AdminAdd;
using Application.RequestsEventsHandlers.MedRHUI.AdminRH.Commands.AdminDelete;
using Application.RequestsEventsHandlers.MedRHUI.AdminRH.Commands.AdminUpdate;
using Application.RequestsEventsHandlers.MedRHUI.AdminRH.Commands.ChangeAdminPassword;
using Application.RequestsEventsHandlers.MedRHUI.AdminRH.Commands.LoginByConnectionKey;
using Application.RequestsEventsHandlers.MedRHUI.AdminRH.Commands.RoleChange;
using Application.RequestsEventsHandlers.MedRHUI.AdminRH.Commands.SuspendToggle;
using Application.RequestsEventsHandlers.MedRHUI.AdminRH.Queries.AdminGetByUid;
using Application.RequestsEventsHandlers.MedRHUI.AdminRH.Queries.GetAdmins;
using Application.RequestsEventsHandlers.MedRHUI.AdminRH.Queries.GetAllAdmins;
using Application.RequestsEventsHandlers.MedRHUI.AdminRH.Queries.GetRoles;
using Application.RequestsEventsHandlers.MedRHUI.AdminRH.Queries.GetUnConfirmedAdmins;
using Core.DTO.Helpers;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using UI.Filters;
using UI.Models;

namespace UI.Controllers;

public class AdminController : Controller
{
    private readonly IMediator _mediator;

    public AdminController(IMediator mediator)
    {
        _mediator = mediator;
    }


    [HttpGet, Authorize(ModuleHelper.Page)]
    public IActionResult Index([FromRoute] GetRequestModel model) => View(model);

    

    

    [HttpGet, Authorize(ModuleHelper.Page)]
    public IActionResult Add([FromRoute] GetRequestModel model) => View(model);

    [HttpPost, ValidateAntiForgeryToken, Authorize(ModuleHelper.Feature)]
    public async Task<IActionResult> AddFeature([FromBody] AdminAddRequest model) => Ok(await _mediator.Send(model));

    
    
    
    [HttpGet, Authorize(ModuleHelper.Page)]
    public IActionResult Update([FromRoute] GetRequestModel model) => View(model);


    [HttpPost, ValidateAntiForgeryToken, Authorize(ModuleHelper.Feature)]
    public async Task<IActionResult> GetAdminFeature([FromBody] AdminGetByUidRequest model) => Ok(await _mediator.Send(model));

    
    
    [HttpPost, ValidateAntiForgeryToken, Authorize(ModuleHelper.Feature)]
    public async Task<IActionResult> UpdateFeature([FromBody] AdminUpdateRequest model) => Ok(await _mediator.Send(model));




    [HttpGet, Authorize(ModuleHelper.Page)]
    public IActionResult Sub([FromRoute] GetRequestModel model) => View(model);

    [HttpPost, ValidateAntiForgeryToken, Authorize(ModuleHelper.Feature)]
    public async Task<IActionResult> SubFeature([FromBody] GetAdminsRequest model) => Ok(await _mediator.Send(model));

    [HttpPost, ValidateAntiForgeryToken, Authorize(ModuleHelper.Feature)]
    public async Task<IActionResult> DeleteFeature([FromBody] AdminDeleteRequest model) => Ok(await _mediator.Send(model));

    [HttpPost, ValidateAntiForgeryToken, Authorize(ModuleHelper.Feature)]
    public async Task<IActionResult> PasswordChangeFeature([FromBody] ChangeAdminPasswordRequest model) => Ok(await _mediator.Send(model));

    [HttpPost, ValidateAntiForgeryToken, Authorize(ModuleHelper.Feature)]
    public async Task<IActionResult> SuspendToogleFeature([FromBody] SuspendToggleRequest model) => Ok(await _mediator.Send(model));




    [HttpGet, Authorize(ModuleHelper.Page)]
    public IActionResult UnConfirmed([FromRoute] GetRequestModel model) => View(model);

    [HttpPost, ValidateAntiForgeryToken, Authorize(ModuleHelper.Feature)]
    public async Task<IActionResult> UnConfirmedFeature([FromBody] GetUnConfirmedAdminsRequest model) => Ok(await _mediator.Send(model));




    [HttpGet, Authorize(ModuleHelper.Page)]
    public IActionResult All([FromRoute] GetRequestModel model) => View(model);

    [HttpPost, ValidateAntiForgeryToken, Authorize(ModuleHelper.Feature)]
    public async Task<IActionResult> AllFeature([FromBody] GetAllAdminsRequest model) => Ok(await _mediator.Send(model));
    [HttpPost, ValidateAntiForgeryToken, Authorize(ModuleHelper.Feature)]
    public async Task<IActionResult> GetRolesFeature([FromBody] GetRolesRequest model) => Ok(await _mediator.Send(model));
    [HttpPost, ValidateAntiForgeryToken, Authorize(ModuleHelper.Feature)]
    public async Task<IActionResult> RoleChangeFeature([FromBody] RoleChangeRequest model) => Ok(await _mediator.Send(model));    

    [HttpPost, ValidateAntiForgeryToken, Authorize(ModuleHelper.Feature)]
    public async Task<IActionResult> LoginByConnectionKeyFeature([FromBody] LoginByConnectionKeyRequest model) => Ok(await _mediator.Send(model));



}