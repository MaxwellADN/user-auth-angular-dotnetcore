using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace dotnet_core.Common
{
    public class Utils
    {
        public static string Encrypt(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }   

        public static bool Verify(string password, string hashedPassword)
        {
            return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
        }   

        public static string GenerateJWTToken(long userId, bool rememberMe)
        {
            IdentityOptions options = new IdentityOptions();
            // We will setup the parameters of token generation
            SecurityTokenDescriptor tokenDescription = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(
                    new Claim[]{
                        new Claim("userId", userId.ToString())
                    }
                ),
                Expires = rememberMe ? DateTime.UtcNow.AddDays(7) : DateTime.UtcNow.AddDays(1),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Properties.Resources.JWT_Secret)),
                    SecurityAlgorithms.HmacSha256Signature
                )
            };
            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
            SecurityToken securityToken = tokenHandler.CreateToken(tokenDescription);
            return tokenHandler.WriteToken(securityToken);
        }

        public static string GenerateJWTTokenByEmail(string email)
        {
            IdentityOptions options = new IdentityOptions();
            // We will setup the parameters of token generation
            SecurityTokenDescriptor tokenDescription = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(
                    new Claim[]{
                        new Claim("email", email)
                    }
                ),
                Expires = DateTime.UtcNow.AddDays(1),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Properties.Resources.JWT_Secret)),
                    SecurityAlgorithms.HmacSha256Signature
                )
            };
            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
            SecurityToken securityToken = tokenHandler.CreateToken(tokenDescription);
            return tokenHandler.WriteToken(securityToken);
        }

        public static async Task<bool> SendWelcomeEmail(string fullname, string email)
        {
            using EmailService emailService = new EmailService();
            emailService._body.Append("<html>");
            emailService._body.Append($"Hello {fullname}! <br /><br />");
            emailService._body.Append("Thank you for signing up!  <br /><br />");
            emailService._body.Append("Regards <br /><br />");
            emailService._body.Append("Company brand");
            emailService._body.Append("</html>");
            return await emailService.SendEmailAsync(fullname,email, $"Welcome {fullname}");
        }

        public static async Task<bool> SendRecoveryLinkEmail(string link, string fullname, string userEmal)
        {
            using EmailService emailService = new EmailService();
            emailService._body.Append("<html>");
            emailService._body.Append($"Hello {fullname}! <br /><br />");
            emailService._body.Append("Please click on the link below o change your password<br /><br />");
            emailService._body.Append("<a href='"+link+"' target='_blank'>Click here</a> <br /><br />");
            emailService._body.Append("This link is available for 24h <br /><br />");
            emailService._body.Append("Regards <br /><br />");
            emailService._body.Append("Company brand");
            emailService._body.Append("</html>");
            return await emailService.SendEmailAsync(fullname ,userEmal, $"Password recovery...");
        }
    }
}