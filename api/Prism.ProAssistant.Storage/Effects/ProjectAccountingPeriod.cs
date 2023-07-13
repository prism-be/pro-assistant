namespace Prism.ProAssistant.Storage.Effects;

using Core.Attributes;
using Domain.DayToDay.Appointments.Events;

[SideEffect(typeof(AppointmentCreated))]
[SideEffect(typeof(AppointmentUpdated))]
public class ProjectAccountingPeriod
{
    
}