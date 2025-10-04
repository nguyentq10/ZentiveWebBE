using Services.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.WriteIndented = true;
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Dependency Injection
builder.Services.AddScoped<IServiceProviders, ServiceProviders>();

var app = builder.Build();

// Configure the HTTP request pipeline
// Luôn bật Swagger (kể cả Production)
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "ZentiveAPI v1");
    c.RoutePrefix = "swagger"; // => http://localhost:8080/swagger
});

// Nếu bạn deploy Azure App Service có SSL sẵn thì có thể bật lại
// app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();
app.Urls.Clear();
app.Urls.Add("http://0.0.0.0:80");
app.Run();
