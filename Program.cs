using GameSecretsAPI.Interfaces;
using GameSecretsAPI.Transactional;

namespace GameSecretsAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Adiciona servi�os ao cont�iner.
            builder.Services.AddSingleton<IGameTRA, GameTRA>();
            builder.Services.AddControllers();

            // Configura CORS
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAllOrigins", policy =>
                {
                    policy.AllowAnyOrigin()   // Permite qualquer origem
                          .AllowAnyMethod()   // Permite qualquer m�todo HTTP
                          .AllowAnyHeader();  // Permite qualquer cabe�alho
                });
            });

            // Configura��o Swagger
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configura o pipeline de requisi��es
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseWebSockets();

            app.Map("/chat", async context =>
            {
                if (context.WebSockets.IsWebSocketRequest)
                {
                    using var webSocket = await context.WebSockets.AcceptWebSocketAsync();
                    await ChatWebSocketHandler.HandleAsync(webSocket);
                }
                else
                {
                    context.Response.StatusCode = 400;
                }
            });

            app.UseCors("AllowAllOrigins");
            app.UseHttpsRedirection();
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
