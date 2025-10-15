using System.Text;
using Ergasia_API.Authorization.Requirements;
using Ergasia_API.Data;
using Ergasia_API.DTOs.Employer;
using Ergasia_API.DTOs.Job;
using Ergasia_API.DTOs.Rating;
using Ergasia_API.DTOs.User;
using Ergasia_API.DTOs.Worker;
using Ergasia_API.Models;
using Ergasia_API.Models.Interfaces;
using Ergasia_API.Models.Repositories;
using Ergasia_API.Services;
using Ergasia_API.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddOpenApi();

builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<PrimaryDbContext>(options =>
    options.UseSqlServer(
        Environment.GetEnvironmentVariable("DATABASE_CONNECTION_STRING"),
        sqlOptions => sqlOptions.EnableRetryOnFailure()));

builder.Services.AddScoped<IUserRepository, UserEfRepository>();
builder.Services.AddScoped<IEmployerRepository, EmployerEfRepository>();
builder.Services.AddScoped<IJobRepository, JobEfRepository>();
builder.Services.AddScoped<IWorkerRepository, WorkerEfRepository>();
builder.Services.AddScoped<IWorkerJobRepository, WorkerJobEfRepository>();
builder.Services.AddScoped<IWorkerJobRequestRepository, WorkerJobRequestEfRepository>();
builder.Services.AddScoped<IEmployerRatingRepository, EmployerRatingEfRepository>();
builder.Services.AddScoped<IWorkerRatingRepository, WorkerRatingEfRepository>();

builder.Services.AddScoped<IAuthorizationHandler, SameUserOrAdminHandler>();

builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IProfilePictureService, ProfilePictureService>();


builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        var tokenKey = Environment.GetEnvironmentVariable("TOKEN_KEY") ?? throw new Exception("Token key not found");
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenKey)),
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true
        };
    });

builder.Services.AddAuthorization(o =>
{
    o.AddPolicy("SameUserOrAdmin", policy => policy.AddRequirements(new SameUserOrAdminRequirement()));
});

builder.Services.AddIdentityCore<User>(options =>
    {
        options.User.RequireUniqueEmail = true;
        options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
    })
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<PrimaryDbContext>();

builder.Services.AddAutoMapper(cfg => 
{
    cfg.CreateMap<User, UserDto>().ReverseMap();
    cfg.CreateMap<Employer, EmployerDto>().ReverseMap();
    cfg.CreateMap<Worker, WorkerDto>().ReverseMap();
    cfg.CreateMap<Job, JobDto>().ReverseMap();
    cfg.CreateMap<Job, Job>();
    cfg.CreateMap<UpdateUserDto, User>();
    cfg.CreateMap<RegisterDto, Employer>().ReverseMap();
    cfg.CreateMap<RegisterDto, Worker>().ReverseMap();
    
    
    cfg.CreateMap<WorkerRating, WorkerRatingDto>()
        .ForMember(vrd => vrd.WorkerDto, opt 
            => opt.MapFrom(vr => vr.Worker))
        .ForMember(vrd => vrd.EmployerDto, opt 
            => opt.MapFrom(vr => vr.Employer));
    
    cfg.CreateMap<EmployerRating, EmployerRatingDto>()
        .ForMember(erd => erd.WorkerDto, opt 
            => opt.MapFrom(er => er.Worker))
        .ForMember(erd => erd.EmployerDto, opt 
            => opt.MapFrom(er => er.Employer));
    
    cfg.CreateMap<WorkerJob, WorkerJobDto>()
        .ForMember(jrd => jrd.WorkerDto, opt 
            => opt.MapFrom(wj => wj.Worker))
        .ForMember(jrd => jrd.JobDto, opt 
            => opt.MapFrom(wj => wj.Job));
    
    cfg.CreateMap<WorkerJobRequest, JobRequestDto>()
        .ForMember(jrd => jrd.WorkerDto, opt 
            => opt.MapFrom(wj => wj.Worker))
        .ForMember(jrd => jrd.JobDto, opt 
            => opt.MapFrom(wj => wj.Job));
});

var app = builder.Build();

// Uncomment this when creating WebApp


app.UseCors(policy => policy
    .AllowAnyHeader()
    .AllowAnyMethod()
    .AllowCredentials()
    .WithOrigins("ergasia-webapp.azurewebsites.net")
);


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "API v1");
    });
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();