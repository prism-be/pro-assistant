namespace Prism.ProAssistant.Api.Services.Listeners;

using Azure.Messaging.ServiceBus;
using Domain.DayToDay.Appointments;

public class AppointmentEventServiceBusListener: BaseDomainEventServiceBusListener<Appointment>
{
    public AppointmentEventServiceBusListener(ServiceBusClient serviceBusClient, IServiceProvider serviceProvider) : base(serviceBusClient, serviceProvider)
    {
    }
}