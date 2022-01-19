using AuthGuard.API.Data;
using AuthGuard.API.Middleware;
using AuthGuard.API.Repositories.Abstracts;
using AuthGuard.API.Repositories.Concretes;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

#region Services
    // Add services to the container.
    builder.Services.AddCors(x =>
    {
        x.AddPolicy(name: "RoofAuthCORSPolicy",
                    builder =>
                    {
                        builder.WithOrigins("https://localhost:44340")
                               .WithMethods("GET", "POST");
                    });
    });

    builder.Services.Configure<AppSettings>(builder.Configuration.GetSection("AppSettings"));
    builder.Services.Configure<RoofAuthGuardSettings>(builder.Configuration.GetSection("RoofAuthGuardSettings"));

    builder.Services.AddScoped<IUserService, UserService>();

    builder.Services.AddDbContext<ApplicationDBContext>(options =>
    {
        options.UseSqlite(builder.Configuration.GetConnectionString("SQLiteConnection"));
    });

    builder.Services.AddControllers().AddNewtonsoftJson();

    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

    builder.Services.AddMemoryCache();
    
#endregion

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("RoofAuthCORSPolicy");

app.UseHttpsRedirection();

app.UseMiddleware<JwtMiddleware>();

app.MapControllers()
    .RequireCors("RoofAuthCORSPolicy");

app.Run();
