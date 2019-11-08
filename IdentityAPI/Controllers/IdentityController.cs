using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using IdentityAPI.Helpers;
using IdentityAPI.Infrastructure;
using IdentityAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;

namespace IdentityAPI.Controllers
{
    [Route("api/Auth")]
    [ApiController]
    public class IdentityController : ControllerBase
    {
        private IdentityDbContext dbContext;
        private IConfiguration config;

        public object JSonConvert { get; private set; }

        public IdentityController(IdentityDbContext db, IConfiguration configuration)
        {
            this.dbContext = db;
            this.config = configuration;
        }
        [HttpPost("register", Name ="RegisterUser")]
        public async Task<ActionResult<dynamic>> Register(User user)
        {
            TryValidateModel(user);
            if (ModelState.IsValid)
            {
                user.Status = "Not Verified";
                await dbContext.Users.AddAsync(user);
                await dbContext.SaveChangesAsync();
                await SendVerificationMail(user);
                return Created("", new
                {
                    user.Id,
                    user.Fullname,
                    user.Email,
                    user.UserName
                });
            }
            else
            {
                return BadRequest(user);
            }
        }

        [HttpPost("token", Name ="GetToken")]
        public ActionResult<dynamic> GetToken(LogInModel model)
        {
            TryValidateModel(model);
            if (ModelState.IsValid)
            {
                var user = dbContext.Users.SingleOrDefault(s => s.UserName == model.username && s.Password == model.password && s.Status=="Verified");
                if (user != null)
                {
                    var token = GenerateToken(user);
                    return Ok(new { user.Fullname, user.Email, user.UserName, user.Role, Token=token});
                }
                else
                {
                    return Unauthorized();
                }
            }

            else
            {
                return BadRequest(model);
            }
        }

        [NonAction]
        private string GenerateToken(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub,user.Fullname),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };
            claims.Add(new Claim(JwtRegisteredClaimNames.Aud, "catalogapi"));
            claims.Add(new Claim(JwtRegisteredClaimNames.Aud, "paymentapi"));
            claims.Add(new Claim(JwtRegisteredClaimNames.Aud, "basketapi"));
            claims.Add(new Claim(JwtRegisteredClaimNames.Aud, "orderapi"));
            claims.Add(new Claim(ClaimTypes.Role, user.Role));
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config.GetValue<string>("Jwt:secret")));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                issuer: config.GetValue<string>("Jwt:issuer"),
                audience: null,
                claims: claims,
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: credentials);
            string tokenString = new JwtSecurityTokenHandler().WriteToken(token);
            return tokenString;
        }

        [NonAction]
        private async Task SendVerificationMail(User user)
        {
            var userObj = new
            {
                user.Id,
                user.Fullname,
                user.Email,
                user.UserName
            };
            var messageText = JsonConvert.SerializeObject(userObj);
            StorageAccountHelper storageAccountHelper = new StorageAccountHelper();
            storageAccountHelper.storageConnectionString = config.GetConnectionString("StorageConnection");
            await storageAccountHelper.SendMessageAsync(messageText, "users");
        }
    }
}