using CryptoCurrencyExchange.Interfaces;
using CryptoCurrencyExchange.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);


// Add services to the container.
builder.Services.AddHttpClient("CryptoClient", client =>
{
	client.DefaultRequestHeaders.Add("User-Agent", "CryptoExchangeApp");
	client.Timeout = TimeSpan.FromSeconds(10);
});
builder.Services.AddScoped<ICryptoExchangeService, CryptoExchangeService>();
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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

app.Run();
