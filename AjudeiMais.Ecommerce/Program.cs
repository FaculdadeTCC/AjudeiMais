using AjudeiMais.Ecommerce.Controllers;

var builder = WebApplication.CreateBuilder(args);

/// <summary>
/// Adiciona os serviços ao contêiner da aplicação.
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
/// Adiciona suporte à sessão para armazenar dados do usuário entre requisições.
/// </summary>
builder.Services.AddSession();

/// <summary>
/// Configura autenticação baseada em cookies.
/// </summary>
builder.Services.AddAuthentication("Cookies")
    .AddCookie("Cookies", options =>
    {
        options.LoginPath = "/login"; // Redireciona usuários não autenticados para a página de login
        options.AccessDeniedPath = "/acesso-negado"; // Redireciona usuários sem permissão para esta página
    });

var app = builder.Build();

/// <summary>
/// Configura o pipeline de requisição HTTP.
/// </summary>
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts(); // Ativa o uso de HSTS (HTTPS estrito) em ambientes de produção
}

/// <summary>
/// Middleware para gerenciar sessões.
/// </summary>
app.UseSession();

/// <summary>
/// Redireciona automaticamente requisições HTTP para HTTPS.
/// </summary>
app.UseHttpsRedirection();

/// <summary>
/// Habilita o uso de arquivos estáticos (CSS, JS, imagens, etc).
/// </summary>
app.UseStaticFiles();

/// <summary>
/// Define o roteamento da aplicação.
/// </summary>
app.UseRouting();

/// <summary>
/// Middleware responsável por autenticar o usuário.
/// IMPORTANTE: deve vir antes do UseAuthorization.
/// </summary>
app.UseAuthentication();

/// <summary>
/// Middleware responsável por autorizar o acesso baseado em roles ou políticas.
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

#region USUARIO VALIDAÇÕES

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
/// Rota padrão da aplicação. Redireciona para Home/Index caso nenhum caminho específico seja encontrado.
/// </summary>
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

/// <summary>
/// Inicia a aplicação.
/// </summary>
app.Run();
