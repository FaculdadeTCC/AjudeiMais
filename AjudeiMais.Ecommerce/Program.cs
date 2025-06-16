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

builder.Services.AddHttpContextAccessor();

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


#region INSTITUIÇÃO
// Rotas de instituição 

app.MapControllerRoute(
    name: "instituicao-cadastrar",
    pattern: "instituicao/cadastrar",
    defaults: new { controller = "Instituicao", action = "Cadastro" }
);

app.MapControllerRoute(
    name: "instituicao-perfil",
    pattern: "instituicao/perfil/{guid}",
    defaults: new { controller = "Instituicao", action = "Perfil" }
);

app.MapControllerRoute(
    name: "instituicao-alterar-dados",
    pattern: "instituicao/alterar-dados",
    defaults: new { controller = "Instituicao", action = "AlterarDados" }
);

app.MapControllerRoute(
    name: "instituicao-dashboard",
    pattern: "instituicao/dashboard/{guid}",
    defaults: new { controller = "Instituicao", action = "Index" }
);

#endregion

#region ADMIN
#region DASHBOARD
app.MapControllerRoute(
    name: "admin-dashboard",
    pattern: "admin/dashboard",
    defaults: new { controller = "Admin", action = "Index" }
);
#endregion

#region CATEGORIA DE PRODUTO
app.MapControllerRoute(
    name: "admin-categorias-produto",
    pattern: "admin/categorias-do-produto",
    defaults: new { controller = "CategoriaProduto", action = "Index" }
);

app.MapControllerRoute(
    name: "admin-categoria-produto-cadastrar",
    pattern: "admin/categoria-do-produto/cadastrar",
    defaults: new { controller = "CategoriaProduto", action = "Cadastro" }
);

app.MapControllerRoute(
    name: "admin-categoria-produto-excluir",
    pattern: "admin/categoria-do-produto/excluir",
    defaults: new { controller = "CategoriaProduto", action = "Excluir" }
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

#region   PRODUTO
app.MapControllerRoute(
    name: "usuario-anuncio-cadastrar",
    pattern: "usuario/{guid}/anuncio",
    defaults: new { controller = "Produto", action = "Cadastro" }
);

app.MapControllerRoute(
    name: "anuncio-excluir-imagem",
    pattern: "usuario/{guid}/anuncio",
    defaults: new { controller = "Produto", action = "ExcluirImagem" }
);

app.MapControllerRoute(
    name: "anuncio-imagem-editar",
    pattern: "anuncio/{guid}/imagens",
    defaults: new { controller = "Produto", action = "Imagens" }
);

app.MapControllerRoute(
    name: "anuncio-detalhe",
    pattern: "anuncio/{guid}",
    defaults: new { controller = "Produto", action = "Detalhe" }
);

app.MapControllerRoute(
    name: "anuncios",
    pattern: "meus-anuncios/{guid}",
    defaults: new { controller = "Produto", action = "Index" }
);

app.MapControllerRoute(
    name: "anuncio-excluir",
    pattern: "anuncio/excluir",
    defaults: new { controller = "Produto", action = "Excluir" }
);

app.MapControllerRoute(
    name: "anuncio-editar",
    pattern: "anuncio/editar/{guid}",
    defaults: new { controller = "Produto", action = "Editar" }
);


#endregion

#region CATEGORIA INSTITUIÇÃO
// Rotas de categoria de instituição

app.MapControllerRoute(
	name: "categoria-instituicao-index",
	pattern: "categoria-instituicao",
	defaults: new { controller = "CategoriaInstituicao", action = "Index" }
);

app.MapControllerRoute(
	name: "categoria-instituicao-cadastrar",
	pattern: "categoria-instituicao/cadastrar",
	defaults: new { controller = "CategoriaInstituicao", action = "Adicionar" }
);

app.MapControllerRoute(
	name: "categoria-instituicao-editar",
	pattern: "categoria-instituicao/editar/{id}",
	defaults: new { controller = "CategoriaInstituicao", action = "AlterarDados" }
);

app.MapControllerRoute(
	name: "categoria-instituicao-excluir",
	pattern: "categoria-instituicao/excluir/{id}",
	defaults: new { controller = "CategoriaInstituicao", action = "Excluir" }
);
#endregion

#region PEDIDO

app.MapControllerRoute(
    name: "pedido-dashboard",
    pattern: "pedido/dashboard",
    defaults: new { controller = "Pedido", action = "Index" }
);

app.MapControllerRoute(
    name: "pedido-criar",
    pattern: "pedido/criar",
    defaults: new { controller = "Pedido", action = "CriarPedido" }
);

app.MapControllerRoute(
    name: "pedido-detalhe",
    pattern: "pedido/detalhe",
    defaults: new { controller = "Pedido", action = "Detalhe" }
);

app.MapControllerRoute(
    name: "meus-pedidos",
    pattern: "pedido/meus-pedidos/{guid}",
    defaults: new { controller = "Pedido", action = "PedidosPorInstituicao" }
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
