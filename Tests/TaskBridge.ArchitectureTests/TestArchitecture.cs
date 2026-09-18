using ArchUnitNET.Domain;
using ArchUnitNET.Loader;

using TaskBridge.Api.Controllers;
using TaskBridge.Application.Common;
using TaskBridge.Contracts.Common;
using TaskBridge.DB;
using TaskBridge.Desktop.ViewModels;
using TaskBridge.Domain.Users;
using TaskBridge.Infrastructure.Security;

namespace TaskBridge.ArchitectureTests;

public static class TestArchitecture
{
    public static readonly Architecture Architecture = new ArchLoader()
        .LoadAssemblies(
            typeof(User).Assembly,
            typeof(Result).Assembly,
            typeof(PagedResponse<>).Assembly,
            typeof(AppDbContext).Assembly,
            typeof(TasksController).Assembly,
            typeof(MainWindowViewModel).Assembly,
            typeof(JwtAccessTokenProvider).Assembly)
        .Build();
}
