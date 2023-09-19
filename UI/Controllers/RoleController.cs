using Application.DTO.Helpers;
using Application.RequestsEventsHandlers.MedRHUI.RoleRH.Commands.PostRoleAuthenticate;
using Application.RequestsEventsHandlers.MedRHUI.RoleRH.Commands.RoleAdd;
using Application.RequestsEventsHandlers.MedRHUI.RoleRH.Commands.RoleAuthenticate;
using Application.RequestsEventsHandlers.MedRHUI.RoleRH.Commands.RoleConstraintLogin;
using Application.RequestsEventsHandlers.MedRHUI.RoleRH.Commands.RoleDelete;
using Application.RequestsEventsHandlers.MedRHUI.RoleRH.Commands.RoleUpdate;
using Application.RequestsEventsHandlers.MedRHUI.RoleRH.Queries.GetRoleByUid;
using Application.RequestsEventsHandlers.MedRHUI.RoleRH.Queries.GetRolesRequest;
using Application.RequestsEventsHandlers.MedRHUI.RoleRH.Queries.RoleAsHierachical;
using Core.DTO.Helpers;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using UI.Filters;
using UI.Models;

namespace UI.Controllers;


public class RoleController : Controller
{
    private readonly IMediator _mediator;
    public RoleController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [Authorize(ModuleHelper.Page), HttpGet]
    public IActionResult Index() => View();

    [HttpPost, ValidateAntiForgeryToken, Authorize(ModuleHelper.Feature)]
    public async Task<IActionResult> IndexFeature([FromBody] GetRolesRequest model) => Ok(await _mediator.Send(model));

    [Authorize(ModuleHelper.Page), HttpGet]
    public IActionResult Add([FromRoute] GetRequestModel model) => View(model);

    [HttpPost, ValidateAntiForgeryToken, Authorize(ModuleHelper.Feature)]
    public async Task<IActionResult> AddFeature([FromBody] RoleAddRequest model) => Ok(await _mediator.Send(model));

    [Authorize(ModuleHelper.Page), HttpGet]
    public IActionResult Update([FromRoute] GetRequestModel model) => View(model);

    [HttpPost, ValidateAntiForgeryToken, Authorize(ModuleHelper.Feature)]
    public async Task<IActionResult> GetRoleFeature([FromBody] GetRoleByUidRequest model) => Ok(await _mediator.Send(model));

    [HttpPost, ValidateAntiForgeryToken, Authorize(ModuleHelper.Feature)]
    public async Task<IActionResult> UpdateFeature([FromBody] RoleUpdateRequest model) => Ok(await _mediator.Send(model));


    [HttpPost, ValidateAntiForgeryToken, Authorize(ModuleHelper.Feature)]
    public async Task<IActionResult> DeleteFeature([FromBody] RoleDeleteRequest model) => Ok(await _mediator.Send(model));



    [Authorize(ModuleHelper.Page), HttpGet]
    public IActionResult Authenticate([FromRoute] GetRequestModel model) => View(model);

    [HttpPost, ValidateAntiForgeryToken, Authorize(ModuleHelper.Feature)]
    public async Task<IActionResult> GetAuthenticateFeature([FromBody] RoleAuthenticateRequest model) => Ok(await _mediator.Send(model));
    [HttpPost, ValidateAntiForgeryToken, Authorize(ModuleHelper.Feature)]
    public async Task<IActionResult> PostAuthenticateFeature([FromBody] PostRoleAuthenticateRequest model) => Ok(await _mediator.Send(model));






    [Authorize(ModuleHelper.Page), HttpGet]
    public IActionResult RoleAsHierachical([FromRoute] GetRequestModel model) => View(model);

    [HttpPost, ValidateAntiForgeryToken, Authorize(ModuleHelper.Feature)]
    public async Task<IActionResult> RoleAsHierachicalFeature([FromBody] RoleAsHierachicalRequest model) => Ok(await _mediator.Send(model));




    [HttpPost, ValidateAntiForgeryToken, Authorize(ModuleHelper.Feature)]
    public async Task<IActionResult> LoginConstraintFeature([FromBody] RoleConstraintLoginRequest model) => Ok(await _mediator.Send(model));


}