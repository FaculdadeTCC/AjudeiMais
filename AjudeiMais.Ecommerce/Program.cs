using System;
using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.DataProtection;

var builder = WebApplication.CreateBuilder(args);

//
// 1) Data Protection: persiste key-ring em disco para que cookies de autenticação continuem válidos
//
var keysFolder = Path.Combine(
    Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
    "AjudeiMais", "DataProtectionKeys"
);
Directory.CreateDirectory(keysFolder);

builder.Services.AddDataProtection()
    .PersistKeysToFileSystem(new DirectoryInfo(keysFolder))
    .SetApplicationName("AjudeiMais");

//
// 2) MVC + HTTP Client + Session
//
builder.Services.AddControllersWithViews();

builder.Services.AddHttpClient("ApiAjudeiMais", client =>
{
    client.BaseAddress = new Uri("http://localhost:5168/");
});

builder.Services.AddSession();

//
// 3) Autenticação e Autorização baseado em Cookie
//
builder.Services
    .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
    {
        options.LoginPath = "/login";
        options.AccessDeniedPath = "/home";
        options.ExpireTimeSpan = TimeSpan.FromDays(1);
        options.SlidingExpiration = true;
        options.Cookie.HttpOnly = true;
        options.Cookie.IsEssential = true;
    });

// Necessário para o [Authorize]
builder.Services.AddAuthorization();

var app = builder.Build();

//
// Pipeline HTTP
//
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseSession();
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();


app.MapControllerRoute(
    name: "usuario-anuncio-cadastrar",
    pattern: "usuario/{guid}/anunciar",
    defaults: new { controller = "Produto", action = "Cadastro" }
);

#region ADMIN

#region CATEGORIA DE PRODUTO
app.MapControllerRoute(
    name: "admin-categoria-produto-cadastrar",
    pattern: "admin/categoria-do-produto/cadastrar",
    defaults: new { controller = "Categoria", action = "Cadastro" }
);
#endregion
#endregion

#region USUARIO 
app.MapControllerRoute(
    name: "usuario-alterar-dados",
    pattern: "usuario/alterardados",
    defaults: new { controller = "Usuario", action = "AlterarDados" }
);

app.MapControllerRoute(
    name: "usuario-dashboard",
    pattern: "usuario/dashboard/{guid}",
    defaults: new { controller = "Usuario", action = "Index" }
);

app.MapControllerRoute(
    name: "usuario-cadastrar",
    pattern: "usuario/cadastrar",
    defaults: new { controller = "Usuario", action = "Cadastro" }
);

app.MapControllerRoute(
    name: "usuario-perfil",
    pattern: "usuario/perfil/{guid}",
    defaults: new { controller = "Usuario", action = "Perfil" }
);

#region VALIDAÇÕES
app.MapControllerRoute(
    name: "verificar-email",
    pattern: "verificaremail",
    defaults: new { controller = "Usuario", action = "VerificarEmailExistente" }
);
#endregion
#endregion

#region LOGIN
app.MapControllerRoute(
    name: "login",
    pattern: "login",
    defaults: new { controller = "Login", action = "Login" }
);

app.MapControllerRoute(
    name: "logout",
    pattern: "logout",
    defaults: new { controller = "Login", action = "Logout" }
);

#endregion

app.MapControllerRoute(
    name: "home",
    pattern: "home",
    defaults: new { controller = "Home", action = "Index" }
);

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}"
);

app.Run();
