using SubscriptionService.Web;
using SubscriptionService.Web.Middlewares;

var builder = WebApplication.CreateBuilder(args);

builder.Services.ConfigureApp(builder.Configuration);

var app = builder.Build();

app.UseMiddleware<ExceptionMiddleware>();
app.UseMiddleware<RequestLoggingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapControllers();

await app.RunAsync().ConfigureAwait(false);