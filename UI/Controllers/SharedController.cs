using Application.RequestsEventsHandlers.MedNotificationsHandlers.Contracts;
using Application.RequestsEventsHandlers.MedRHUI.HelperRH.Queries.BreadCrumb;
using Application.RequestsEventsHandlers.MedRHUI.HelperRH.Queries.HideIgnoreSection;
using Core.DTO.Helpers;
using Core.Functions_Extensions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Serilog.Context;
using Serilog.Core.Enrichers;
using UI.Filters;

namespace UI.Controllers;

public class SharedController : Controller
{
    private readonly IMediator _mediator;
    private readonly ILogger<SharedController> _logger;

    public SharedController(IMediator mediator, ILogger<SharedController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }


    [HttpPost, ValidateAntiForgeryToken, AuthorizeShared(ModuleHelper.Feature)]
    public async Task<IActionResult> HideIgnoreSectionRequest([FromBody] HideIgnoreSectionRequest model) => Ok(await _mediator.Send(model));


    [HttpPost, ValidateAntiForgeryToken, AuthorizeShared(ModuleHelper.Feature)]
    public async Task<IActionResult> BreadCrumbRequest([FromBody] BreadCrumbRequest model) => Ok(await _mediator.Send(model));


    [HttpGet, AuthorizeShared(ModuleHelper.Page)]
    public async Task<IActionResult> NewSession([FromQuery] string to)
    {

        var enrichers = await RequestEnrichersFunction.EventEnrichers(HttpContext);
        enrichers.Add(new PropertyEnricher("StatusCode", StatusCodes.Status200OK));
        enrichers.Add(new PropertyEnricher("LogLevel", "Info"));

        using (LogContext.Push(enrichers.ToArray()))
        {
            _logger.LogWarning("Vekalet ile giriş işlem başarılı.");
        }

        return Redirect(to);
    }



    [HttpPost]
    public Task SendMailNotification([FromBody] BackJobMN model)
    {
        //await _mediator.Publish(model);

        return Task.CompletedTask;
    }
}
