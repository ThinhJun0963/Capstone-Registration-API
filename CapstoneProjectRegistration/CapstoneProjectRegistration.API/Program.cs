using CapstoneProjectRegistration.Repositories;
using CapstoneProjectRegistration.Repositories.Data;
using CapstoneProjectRegistration.Repositories.Implements;
using CapstoneProjectRegistration.Repositories.Interfaces;
using CapstoneProjectRegistration.Services.Interface;
using CapstoneProjectRegistration.Services.MyMapper;
using CapstoneProjectRegistration.Services.Service.Topicss;
using Microsoft.EntityFrameworkCore;

namespace CapstoneProjectRegistration.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);


            var configuration = builder.Configuration.Get<AppSetting>();


            builder.Services.AddDbContext<CapstoneDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));


            builder.Services.AddHttpContextAccessor();
            //builder.Services.AddScoped<IClaimService, ClaimService>();
            builder.Services.AddAutoMapper(typeof(MapperConfigurationsProfile).Assembly);
            builder.Services.AddSingleton(configuration!);
            builder.Configuration.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
            builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
            builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

            builder.Services.AddScoped<ITopicService, TopicService>();

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
