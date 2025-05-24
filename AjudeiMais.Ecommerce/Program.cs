using AjudeiMais.Ecommerce.Controllers;

var builder = WebApplication.CreateBuilder(args);

/// <summary>
/// Adiciona os servi�os ao cont�iner da aplica��o.
/// </summary>
builder.Services.AddControllersWithViews();

/// <summary>
/// Configura o cliente HTTP para se comunicar com a API do sistema.
/// </summary>
builder.Services.AddHttpClient("ApiAjudeiMais", client =>
{
    client.BaseAddress = new Uri("http://localhost:5168/");
});

/// <summary>
/// Adiciona suporte � sess�o para armazenar dados do usu�rio entre requisi��es.
/// </summary>
builder.Services.AddSession();

/// <summary>
/// Configura autentica��o baseada em cookies.
/// </summary>
builder.Services.AddAuthentication("Cookies")
    .AddCookie("Cookies", options =>
    {
        options.LoginPath = "/login"; // Redireciona usu�rios n�o autenticados para a p�gina de login
        options.AccessDeniedPath = "/acesso-negado"; // Redireciona usu�rios sem permiss�o para esta p�gina
    });

var app = builder.Build();

/// <summary>
/// Configura o pipeline de requisi��o HTTP.
/// </summary>
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts(); // Ativa o uso de HSTS (HTTPS estrito) em ambientes de produ��o
}

/// <summary>
/// Middleware para gerenciar sess�es.
/// </summary>
app.UseSession();

/// <summary>
/// Redireciona automaticamente requisi��es HTTP para HTTPS.
/// </summary>
app.UseHttpsRedirection();

/// <summary>
/// Habilita o uso de arquivos est�ticos (CSS, JS, imagens, etc).
/// </summary>
app.UseStaticFiles();

/// <summary>
/// Define o roteamento da aplica��o.
/// </summary>
app.UseRouting();

/// <summary>
/// Middleware respons�vel por autenticar o usu�rio.
/// IMPORTANTE: deve vir antes do UseAuthorization.
/// </summary>
app.UseAuthentication();

/// <summary>
/// Middleware respons�vel por autorizar o acesso baseado em roles ou pol�ticas.
/// </summary>
app.UseAuthorization();

#region USUARIO

app.MapControllerRoute(
    name: "usuario-cadastrar",
    pattern: "usuario/cadastrar",
    defaults: new { controller = "Usuario", action = "Cadastro" });

app.MapControllerRoute(
    name: "usuario-alterar-dados",
    pattern: "usuario/alterardados",
    defaults: new { controller = "Usuario", action = "AlterarDados" });

app.MapControllerRoute(
    name: "usuario-perfil",
    pattern: "usuario/perfil/{guid}",
    defaults: new { controller = "Usuario", action = "Perfil" });

#region USUARIO VALIDA��ES

app.MapControllerRoute(
    name: "verificar-email",
    pattern: "verificaremail",
    defaults: new { controller = "Usuario", action = "VerificarEmailExistente" });

#endregion

#region LOGIN

app.MapControllerRoute(
    name: "login",
    pattern: "login",
    defaults: new { controller = "Login", action = "Login" });

app.MapControllerRoute(
    name: "logout",
    pattern: "logout",
    defaults: new { controller = "Login", action = "Logout" });

#endregion

#endregion
app.MapControllerRoute(
    name: "home",
    pattern: "home",
    defaults: new { controller = "Home", action = "Index" });
/// <summary>
/// Rota padr�o da aplica��o. Redireciona para Home/Index caso nenhum caminho espec�fico seja encontrado.
/// </summary>
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

/// <summary>
/// Inicia a aplica��o.
/// </summary>
app.Run();
