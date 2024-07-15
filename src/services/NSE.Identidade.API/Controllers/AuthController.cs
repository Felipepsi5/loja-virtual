using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using NetDevPack.Security.Jwt.Core.Interfaces;
using NSE.Core.Messages.Integration;
using NSE.Identidade.API.Models;
using NSE.MessageBus;
using NSE.Web.Api.Core.Controllers;
using NSE.Web.Api.Core.Identidade;
using NSE.WebAPI.Core.Usuario;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using JwtRegisteredClaimNames = Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames;

namespace NSE.Identidade.API.Controllers
{
	[Route("api/identidade")]
	public class AuthController : MainController
	{
		private readonly SignInManager<IdentityUser> _signInManager;
		private readonly UserManager<IdentityUser> _userManager;
		private readonly AppSettings _appSettings;
		private readonly IMessageBus _bus;
		private readonly IAspNetUser _aspNetUser;
		private readonly IJwtService _jwksService;

		public AuthController(SignInManager<IdentityUser> signInManager,
                              UserManager<IdentityUser> userManager,
                              IOptions<AppSettings> appSettings,
                              IMessageBus bus,
							  IAspNetUser aspNetUser,
                              IJwtService jwksService)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _appSettings = appSettings.Value;
			_bus = bus;
			_aspNetUser = aspNetUser;
			_jwksService = jwksService;
        }

        [HttpPost("nova-conta")]
		public async Task<ActionResult> Registrar(UsuarioRegistro usuarioRegistro)
		{
			if (!ModelState.IsValid) return CustomResponse(ModelState);

			var user = new IdentityUser
			{
				UserName = usuarioRegistro.Email,
				Email = usuarioRegistro.Email,
				EmailConfirmed = true
			};

			var result = await _userManager.CreateAsync(user, usuarioRegistro.Senha);

			if (result.Succeeded)
			{
			   var clientResult = await RegistrarCliente(usuarioRegistro);

				if (!clientResult.ValidationResult.IsValid)
				{
					await _userManager.DeleteAsync(user);
					return CustomResponse(clientResult.ValidationResult);	
				}

				return CustomResponse(await GerarJwt(usuarioRegistro.Email));
			}

			foreach (var error in result.Errors)
			{
				AdicionarErroProcessamento(error.Description);
			}

			return CustomResponse();            
		}



		[HttpPost("autenticar")]
		public async Task<ActionResult> Login (UsuarioLogin usuarioLogin)
		{
			if (!ModelState.IsValid) return CustomResponse(ModelState);

			var result = await _signInManager.PasswordSignInAsync(usuarioLogin.Email, usuarioLogin.Senha, false, true);

			if (result.Succeeded)
			{
				return CustomResponse(await GerarJwt(usuarioLogin.Email));
			}

			if (result.IsLockedOut)
			{
				AdicionarErroProcessamento("Usuário temporariamente bloqueado por tentativas inválidas.");
				return CustomResponse();
			}

			AdicionarErroProcessamento("Usuário ou Senha incorretos");
			return CustomResponse();
		}

		private async Task<UsuarioRespostaLogin> GerarJwt(string email)
		{
			var user = await _userManager.FindByEmailAsync(email);
			var claims = await _userManager.GetClaimsAsync(user);

			var identityClaims = ObterClaimsUsuario(user, claims);
			var encodedToken = await CodificarToken(identityClaims);

			return ObterRespostaToken(encodedToken, user, claims);
		}

		private UsuarioRespostaLogin ObterRespostaToken(string encodedToken, IdentityUser user, IList<Claim> claims)
		{
			return new UsuarioRespostaLogin
			{
				AccessToken = encodedToken,
				ExpiresIn = TimeSpan.FromHours(1).TotalSeconds,
				UsuarioToken = new UsuarioToken
				{
					Id = user.Id,
					Email = user.Email,
					Claims = claims.Select(c => new UsuarioClaim { Type = c.Type, Value = c.Value })
				}
			};
		}

		private ClaimsIdentity ObterClaimsUsuario(IdentityUser user, IList<Claim> claims)
		{
			var userRoles = _userManager.GetRolesAsync(user).Result;

			claims.Add(new Claim(JwtRegisteredClaimNames.Sub, user.Id));
			claims.Add(new Claim(JwtRegisteredClaimNames.Email, user.Email));
			claims.Add(new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()));
			claims.Add(new Claim(JwtRegisteredClaimNames.Nbf, ToUnixEpochDate(DateTime.UtcNow).ToString()));
			claims.Add(new Claim(JwtRegisteredClaimNames.Iat, ToUnixEpochDate(DateTime.UtcNow).ToString(), ClaimValueTypes.Integer64));

			foreach (var userRole in userRoles)
			{
				claims.Add(new Claim("role", userRole));
			}

			var identityClaims = new ClaimsIdentity();
			identityClaims.AddClaims(claims);

			return identityClaims;
		}

		private async Task<string> CodificarToken(ClaimsIdentity identityClaims)
		{
			var tokenHandler = new JwtSecurityTokenHandler();

			var currentIssuer = $"{_aspNetUser.ObterHttpContext().Request.Scheme}://{_aspNetUser.ObterHttpContext().Request.Host}";

			var key = await _jwksService.GetCurrentSigningCredentials();

			var token = tokenHandler.CreateToken(new SecurityTokenDescriptor
			{
				Issuer = currentIssuer,
				Subject = identityClaims,
				Expires = DateTime.UtcNow.AddHours(1),
				SigningCredentials = key
            });

			return tokenHandler.WriteToken(token);
		}
		private static long ToUnixEpochDate(DateTime date)
			=> (long)Math.Round((date.ToUniversalTime() - new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero)).TotalSeconds);

        private async Task<ResponseMessage> RegistrarCliente(UsuarioRegistro usuarioRegistro)
        {
            var usuario = await _userManager.FindByEmailAsync(usuarioRegistro.Email);

            var usuarioRegistrado = new UsuarioRegistradoIntegrationEvent(
                Guid.Parse(usuario.Id), usuarioRegistro.Nome, usuarioRegistro.Email, usuarioRegistro.Cpf);

			try
			{
                return await _bus.RequestAsync<UsuarioRegistradoIntegrationEvent, ResponseMessage>(usuarioRegistrado);
            }
            catch (Exception ex)
			{

				await _userManager.DeleteAsync(usuario);
				throw ex;
			}
        }
    }
}
