using Application.DTO.Helpers;
using Application.RequestsEventsHandlers.MedRHUI.ModuleRH.Commands.ModuleAdd;
using Application.RequestsEventsHandlers.MedRHUI.ModuleRH.Commands.ModuleChangeParent;
using Application.RequestsEventsHandlers.MedRHUI.ModuleRH.Commands.ModuleDelete;
using Application.RequestsEventsHandlers.MedRHUI.ModuleRH.Commands.ModuleOrderRecords;
using Application.RequestsEventsHandlers.MedRHUI.ModuleRH.Commands.ModuleUpdate;
using Application.RequestsEventsHandlers.MedRHUI.ModuleRH.Queries.GetAllModules;
using Application.RequestsEventsHandlers.MedRHUI.ModuleRH.Queries.GetModuleByUid;
using Application.RequestsEventsHandlers.MedRHUI.ModuleRH.Queries.GetModules;
using Core.DTO.Helpers;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using UI.Filters;
using UI.Models;

namespace UI.Controllers;


public class ModuleController : Controller
{
    private readonly IMediator _mediator;
    public ModuleController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet, Authorize(ModuleHelper.Page)]
    public IActionResult Index([FromRoute] GetRequestModel model) =>  View(model);
    [HttpPost, ValidateAntiForgeryToken, Authorize(ModuleHelper.Feature)]
    public async Task<IActionResult> IndexFeature([FromForm] GetModulesRequest model) => Ok(await _mediator.Send(model));
    [HttpGet, Authorize(ModuleHelper.Page)]
    public IActionResult All([FromRoute] GetRequestModel model) => View(model);

    [HttpPost, ValidateAntiForgeryToken, Authorize(ModuleHelper.Feature)]
    public async Task<IActionResult> AllFeature([FromForm] GetAllModulesRequest model) => Ok(await _mediator.Send(model));

    [HttpPost, ValidateAntiForgeryToken, Authorize(ModuleHelper.Feature)]
    public async Task<IActionResult> ChangeParentFeature([FromBody] ModuleChangeParentRequest model) => Ok(await _mediator.Send(model));

    [HttpGet, Authorize(ModuleHelper.Page)]
    public IActionResult Add([FromRoute] GetRequestModel model) => View(model);

    [HttpPost, ValidateAntiForgeryToken, Authorize(ModuleHelper.Feature)]
    public async Task<IActionResult> AddFeature([FromBody] ModuleAddRequest model) => Ok(await _mediator.Send(model));

    [HttpGet, Authorize(ModuleHelper.Page)]
    public IActionResult Update([FromRoute] GetRequestModel model) => View(model);

    [HttpPost, ValidateAntiForgeryToken, Authorize(ModuleHelper.Feature)]
    public async Task<IActionResult> GetModuleFeature([FromBody] GetModuleByUidRequest model) => Ok(await _mediator.Send(model));
    [HttpPost, ValidateAntiForgeryToken, Authorize(ModuleHelper.Feature)]
    public async Task<IActionResult> GetModulesFeature([FromBody] GetModulesRequest model) => Ok(await _mediator.Send(model));

    [HttpPost, ValidateAntiForgeryToken, Authorize(ModuleHelper.Feature)]
    public async Task<IActionResult> UpdateFeature([FromBody] ModuleUpdateRequest model) => Ok(await _mediator.Send(model));


    [HttpPost, ValidateAntiForgeryToken, Authorize(ModuleHelper.Feature)]
    public async Task<IActionResult> DeleteFeature([FromBody] ModuleDeleteRequest model) => Ok(await _mediator.Send(model));




    [HttpPost, ValidateAntiForgeryToken, Authorize(ModuleHelper.Feature)]
    public async Task<IActionResult> OrderRecordsFeature([FromBody] ModuleOrderRecordsRequest model) => Ok(await _mediator.Send(model));


}