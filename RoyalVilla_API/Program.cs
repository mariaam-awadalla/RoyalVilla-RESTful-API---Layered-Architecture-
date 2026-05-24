using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Microsoft.IdentityModel.Tokens.Experimental;
using RoyalVilla_API.Data;
using RoyalVilla_API.Models;
using RoyalVilla_API.Models.DTO;
using RoyalVilla_API.Services;
using Scalar.AspNetCore;
using System.Reflection.Emit;
using System.Text;
using Microsoft.OpenApi.Models;
//https://localhost:7193/scalar/v1

var builder = WebApplication.CreateBuilder(args);
var key = Encoding.ASCII.GetBytes(builder.Configuration.GetSection("JWTSettings")["Secret"]);


builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme =
        JwtBearerDefaults.AuthenticationScheme;

    options.DefaultChallengeScheme =
        JwtBearerDefaults.AuthenticationScheme;

    options.DefaultScheme =
       JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero
    };

});
// Add services to the container.

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle



//builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "RoyalVilla API",
        Version = "v1"
    });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "Enter JWT Token",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});
//builder.Services.AddAutoMapper(CreateMap<Source, Destination>())
builder.Services.AddAutoMapper(map =>
{
    //map.CreateMap<VillaCreateDTO,Villa >();
    //but this support Villa → DTO and DTO → Villa
    map.CreateMap<Villa, VillaCreateDTO>().ReverseMap();
    map.CreateMap<Villa, VillaUpdateDTO>().ReverseMap();
    map.CreateMap<Villa, VillaDTO>().ReverseMap();
    map.CreateMap<VillaUpdateDTO, VillaDTO>().ReverseMap();
    map.CreateMap<User, UserDTO>().ReverseMap();

    // Villa Amenity Mapping

    // VillaAmenity Entity Class name not table name in db set ..bd set
    map.CreateMap<VillaAmenity, VillaAmenityCreateDTO>().ReverseMap();

    map.CreateMap<VillaAmenity, VillaAmenityUpdateDTO>().ReverseMap();

    map.CreateMap<VillaAmenity, VillaAmenityDTO>()
        .ForMember(
            dest => dest.VillaName,
            opt => opt.MapFrom(
                src => src.Villa != null
                    ? src.Villa.Name
                    : null
            )
        );

    map.CreateMap<VillaAmenityDTO, VillaAmenity>();

});

builder.Services.AddScoped<IAuthService, AuthService>();

var app = builder.Build();
await SeedDataAsync(app);
//app.UseSwagger();

//app.UseSwaggerUI();

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
//    app.UseSwagger();
//    app.UseSwaggerUI();

//    //app.MapScalarApiReference();

//    //app.MapScalarApiReference(options =>
//    //{
//    //    options.OpenApiRoutePattern = "/swagger/v1/swagger.json";
//    //});
//    //app.MapScalarApiReference();

//    app.MapScalarApiReference(options =>
//    {
//        options
//            .WithTitle("RoyalVilla API")
//            .WithOpenApiRoutePattern("/swagger/v1/swagger.json");
//    });
//}
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();

    app.MapScalarApiReference(options =>
    {
        options
            .WithTitle("RoyalVilla API")
            .WithOpenApiRoutePattern("/swagger/v1/swagger.json");
    });
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();


//instead of update-dadabase
static async Task SeedDataAsync(WebApplication app)
{
    using var scope = app.Services.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    await context.Database.MigrateAsync();

}

