
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using ToDoList.Configurations;
using ToDoList.Models;
using ToDoList.UnitOfWorks;

namespace ToDoList
{
    public class Program
    {
        public static void Main(string[] args)
        {
            string txt = "ToDoList";
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();

            builder.Services.AddEndpointsApiExplorer();
            #region Swagger Configurations
            builder.Services.AddSwaggerGen(op =>
            {
                op.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "ToDoList API - V1",
                    Version = "v1",


                });
                op.EnableAnnotations();
            });

            #endregion
            builder.Services.AddDbContext<ToDoListContext>(op => op.UseSqlServer(builder.Configuration.GetConnectionString("TODO_Con")));
            builder.Services.AddScoped<UnitOfWork>();
            builder.Services.AddAutoMapper(typeof(TaskMapperConfig));
            builder.Services.AddCors(options =>
            {
                options.AddPolicy(txt,
                    builder =>
                    {
                        builder.AllowAnyOrigin();
                        builder.AllowAnyHeader();
                        builder.AllowAnyMethod();
                    });
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();
            app.UseCors(txt);

            app.MapControllers();

            app.Run();
        }
    }
}
