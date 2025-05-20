using AjudeiMais.Ecommerce.Controllers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddHttpClient("ApiAjudeiMais", client =>
{
    client.BaseAddress = new Uri("http://localhost:5168/");
});

builder.Services.AddSession();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseSession();
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "usuario-cadastrar",
    pattern: "usuario/cadastrar",
    defaults: new { controller = "Usuario", action = "Cadastro" });

app.MapControllerRoute(
    name: "usuario-entrar",
    pattern: "usuario/entrar",
    defaults: new { controller = "Usuario", action = "Entrar" });

app.MapControllerRoute(
    name: "usuario-perfil",
    pattern: "usuario/perfil/{id}",
    defaults: new { controller = "Usuario", action = "Entrar" });

app.MapControllerRoute(
    name: "usuario-login",
    pattern: "usuario/login",
    defaults: new { controller = "Home", action = "Login" });


app.Run();