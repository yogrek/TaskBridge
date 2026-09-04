using TaskBridge.Api.CurrentUser;
using TaskBridge.Api.ExceptionHandling;
using TaskBridge.Api.Mapping;
using TaskBridge.Application;
using TaskBridge.Application.Abstractions.Security;
using TaskBridge.DB.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddProblemDetails(options =>
{
    options.CustomizeProblemDetails = context =>
    {
        context.ProblemDetails.Extensions["traceId"] = context.HttpContext.TraceIdentifier;
    };
});

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

builder.Services.AddApplication();
builder.Services.AddTaskBridgeDatabase(builder.Configuration);

builder.Services.AddAutoMapper(typeof(ApiMappingProfile));

builder.Services.AddHttpContextAccessor();

builder.Services.AddScoped<ICurrentUser, CurrentUser>();

builder.Services.AddAuthentication();
builder.Services.AddAuthorization();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi();

var app = builder.Build();

app.UseExceptionHandler();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
