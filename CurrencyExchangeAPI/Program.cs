
using CurrencyExchangeAPI.Services;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddHttpClient<ExchangeRateService>(client =>
{
  
    client.BaseAddress = new Uri("https://localhost:5001");
});


builder.Services.AddControllers();
builder.Services.AddMemoryCache();


builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
        builder.AllowAnyOrigin()  
               .AllowAnyMethod() 
               .AllowAnyHeader()); 
});

builder.Services.AddLogging();


var app = builder.Build();

app.UseCors("AllowAll");

app.UseRouting();

app.UseAuthorization();

app.MapControllers();

app.Run();
