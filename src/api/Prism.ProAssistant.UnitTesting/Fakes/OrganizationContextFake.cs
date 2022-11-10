// -----------------------------------------------------------------------
//  <copyright file = "OrganizationContextFake.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using MongoDB.Driver;
using Moq;
using Prism.ProAssistant.Business.Models;
using Prism.ProAssistant.Business.Security;
using Prism.ProAssistant.Business.Storage;

namespace Prism.ProAssistant.UnitTesting.Fakes;
/*
public class OrganizationContextFake : IOrganizationContext
{

    public OrganizationContextFake()
    {
        Database = new Mock<IMongoDatabase>();

        HistoryReplace = new History
        {
            Id = Identifier.GenerateString()
        };

        PatientsReplace = new Patient
        {
            Id = Identifier.GenerateString()
        };

        MeetingsReplace = new Meeting
        {
            Id = Identifier.GenerateString()
        };

        SettingsReplace = new Setting
        {
            Id = Identifier.GenerateString()
        };

        TariffsReplace = new Tariff
        {
            Id = Identifier.GenerateString()
        };

        HistoryMock = Database.SetupCollectionAndReplace(HistoryReplace);
        PatientsMock = Database.SetupCollectionAndReplace(PatientsReplace);
        MeetingsMock = Database.SetupCollectionAndReplace(MeetingsReplace);
        SettingsMock = Database.SetupCollectionAndReplace(SettingsReplace);
        TariffsMock = Database.SetupCollectionAndReplace(TariffsReplace);
    }

    public History HistoryReplace { get; set; }
    public Meeting MeetingsReplace { get; set; }

    public Mock<IMongoCollection<History>> HistoryMock { get; set; }
    public Mock<IMongoCollection<Meeting>> MeetingsMock { get; set; }
    public Mock<IMongoCollection<Patient>> PatientsMock { get; set; }
    public Mock<IMongoCollection<Setting>> SettingsMock { get; set; }
    public Mock<IMongoCollection<Tariff>> TariffsMock { get; set; }
    public Mock<IMongoDatabase> Database { get; }
    public Patient PatientsReplace { get; set; }
    public Setting SettingsReplace { get; set; }
    public Tariff TariffsReplace { get; set; }

    public IMongoCollection<History> History => HistoryMock.Object;
    public IMongoCollection<Patient> Patients => PatientsMock.Object;
    public IMongoCollection<Meeting> Meetings => MeetingsMock.Object;
    public IMongoCollection<Setting> Settings => SettingsMock.Object;
    public IMongoCollection<Tariff> Tariffs => TariffsMock.Object;
}*/