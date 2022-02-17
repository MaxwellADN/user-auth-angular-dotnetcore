using Microsoft.AspNetCore.Mvc;
using dotnet_core.Models;
using System.Threading.Tasks;
using System;
using Microsoft.EntityFrameworkCore;
using dotnet_core.Common;
using Microsoft.AspNetCore.Authorization;

namespace dotnet_core.Controllers
{
    [Route("api/[controller]")]
    public class AuthController : Controller
    {
       private readonly DatabaseContext _context;

       public AuthController(DatabaseContext context)
       {
           _context = context;
       }

       [HttpPost("sign-in")]
       public async Task<ActionResult<User>> SignIn([FromBody] User user)
       {
           try
           {
                var dbuser = await _context.Users
                    .Include(x => x.Role)
                    .FirstOrDefaultAsync(x => x.Email == user.Email);
                if (dbuser == null) return NotFound();
                // Verify password 
                if (Utils.Verify(user.Password, dbuser.Password))
                {
                    dbuser.Token = Utils.GenerateJWTToken(dbuser.Id, user.RememberMe);
                    return Ok(dbuser);
                }
                return BadRequest();
           }
           catch(Exception ex)
           {
               return BadRequest(ex);
           }
       }

       [HttpPost("sign-up")]
       public async Task<ActionResult<User>> SignUp([FromBody] User user)
       {
           var transaction = await _context.Database.BeginTransactionAsync();
           try
           {
               //Check if user already exist in the database
                var dbuser = await _context.Users
                    .Include(x => x.Role)
                    .FirstOrDefaultAsync(x => x.Email == user.Email);

                //If this user already exist return a conflit 
                if (dbuser != null) return Conflict(); // HTTP:409

                //Create and bind a tenant to the signed up user
                //When this user will add another users they will also bind to this tenant
                //So they can see same data in the protected area 
                Tenant tenant = new Tenant()
                {
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };
                user.Tenant = tenant;
                user.CreatedAt = DateTime.Now;
                user.UpdatedAt = DateTime.Now;

                //Check that password is not null or empty
                if (!string.IsNullOrEmpty(user.Password))
                    user.Password = Utils.Encrypt(user.Password); // Hash password
                
                //Store the user in the database
                await _context.Users.AddAsync(user);   
                await _context.SaveChangesAsync();       

                //Generate a token to let user access the protected area after sign up
                user.Token = Utils.GenerateJWTToken(user.Id, user.RememberMe);

                //We will need the user role to access the protected area
                user.Role = await _context.Roles.FirstOrDefaultAsync(x => x.Id == user.RoleId);

                //Send welcome email after sign up
                await Utils.SendWelcomeEmail(user.Fullname, user.Email);

                //Commit transaction if every is right.
                await transaction.CommitAsync();

                return Created("", user);
           }
           catch(Exception ex)
           {
               // Rollback transaction if something went wrong. ex: while sending welcome email
               //That avoid storing user into database
               await transaction.RollbackAsync();
               return BadRequest(ex);
           }
       }

       [HttpPost("send-recovery-link")]
       public async Task<IActionResult> SendRecoveryLink([FromBody] User user)
       {
           try
           {
                var dbuser = await _context.Users
                    .Include(x => x.Role)
                    .FirstOrDefaultAsync(x => x.Email == user.Email);
                if (dbuser != null)
                {
                    //This will generate a token that will be available for 24h.
                    string recoveryToken = Utils.GenerateJWTTokenByEmail(dbuser.Email);
                    //With this link that have in param the token the user can access
                    //The password update page if the token still available
                    string recoveryLink = $"{user.AppOriginUrl}/auth/reset-password/{recoveryToken}";
                    bool isSent = await Utils.SendRecoveryLinkEmail(recoveryLink, dbuser.Fullname, dbuser.Email);
                    if (isSent) return Created("", new { recoveryToken, dbuser.Email });
                }
                return NotFound();
           }
           catch(Exception ex)
           {
               return BadRequest(ex);
           }
       }

       [Authorize]
       [HttpPut("reset-password")]
       public async Task<IActionResult> UpdatePassword([FromBody] User user)
       {
           try
           {
                var dbuser = await _context.Users
                    .Include(x => x.Role)
                    .FirstOrDefaultAsync(x => x.Email == user.Email);
                if (dbuser != null)
                {
                    dbuser.Password = Utils.Encrypt(user.Password);
                    _context.Users.Update(dbuser);
                    await _context.SaveChangesAsync();
                    return Ok(dbuser);
                }
                return  BadRequest();
           }
           catch(Exception ex)
           {
               return BadRequest(ex);
           }
       }
    }
}