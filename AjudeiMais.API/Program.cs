using AjudeiMais.API.Repositories;
using AjudeiMais.API.Services;
using AjudeiMais.Data.Context;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Registra o ApplicationDbContext com a string de conex�o
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Registra os outros servi�os
builder.Services.AddScoped<UsuarioService>();  // Servi�o de usu�rios
builder.Services.AddScoped<UsuarioRepository>();  // Reposit�rio de usu�rios

// Adiciona os servi�os ao container.
builder.Services.AddControllers();

// Adiciona o Swagger para documenta��o da API.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configura o pipeline de requisi��o HTTP.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapControllers();  // Mapeia os controllers, incluindo o UsuarioController.

app.Run();
