// -----------------------------------------------------------------------
//  <copyright file = "PatientType.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using Prism.ProAssistant.Business.Models;

namespace Prism.ProAssistant.Api.Graph.Meetings;

public class MeetingType : ObjectType<Meeting>
{
    public MeetingType() : base(ConfigureMeeting)
    {
    }

    internal static void ConfigureMeeting(IObjectTypeDescriptor<Meeting> descriptor)
    {
        descriptor.Field(_ => _.Id);
        descriptor.Field(_ => _.Duration);
        descriptor.Field(_ => _.Payment);
        descriptor.Field(_ => _.Price);
        descriptor.Field(_ => _.State);
        descriptor.Field(_ => _.Title);
        descriptor.Field(_ => _.FirstName);
        descriptor.Field(_ => _.LastName);
        descriptor.Field(_ => _.PatientId);
        descriptor.Field(_ => _.PaymentDate);
        descriptor.Field(_ => _.StartDate);
        descriptor.Field(_ => _.Type);
    }
}