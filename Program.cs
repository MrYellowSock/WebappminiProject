// <snippet_UsingModels>
using BookStoreApi.Models;
using BookStoreApi.Services;
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(options =>
{
    options.Cookie.Name = "Tracker";
    options.Cookie.IsEssential = true;
});

builder.Services.AddControllersWithViews();

builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();


app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAuthorization();
app.UseSession();

app.MapControllers();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.Run();