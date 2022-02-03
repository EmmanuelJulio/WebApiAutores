using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WebApiAutores.Dtos;
using WebApiAutores.Servicios;

namespace WebApiAutores.Controllers.V1
{
    [ApiController]
    [Route("api/V1/cuentas")]
    //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class CuentasController : ControllerBase
    {
        private readonly UserManager<IdentityUser> userManager;
        private readonly IConfiguration configuration;
        private readonly SignInManager<IdentityUser> signInManager;
        private readonly IDataProtectionProvider dataProtectionProvider;
        private readonly HashService hashService;
        private readonly IDataProtector dataProtector;
        public CuentasController(UserManager<IdentityUser> userManager,
            IConfiguration configuration,
            SignInManager<IdentityUser> signInManager
            , IDataProtectionProvider dataProtectionProvider,
            HashService hashService)
        {
            this.userManager = userManager;
            this.configuration = configuration;
            this.signInManager = signInManager;
            this.dataProtectionProvider = dataProtectionProvider;
            this.hashService = hashService;
            dataProtector = dataProtectionProvider.CreateProtector("Valor_unico_Secreto_que_Sea_Largo");
        }
        //[HttpGet("hash/{textoPlano}")]
        //public ActionResult RealizarHash(string textoPlano)
        //{
        //    var resultado1= hashService.Hash(textoPlano);
        //    var resultado2= hashService.Hash(textoPlano);
        //    return Ok(new
        //    {
        //        textoPlano = textoPlano,
        //        hash1 = resultado1,
        //        hash2 = resultado2
        //    });

        //}
        //[HttpGet("Encriptar")]
        //public ActionResult Encriptar()
        //{
        //    var textoPlano = "Emmanuel julio";
        //    var textoEncriptado = dataProtector.Protect(textoPlano);
        //    var textoDesencriptado = dataProtector.Unprotect(textoEncriptado);
        //    return Ok(new
        //    {
        //        textoPlano = textoPlano,
        //        textoEncriptado = textoEncriptado,
        //        textoDesencriptado = textoDesencriptado
        //    });
        //}
        //[HttpGet("EncriptarPorTiempo")]
        //public ActionResult EncriptarPorTiempo()
        //{
        //    var protectorLimitadoPorTiempo = dataProtector.ToTimeLimitedDataProtector();
        //    var textoPlano = "Emmanuel julio";
        //    var textoEncriptado = protectorLimitadoPorTiempo.Protect(textoPlano,lifetime:TimeSpan.FromSeconds(5));
        //    Thread.Sleep(6000);
        //    var textoDesencriptado = protectorLimitadoPorTiempo.Unprotect(textoEncriptado);
        //    return Ok(new
        //    {
        //        textoPlano = textoPlano,
        //        textoEncriptado = textoEncriptado,
        //        textoDesencriptado = textoDesencriptado
        //    });
        //}


        [HttpPost("registrar",Name ="registrarUsuario")]
        public async Task<ActionResult<RespuestaAutentificacion>> CuentasControl(CredencialesUsuario credencialesUsuario)
        {
            var usuario = new IdentityUser
            {
                UserName = credencialesUsuario.Email,
                Email = credencialesUsuario.Email
            };
            var resultado = await userManager.CreateAsync(usuario, credencialesUsuario.Password);
            if (resultado.Succeeded)
            {
                return await ConstruirToken(credencialesUsuario);
            }
            else
            {
                return BadRequest(resultado.Errors);
            };
        }
        
        [HttpPost("loggin",Name ="logginUsuario")]
        public async Task<ActionResult<RespuestaAutentificacion>> Loggin(CredencialesUsuario credencialesUsuario)
        {
            var resultado = await signInManager.PasswordSignInAsync(credencialesUsuario.Email
                , credencialesUsuario.Password, isPersistent: false, lockoutOnFailure: false);

            if (resultado.Succeeded)
            {
                return await ConstruirToken(credencialesUsuario);
            }
            else
            {
                return BadRequest("Loggin incorrecto papu");
            }

        }
        [HttpGet("RenovarToken",Name ="renovarToken")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult<RespuestaAutentificacion>> Renovar()
        {
            var emailClaim = HttpContext.User.Claims.Where(claim => claim.Type == "email").FirstOrDefault();
            var email = emailClaim.Value;
            var credencialesUsuario = new CredencialesUsuario()
            {
                Email = email

            };
            return await ConstruirToken(credencialesUsuario);
        }
        private async Task<RespuestaAutentificacion> ConstruirToken(CredencialesUsuario credencialesUsuario)
        {
            var claims = new List<Claim>()
            {
                new Claim("email",credencialesUsuario.Email),
                new Claim("Otra cosa mas ","Soy un claim")
            };
            var usuario = await userManager.FindByEmailAsync(credencialesUsuario.Email);
            var claimsDB= await userManager.GetClaimsAsync(usuario);
            claims.AddRange(claimsDB);
            var llave = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["LlaveJwt"]));
            var creds= new SigningCredentials(llave, SecurityAlgorithms.HmacSha256);
            var expiracion = DateTime.UtcNow.AddYears(1);
            var securityToken = new JwtSecurityToken(issuer: null, audience: null,
                claims: claims, expires: expiracion, signingCredentials: creds);
            return new RespuestaAutentificacion()
            {
                Token = new JwtSecurityTokenHandler().WriteToken(securityToken),
                Expiracion = expiracion
            };
        }
        [HttpPost("HacerAdmin",Name ="hacerAdmin")]
        public async Task<ActionResult> HacerADmin(EditarAdminDTO editarAdminDTO)
        {
            var usuario = await userManager.FindByEmailAsync(editarAdminDTO.Email);
            await userManager.AddClaimAsync(usuario, new Claim("EsAdmin", "true"));
            return NoContent();
        }
        [HttpPost("RemoverAdmin",Name ="removerAdmin")]
        public async Task<ActionResult> RemoveADmin(EditarAdminDTO editarAdminDTO)
        {
            var usuario = await userManager.FindByEmailAsync(editarAdminDTO.Email);
            await userManager.RemoveClaimAsync(usuario, new Claim("EsAdmin", "true"));
            return NoContent();
        }
    }
}
