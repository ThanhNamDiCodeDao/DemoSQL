using DemoSQL.Data;
using DemoSQL.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace DemoSQL.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly MyDbContext _context;
        private readonly AppSetting _appSettings;

        public UserController(MyDbContext context, IOptionsMonitor<AppSetting> optionsMonitor)
        {
            _context = context;
            _appSettings = optionsMonitor.CurrentValue;
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Validate(LoginModel model)
        {
            var user = _context.NguoiDungs.SingleOrDefault(x => x.Username == model.Username && x.Password == model.Password);
            if(user == null)
            {
                return Ok(new ApiRespond
                {
                    Success = false,
                    Message = "invalid username or password"
                });
            }

            // cap token
            var token = await GenerateToken(user);
    
            return Ok(new ApiRespond
            {
                Success = true,
                Message = "authenticate successfully",
                Data = token
            });
        }
        private async Task<TokenModel> GenerateToken(NguoiDung nguoiDung)
        {
            var jwtTokenHandler = new JwtSecurityTokenHandler();

            var secretKeyByte = Encoding.UTF8.GetBytes(_appSettings.SecretKey);

            var tokenDescription = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Name, nguoiDung.HovaTen),
                    new Claim(JwtRegisteredClaimNames.Email, nguoiDung.Email),
                    new Claim(JwtRegisteredClaimNames.Sub, nguoiDung.Email),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim("Username", nguoiDung.Username),
                    new Claim("Id", nguoiDung.Id.ToString()),

                    //roles
                }),
                Expires = DateTime.UtcNow.AddMinutes(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(secretKeyByte), SecurityAlgorithms.HmacSha512Signature)
                
            };

            var token = jwtTokenHandler.CreateToken(tokenDescription);

            var accessToken =  jwtTokenHandler.WriteToken(token);

            var refreshToken = GenerateRefreshToken();

            //Luu database
            var refreshTokenEnity = new RefreshToken
            {
                Id = Guid.NewGuid(),
                JwtId = token.Id,
                UserId = nguoiDung.Id,
                Token = refreshToken,
                IsUsed = false,
                IsRevoked = false,
                IssuedAt = DateTime.UtcNow,
                ExpiredAt = DateTime.UtcNow.AddHours(1),
            };

            await _context.AddAsync(refreshTokenEnity);
            await _context.SaveChangesAsync();

            return new TokenModel
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken
            };

        }

        private string GenerateRefreshToken()
        {
            var random = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(random);
            }
            return Convert.ToBase64String(random);
        }

        [HttpPost("RenewToken")]
        public async Task<IActionResult> RenewToken(TokenModel tokenModel)
        {
            var jwtTokenHandler = new JwtSecurityTokenHandler();

            var secretKeyBytes = Encoding.UTF8.GetBytes(_appSettings.SecretKey);
            
            var tokenValidateParam = new TokenValidationParameters
            {
                ValidateIssuer = false,
                ValidateAudience = false,

                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(secretKeyBytes),

                ClockSkew = TimeSpan.Zero,

                ValidateLifetime = false,
            };

            try
            {
                //check 1: AccessToken valid format
                var tokenInVerfication = jwtTokenHandler.ValidateToken(tokenModel.AccessToken, tokenValidateParam, out var validatedToken);

                //check 2: Check algorithms
                if(validatedToken is JwtSecurityToken jwtSecurityToken)
                {
                    var result = jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha512, StringComparison.InvariantCultureIgnoreCase);
                    if(!result)//false
                    {
                        return Ok(new ApiRespond
                        {
                            Success = false,
                            Message = "invalide token",
                        });
                    }
                }

                //check 3: Check Token expire?
                var utcExpiredDate = long.Parse(tokenInVerfication.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Exp).Value);

                var expireDate = ConvertUnixTimeToDateTime(utcExpiredDate);

                if(expireDate > DateTime.UtcNow)
                {
                    return Ok(new ApiRespond
                    {
                        Success = false,
                        Message = "AccessToken has not yet expired",
                    });
                }

                //check 4: Check Refreshtoken exist in DB
                var storedToken = _context.RefreshTokens.FirstOrDefault(x => x.Token == tokenModel.RefreshToken);
                if(storedToken == null)
                {
                    return Ok(new ApiRespond
                    {
                        Success = false,
                        Message = "RefreshToken does not exist",
                    });
                }

                //check 5: check Refreshtoken is used/revoked
                if(storedToken.IsUsed)
                {
                    return Ok(new ApiRespond
                    {
                        Success = false,
                        Message = "RefreshToken has been used",
                    });
                }

                if (storedToken.IsRevoked)
                {
                    return Ok(new ApiRespond
                    {
                        Success = false,
                        Message = "RefreshToken has been revoked",
                    });
                }

                //check 6: AccessTokenId == JwtId in RefreshToken
                var jti = tokenInVerfication.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Jti).Value;

                if(storedToken.JwtId != jti)
                {
                    return Ok(new ApiRespond
                    {
                        Success = false,
                        Message = "Token does not match",
                    });
                }

                //update token is used
                storedToken.IsRevoked = true;
                storedToken.IsUsed  = true;
                _context.Update(storedToken);
                await _context.SaveChangesAsync();

                //Create new token
                var user = await _context.NguoiDungs.SingleOrDefaultAsync(x => x.Id == storedToken.UserId);
                var token = await GenerateToken(user);

                return Ok(new ApiRespond
                {
                    Success = true,
                    Message = "Renew token successfully",
                    Data = token
                });

            }
            catch(Exception ex)
            {
                return BadRequest(new ApiRespond
                {
                    Success = false,
                    Message = "Something went wrong",
                });
            }
        }

        private DateTime ConvertUnixTimeToDateTime(long utcExpiredDate)
        {
            var dateTimeInterval = new DateTime(1970,1,1,0,0,0,0,DateTimeKind.Utc);
            dateTimeInterval.AddSeconds(utcExpiredDate).ToUniversalTime();

            return dateTimeInterval;
        }
    }
}
