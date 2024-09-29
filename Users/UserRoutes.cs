using GodzillaLocalFilmes.Data;
using GodzillaLocalFilmes.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace GodzillaLocalFilmes.Users
{
    public static class UserRoutes
    {
        public static void AddUserRoutes(this WebApplication app)
        {
            // Endpoint para inserir um novo usuário
            app.MapPost("users/user", async (AddUserRequest request, AppDbContext context, CancellationToken ct) =>
            {
                try
                {
                    bool emailExist = await context.Users.AnyAsync(user => user.Email == request.Email);
                    if (emailExist)
                        return Results.Conflict("Já existe um usuário com este e-mail.");

                    var newUser = new User(request.Email, request.Password, request.Name);
                    await context.Users.AddAsync(newUser);
                    await context.SaveChangesAsync(ct);

                    string token = GenerateToken(newUser);

                    return Results.Ok(new
                    {
                        auth = true,
                        usuario = new
                        {
                            id = newUser.UserId,
                            email = newUser.Email,
                            nome = newUser.Name
                        },
                        token
                    });
                }
                catch (Exception ex)
                {
                    return Results.Problem($"Ocorreu um erro: {ex.Message}");
                }
            });
        }

        private static string GenerateToken(User user)
        {
            var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes("8OZQZrDnqJ52cXeAoHWzI1rZSKSfPpaLGc5FI-OPSpE="));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim("id", user.UserId.ToString())
            };

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddHours(1),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
