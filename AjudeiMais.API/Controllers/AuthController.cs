using AjudeiMais.API.DTOs;
using AjudeiMais.API.Services;
using AjudeiMais.Data.Models.UsuarioModel;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly UsuarioService _usuarioService;
    private readonly IConfiguration _config;

    public AuthController(UsuarioService usuarioService, IConfiguration config)
    {
        _usuarioService = usuarioService;
        _config = config;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDTO model)
    {
        var usuario = await _usuarioService.Login(model.Email, model.Senha);
        if (usuario == null)
        {
            return Unauthorized("Email ou senha inválidos.");
        }

        var token = GenerateJwtToken(usuario);
        return Ok(new
        {
            token,
            role = usuario.Role,
            id = usuario.Usuario_ID.ToString()
        });
    }

    private string GenerateJwtToken(Usuario usuario)
    {
        var jwtSettings = _config.GetSection("JwtSettings");
        var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["SecretKey"]));
        var credentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, usuario.Usuario_ID.ToString()),
            new Claim(ClaimTypes.Name, usuario.NomeCompleto),
            new Claim(ClaimTypes.Email, usuario.Email),
            new Claim(ClaimTypes.Role, usuario.Role),
            new Claim("guid", usuario.GUID ?? "")
        };

        var token = new JwtSecurityToken(
            issuer: jwtSettings["Issuer"],
            audience: jwtSettings["Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(Convert.ToDouble(jwtSettings["ExpirationMinutes"])),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
