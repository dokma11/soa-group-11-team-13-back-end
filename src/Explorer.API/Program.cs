using Explorer.API.Controllers;
using Explorer.API.Controllers.Author;
using Explorer.API.Startup;
using Explorer.Tours.Core.UseCases;
using EquipmentController = Explorer.API.Controllers.Administrator.Administration.EquipmentController;
using CommentController = Explorer.API.Controllers.Tourist.CommentController;
using KeyPointController = Explorer.API.Controllers.Author.TourAuthoring.KeyPointController;
using TourController = Explorer.API.Controllers.Author.TourAuthoring.TourController;

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
app.MapGrpcService<TourController>();
app.MapGrpcService<KeyPointController>();
app.MapGrpcService<EquipmentController>();
app.MapGrpcService<CommentController>();
app.MapGrpcService<BlogController>();


app.Run();

// Required for automated tests
namespace Explorer.API
{
    public partial class Program { }
}