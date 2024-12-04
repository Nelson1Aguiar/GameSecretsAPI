using GameSecretsAPI.Interfaces;
using GameSecretsAPI.Transactional;

namespace GameSecretsAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Adiciona serviços ao contêiner.
            builder.Services.AddSingleton<IGameTRA, GameTRA>();
            builder.Services.AddControllers();

            // Configura CORS
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAllOrigins", policy =>
                {
                    policy.AllowAnyOrigin()   // Permite qualquer origem
                          .AllowAnyMethod()   // Permite qualquer método HTTP
                          .AllowAnyHeader();  // Permite qualquer cabeçalho
                });
            });

            // Configuração Swagger
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configura o pipeline de requisições
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseCors("AllowAllOrigins");  // Configuração do CORS antes de outros middlewares
            app.UseHttpsRedirection();
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
