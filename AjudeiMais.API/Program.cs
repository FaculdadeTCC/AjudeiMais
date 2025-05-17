using AjudeiMais.API.Controllers;
using AjudeiMais.API.Repositories;
using AjudeiMais.API.Services;
using AjudeiMais.Data.Context;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Registra o ApplicationDbContext com a string de conexão
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Registra os outros serviços
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
builder.Services.AddScoped<EnderecoService>();
builder.Services.AddScoped<EnderecoRepository>();



// Adiciona os serviços ao container.
builder.Services.AddControllers();

// Adiciona o Swagger para documentação da API.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configura o pipeline de requisição HTTP.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapControllers();  // Mapeia os controllers, incluindo o UsuarioController.

app.Run();
