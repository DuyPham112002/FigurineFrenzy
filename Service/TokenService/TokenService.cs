using DBAccess.Entites;
using DBAccess.UnitOfWork;
using FigurineFrenzeyViewModel.Token;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Service.TokenService
{
    public interface ITokenService
    {
        string GenerateToken(string accountId, string phone, string role);
        Task<TokenDecodeViewModel> CheckTokenAsync(string token);
        Task<bool> SaveToken(string tokenvalue, string accId);
        Task<bool> DeleteTokenAsync(string tokenvalue, string accId);
    }
    public class TokenService : ITokenService
    {
        private readonly IUnitOfWork _uow;
        
        public TokenService(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<TokenDecodeViewModel> CheckTokenAsync(string token)
        {
            var tokenDecoded = DecodeToken(token);
            if (tokenDecoded != null)
            {
                Account validAccount = await _uow.Account.GetFirstOrDefaultAsync(a => a.AccountId == tokenDecoded.AccountId);
                if (validAccount != null)
                {
                    Token validToken = await _uow.Token.GetFirstOrDefaultAsync(q => q.AccountId == validAccount.AccountId && q.Value == token);
                    if (validToken != null)
                    {
                        if (validToken.CreatedDate.Value.AddDays(1) > DateTime.UtcNow)
                        {
                            return tokenDecoded;
                        }
                        else
                        {
                            return null;
                        }
                    }
                    else
                    {
                        return null;
                    }
                }
                else
                {
                    return null;
                }
            }
            else return null;
        }

        public async Task<bool> SaveToken(string tokenvalue, string accId)
        {
            try
            {
                //create or update token (save token to Db)
                Token token = await _uow.Token.GetFirstOrDefaultAsync(a => a.AccountId == accId);
                if (token == null)
                {
                    Token newToken = new Token
                    {
                        TokenId = Guid.NewGuid().ToString(),
                        AccountId = accId,
                        CreatedDate = DateTime.Now,
                        IsActive = true,
                        Value = tokenvalue
                    };
                    await _uow.Token.AddAsync(newToken);
                    await _uow.SaveAsync();
                    return true;
                }
                else
                {
                    token.Value = tokenvalue;
                    token.IsActive = true;
                    token.CreatedDate = DateTime.Now;
                    _uow.Token.Update(token);
                    await _uow.SaveAsync();
                    return true;
                }

            }catch (Exception ex)
            {
                return false;
            }
        }

        public string GenerateToken(string accountId, string phone, string role)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes("Cuoasnnlqlql48820938!#*#**....a9/./01002099((**");

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.NameIdentifier, accountId),
                    new Claim(ClaimTypes.MobilePhone, phone),
                    new Claim(ClaimTypes.Role, role)
                }),
                Expires = DateTime.UtcNow.AddDays(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);
            return tokenString;
        }

     

        private TokenDecodeViewModel DecodeToken(string token)
        {
            TokenDecodeViewModel tokeDecode = null;

            try
            {
                var key = Encoding.UTF8.GetBytes("Cuoasnnlqlql48820938!#*#**....a9/./01002099((**");
                var handler = new JwtSecurityTokenHandler();
                var validations = new TokenValidationParameters
                {
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    RequireExpirationTime = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true
                };
                var tokenInfo = handler.ValidateToken(token, validations, out var securityToken);
                string accountId = tokenInfo.Claims.First(claim => claim.Type == ClaimTypes.NameIdentifier).Value;
                string phone = tokenInfo.Claims.First(claim => claim.Type == ClaimTypes.MobilePhone).Value;
                string role = tokenInfo.Claims.First(claim => claim.Type == ClaimTypes.Role).Value;

                tokeDecode = new TokenDecodeViewModel
                {
                    AccountId = accountId,
                    Phone = phone,
                    Role = role,
                };

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return tokeDecode;
        }

        public async Task<bool> DeleteTokenAsync(string tokenvalue, string accId)
        {
            try
            {
                Token tokenInfo = await _uow.Token.GetFirstOrDefaultAsync(a => a.Value == tokenvalue && a.AccountId == accId);

                if (tokenInfo != null)
                {
                    tokenInfo.Value = " ";
                    tokenInfo.IsActive = false;

                    _uow.Token.Update(tokenInfo);
                    await _uow.SaveAsync();

                    return true;
                }
                else return false;
            }catch (Exception ex)
            {
                return false;
            }
        
        }
    }
}
