using MediatR;

namespace Application.RequestsEventsHandlers.MedNotificationsHandlers.Contracts;


public record BackJobMN(string? message) : INotification;
