// -----------------------------------------------------------------------
//  <copyright file = "OrganizationContextFake.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System;
using MongoDB.Driver;
using Moq;
using Prism.ProAssistant.Business.Models;
using Prism.ProAssistant.Business.Security;
using Prism.ProAssistant.Business.Storage;

namespace Prism.ProAssistant.Api.Tests.Fakes;

public class OrganizationContextFake : IOrganizationContext
{

    public OrganizationContextFake()
    {
        var database = new Mock<IMongoDatabase>();

        HistoryReplace = new History()
        {
            Id = Identifier.GenerateString()
        };

        PatientsReplace = new Patient
        {
            Id = Identifier.GenerateString()
        };

        TariffsReplace = new Tariff
        {
            Id = Identifier.GenerateString()
        };

        HistoryMock = new Lazy<Mock<IMongoCollection<History>>>(() => database.SetupCollectionAndReplace(HistoryReplace));
        PatientsMock = new Lazy<Mock<IMongoCollection<Patient>>>(() => database.SetupCollectionAndReplace(PatientsReplace));
        TariffsMock = new Lazy<Mock<IMongoCollection<Tariff>>>(() => database.SetupCollectionAndReplace(TariffsReplace));
    }

    public History HistoryReplace { get; set; }
    public Patient PatientsReplace { get; set; }
    public Tariff TariffsReplace { get; set; }

    public Lazy<Mock<IMongoCollection<History>>> HistoryMock { get; }
    public Lazy<Mock<IMongoCollection<Patient>>> PatientsMock { get; }
    public Lazy<Mock<IMongoCollection<Tariff>>> TariffsMock { get; }
    
    public IMongoCollection<History> History => HistoryMock.Value.Object;
    public IMongoCollection<Patient> Patients => PatientsMock.Value.Object;
    public IMongoCollection<Tariff> Tariffs => TariffsMock.Value.Object;
}