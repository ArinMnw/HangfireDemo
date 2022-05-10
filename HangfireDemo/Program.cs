using Hangfire;
using Hangfire.SqlServer;
using HangfireDemo.Controllers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddHangfire(configuration =>
{
    configuration.UseSqlServerStorage(builder.Configuration.GetConnectionString("HangfireConnection"));
});
builder.Services.AddHangfireServer();
builder.Services.AddHttpClient("ApiService");
//IBackgroundJobClient client = new BackgroundJobClient(new SqlServerStorage(builder.Configuration.GetConnectionString("HangfireConnection")));
//client.Enqueue(() =>
//    Console.WriteLine("Hello world from Hangfire")
//);
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();
app.UseHangfireDashboard();

app.Run();
