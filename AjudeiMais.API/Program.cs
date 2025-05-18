using AjudeiMais.API.Controllers;
using AjudeiMais.API.Repositories;
using AjudeiMais.API.Services;
using AjudeiMais.Data.Context;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Registra o ApplicationDbContext com a string de conexão
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

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
builder.Services.AddScoped<InstituicaoCategoriaService>();
builder.Services.AddScoped<InstituicaoCategoriaRepository>();
builder.Services.AddScoped<InstituicaoImagemService>();
builder.Services.AddScoped<InstituicaoImagemRepository>();
builder.Services.AddScoped<CategoriaService>();
builder.Services.AddScoped<CategoriaRepository>();
builder.Services.AddScoped<ChatRepository>();
builder.Services.AddScoped<ChatService>();
builder.Services.AddScoped<MensagemChatRepository>();
builder.Services.AddScoped<MensagemChatService>();
builder.Services.AddScoped<NominatimService>();

builder.Services.AddHttpClient();

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
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configuração do pipeline de requisição HTTP
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();  // Habilita o Swagger no ambiente de desenvolvimento
    app.UseSwaggerUI();  // Interface do Swagger
}

// Habilita o CORS antes do middleware de roteamento
app.UseCors("AllowAll");

app.UseStaticFiles();

// Redirecionamento de HTTPS
app.UseHttpsRedirection();

// Mapeia os controllers (inclusive o UsuarioController)
app.MapControllers();

app.Run();  // Inicia a aplicação
