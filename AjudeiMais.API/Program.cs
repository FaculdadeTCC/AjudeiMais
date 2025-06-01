using AjudeiMais.API.Controllers;
using AjudeiMais.API.Repositories;
using AjudeiMais.API.Services;
using AjudeiMais.Data.Context;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Registra o ApplicationDbContext com a string de conexão
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("ConnectionDev-Danilo")));

// Registra os outros serviços no container de dependências (DI)
builder.Services.AddScoped<UsuarioService>();
builder.Services.AddScoped<UsuarioRepository>();
builder.Services.AddScoped<ProdutoService>();
builder.Services.AddScoped<ProdutoRepository>();
builder.Services.AddScoped<ProdutoImagemService>();
builder.Services.AddScoped<ProdutoImagemRepository>();
builder.Services.AddScoped<CategoriaProdutoService>();
builder.Services.AddScoped<CategoriaProdutoRepository>();
builder.Services.AddScoped<InstituicaoService>();
builder.Services.AddScoped<InstituicaoRepository>();
builder.Services.AddScoped<EnderecoService>();
builder.Services.AddScoped<EnderecoRepository>();
//builder.Services.AddScoped<InstituicaoCategoriaService>();
//builder.Services.AddScoped<InstituicaoCategoriaRepository>();
builder.Services.AddScoped<InstituicaoImagemService>();
builder.Services.AddScoped<InstituicaoImagemRepository>();
builder.Services.AddScoped<CategoriaInstituicaoService>();
builder.Services.AddScoped<CategoriaInstituicaoRepository>();
//builder.Services.AddScoped<ChatRepository>();
//builder.Services.AddScoped<ChatService>();
//builder.Services.AddScoped<MensagemChatRepository>();
//builder.Services.AddScoped<MensagemChatService>();
builder.Services.AddScoped<NominatimService>();

builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 50 * 1024 * 1024; // 50 MB
});

builder.Services.AddHttpClient();

// Adiciona autenticação JWT
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var key = Encoding.ASCII.GetBytes(jwtSettings["SecretKey"]);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(key)
    };

    // Eventos para personalizar resposta de erros 401 e 403
    options.Events = new JwtBearerEvents
    {
        OnChallenge = context =>
        {
            // Este evento é chamado quando a autenticação falha (token inválido ou ausente)
            context.HandleResponse(); // evita a resposta padrão do middleware
            context.Response.StatusCode = 401;
            context.Response.ContentType = "application/json";
            var result = System.Text.Json.JsonSerializer.Serialize(new { message = "Autenticação necessária: token inválido ou ausente." });
            return context.Response.WriteAsync(result);
        },
        OnForbidden = context =>
        {
            // Este evento é chamado quando o usuário está autenticado mas não autorizado (ex: role errada)
            context.Response.StatusCode = 403;
            context.Response.ContentType = "application/json";
            var result = System.Text.Json.JsonSerializer.Serialize(new { message = "Acesso negado: você não tem permissão para este recurso." });
            return context.Response.WriteAsync(result);
        }
    };
});

builder.Services.AddAuthorization(); // Habilita autorização por Role

// Configuração de CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        builder => builder.AllowAnyOrigin()  // Permite qualquer origem
                         .AllowAnyMethod()  // Permite qualquer método HTTP (GET, POST, PUT, DELETE)
                         .AllowAnyHeader()); // Permite qualquer cabeçalho
});

// Adiciona os serviços ao container (Controllers)
builder.Services.AddControllers();

// Adiciona o Swagger para documentação da API
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "AjudeiMais API", Version = "v1" });

    // Configura o suporte ao token JWT no Swagger
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Insira o token JWT assim: **Bearer seu_token_aqui**"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            new string[] {}
        }
    });
});

var app = builder.Build();

// Configuração do pipeline de requisição HTTP
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();  // Habilita o Swagger no ambiente de desenvolvimento
    app.UseSwaggerUI(); // Interface do Swagger
}

app.UseAuthentication();
app.UseAuthorization();

// Habilita o CORS antes do middleware de roteamento
app.UseCors("AllowAll");

app.UseStaticFiles();

// Redirecionamento de HTTPS
app.UseHttpsRedirection();

// Mapeia os controllers (inclusive o UsuarioController)
app.MapControllers();

app.Run();  // Inicia a aplicação
