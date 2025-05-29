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
//
// Rotas específicas de "usuario"
//
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

//
// Rotas de validações de usuário
//
app.MapControllerRoute(
    name: "verificar-email",
    pattern: "verificaremail",
    defaults: new { controller = "Usuario", action = "VerificarEmailExistente" }
);


// Rotas de instituição 

app.MapControllerRoute(
    name: "instituicao-cadastrar",
    pattern: "instituicao/cadastrar",
    defaults: new { controller = "Instituicao", action = "Cadastro" }
);

app.MapControllerRoute(
    name: "instituicao-perfil",
    pattern: "instituicao/perfil/{guid}",
    defaults: new { controller = "Instituicao", action = "Perfil"}
);

app.MapControllerRoute(
    name: "instituicao-alterar-dados",
    pattern: "instituicao/alterar-dados",
    defaults: new { controller = "Instituicao", action = "AlterarDados" }
);

app.MapControllerRoute(
    name: "instituicao-index",
    pattern: "instituicao",
    defaults: new { controller = "Instituicao", action = "Index" }
);


//
// Rotas de Login/Logout
//
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


app.MapControllerRoute(
    name: "home",
    pattern: "home",
    defaults: new { controller = "Home", action = "Index" }
);
//
// Rodas de categoria 
//

app.MapControllerRoute(
    name: "categoria-index",
    pattern: "admin/categorias",
    defaults: new { controller = "Categoria", action = "Index" }
);

app.MapControllerRoute(
    name: "categoria-adicionar",
    pattern: "admin/categorias/adicionar",
    defaults: new { controller = "Categoria", action = "Adicionar" }
);

app.MapControllerRoute(
    name: "categoria-alterar",
    pattern: "admin/categorias/alterar",
    defaults: new { controller = "Categoria", action = "AlterarDados" }
);

//
// Rota padrão
//
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}"
);

app.Run();
