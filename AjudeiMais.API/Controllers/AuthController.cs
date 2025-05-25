using AjudeiMais.API.DTOs;
using AjudeiMais.API.Services;
using AjudeiMais.Data.Models.InstituicaoModel;
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
    private readonly InstituicaoService _instituicaoService;
    private readonly IConfiguration _config;

    public AuthController(UsuarioService usuarioService, InstituicaoService instituicaoService ,IConfiguration config)
    {
        _usuarioService = usuarioService;
        _instituicaoService = instituicaoService;
        _config = config;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDTO model)
    {
        // Tenta logar como usuário
        var usuario = await _usuarioService.Login(model.Email, model.Senha);
        if (usuario != null)
        {
            var token = GenerateJwtToken(usuario);
            return Ok(new
            {
                token,
                role = usuario.Role,
                id = usuario.Usuario_ID.ToString(),
                GUID = usuario.GUID
            });
        }

        // Tenta logar como instituição
        var instituicao = await _instituicaoService.Login(model.Email, model.Senha);
        if (instituicao != null)
        {
            var token = GenerateJwtToken(instituicao);
            return Ok(new
            {
                token,
                role = instituicao.Role,
                id = instituicao.Instituicao_ID.ToString(),
            });
        }

        return Unauthorized("Email ou senha inválidos.");
    }

    private string GenerateJwtToken(Usuario usuario)
    {
        return GenerateJwt(usuario.Usuario_ID.ToString(), usuario.NomeCompleto, usuario.Email, usuario.Role, usuario.GUID);
    }

    private string GenerateJwtToken(Instituicao instituicao)
    {
        return GenerateJwt(instituicao.Instituicao_ID.ToString(), instituicao.Nome, instituicao.Email, instituicao.Role, instituicao.GUID);
    }

    private string GenerateJwt(string id, string nome, string email, string role, string guid)
    {
        var jwtSettings = _config.GetSection("JwtSettings");
        var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["SecretKey"]));
        var credentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, id),
            new Claim(ClaimTypes.Name, nome),
            new Claim(ClaimTypes.Email, email),
            new Claim(ClaimTypes.Role, role),
            new Claim("guid", guid ?? "")
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
