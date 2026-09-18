using ArchUnitNET.Domain;
using ArchUnitNET.Fluent;
using ArchUnitNET.xUnit;

using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using TaskBridge.Api.Mapping;

using static ArchUnitNET.Fluent.ArchRuleDefinition;

namespace TaskBridge.ArchitectureTests;

public sealed class LayerArchitectureTests
{
    private static readonly IObjectProvider<IType> Domain = Types().That().ResideInNamespaceMatching("^TaskBridge\\.Domain(\\..*)?$");
    private static readonly IObjectProvider<IType> Application = Types().That().ResideInNamespaceMatching("^TaskBridge\\.Application(\\..*)?$");
    private static readonly IObjectProvider<IType> Contracts = Types().That().ResideInNamespaceMatching("^TaskBridge\\.Contracts(\\..*)?$");
    private static readonly IObjectProvider<IType> Persistence = Types().That().ResideInNamespaceMatching("^TaskBridge\\.DB(\\..*)?$");
    private static readonly IObjectProvider<IType> Api = Types().That().ResideInNamespaceMatching("^TaskBridge\\.Api(\\..*)?$");
    private static readonly IObjectProvider<IType> Desktop = Types().That().ResideInNamespaceMatching("^TaskBridge\\.Desktop(\\..*)?$");

    [Fact]
    public void Domain_ShouldNotDependOnOtherTaskBridgeLayers() =>
        Types().That().Are(Domain).Should().NotDependOnAny(Application).AndShould().NotDependOnAny(Persistence)
            .AndShould().NotDependOnAny(Api).AndShould().NotDependOnAny(Desktop).AndShould().NotDependOnAny(Contracts)
            .Because("Domain must remain independent from other TaskBridge layers")
            .Check(TestArchitecture.Architecture);

    [Fact]
    public void Application_ShouldNotDependOnApiPersistenceOrDesktop() =>
        Types().That().Are(Application).Should().NotDependOnAny(Api).AndShould().NotDependOnAny(Persistence)
            .AndShould().NotDependOnAny(Desktop)
            .Because("Application must not depend on delivery or persistence layers")
            .Check(TestArchitecture.Architecture);

    [Fact]
    public void Contracts_ShouldNotDependOnDomainApplicationOrPersistence() =>
        Types().That().Are(Contracts).Should().NotDependOnAny(Domain).AndShould().NotDependOnAny(Application)
            .AndShould().NotDependOnAny(Persistence)
            .Because("Contracts must remain transport-only")
            .Check(TestArchitecture.Architecture);

    [Fact]
    public void Desktop_ShouldNotDependOnPersistence() =>
        Types().That().Are(Desktop).Should().NotDependOnAny(Persistence)
            .Because("Desktop must not access persistence directly")
            .Check(TestArchitecture.Architecture);

    [Fact]
    public void Controllers_ShouldResideOnlyInApi() =>
        Classes().That().AreAssignableTo(typeof(ControllerBase)).Should().ResideInNamespaceMatching("^TaskBridge\\.Api\\.Controllers$")
            .Because("controllers belong to the API layer")
            .Check(TestArchitecture.Architecture);

    [Fact]
    public void EntityTypeConfigurations_ShouldResideOnlyInPersistence() =>
        Classes().That().AreAssignableTo(typeof(IEntityTypeConfiguration<>)).Should().ResideInNamespaceMatching("^TaskBridge\\.DB\\.Configurations$")
            .Because("EF configurations belong to persistence")
            .Check(TestArchitecture.Architecture);

    [Fact]
    public void ApplicationHandlers_ShouldEndWithHandler() =>
        Classes().That().ResideInNamespaceMatching("^TaskBridge\\.Application(\\..*)?$").And().HaveNameContaining("Handler")
            .Should().HaveNameEndingWith("Handler")
            .Because("application handler names are part of the convention")
            .Check(TestArchitecture.Architecture);

    [Fact]
    public void AutoMapperProfiles_ShouldResideInApiMapping() =>
        Classes().That().AreAssignableTo(typeof(Profile)).Should().ResideInNamespace(typeof(ApiMappingProfile).Namespace!)
            .Because("API mapping profiles must reside in Api.Mapping")
            .Check(TestArchitecture.Architecture);
}
