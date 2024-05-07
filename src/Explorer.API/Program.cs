using Explorer.API.Controllers;
using Explorer.API.Controllers.Administrator.Administration;
using Explorer.API.Controllers.Author;
using Explorer.API.Controllers.Author.TourAuthoring;
using Explorer.API.Controllers.Tourist.MarketPlace;
using Explorer.API.Startup;
using Explorer.Tours.Core.UseCases;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddHostedService<MailingListScheduler>();
builder.Services.ConfigureSwagger(builder.Configuration);
const string corsPolicy = "_corsPolicy";
builder.Services.ConfigureCors(corsPolicy);
builder.Services.ConfigureAuth();

builder.Services.RegisterModules();

builder.Services.AddGrpc().AddJsonTranscoding();

var app = builder.Build();


app.UseDeveloperExceptionPage();
app.UseSwagger();
app.UseSwaggerUI();


app.UseRouting();
app.UseCors(corsPolicy);
app.UseHttpsRedirection();
app.UseAuthorization();
app.UseAuthorization();
app.UseStaticFiles();

app.MapControllers();
app.MapGrpcService<FollowerController>();
app.MapGrpcService<FacilityController>();


app.Run();

// Required for automated tests
namespace Explorer.API
{
    public partial class Program { }
}