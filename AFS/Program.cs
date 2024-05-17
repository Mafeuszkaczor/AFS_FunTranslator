using AFS.Services;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Configure HttpClient
builder.Services.AddHttpClient("FunTranslationsApi", client =>
{
    client.BaseAddress = new Uri("https://api.funtranslations.com/");
});

// Register the FunTranslationsService
builder.Services.AddTransient<IFunTranslationsService, FunTranslationsService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Translation}/{action=Index}/{id?}");

app.Run();
