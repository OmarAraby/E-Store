using Estore.Application.DependancyInjection;
using Estore.Infrastructure.DependancyInjection;
using Microsoft.Extensions.FileProviders;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

//fluent validations
//builder.Services.AddFluentValidationAutoValidation();

builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddApplication();
builder.Services.AddIdentityServices(builder.Configuration);




var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

#region HandleFiles

// handle image upload
//if folder doesn't exist create it
var imageFolderPath = Path.Combine(Directory.GetCurrentDirectory(), "Upload");
Directory.CreateDirectory(imageFolderPath);


app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(imageFolderPath),
    RequestPath = "/api/static-files"
});
#endregion

app.MapControllers();

app.Run();
