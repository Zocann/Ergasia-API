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
using Ergasia_API.Services.Interfaces.Model;
using Ergasia_API.Services.Model;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<PrimaryDbContext>(options =>
    options.UseSqlServer(
        Environment.GetEnvironmentVariable("DATABASE_CONNECTION_STRING"),
        //builder.Configuration.GetConnectionString("DatabaseConnection"),
        sqlOptions => sqlOptions.EnableRetryOnFailure()));

builder.Services.AddScoped<IUserRepository, UserEfRepository>();
builder.Services.AddScoped<IEmployerRepository, EmployerEfRepository>();
builder.Services.AddScoped<IJobRepository, JobEfRepository>();
builder.Services.AddScoped<IWorkerRepository, WorkerEfRepository>();
builder.Services.AddScoped<IWorkerJobRepository, WorkerJobEfRepository>();
builder.Services.AddScoped<IWorkerJobRequestRepository, WorkerJobRequestEfRepository>();
builder.Services.AddScoped<IEmployerRatingRepository, EmployerRatingEfRepository>();
builder.Services.AddScoped<IWorkerRatingRepository, WorkerRatingEfRepository>();

builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IEmployerService, EmployerService>();
builder.Services.AddScoped<IWorkerService, WorkerService>();
builder.Services.AddScoped<IJobService, JobService>();
builder.Services.AddScoped<IWorkerJobService, WorkerJobService>();
builder.Services.AddScoped<IWorkerJobRequestService, WorkerJobRequestService>();
builder.Services.AddScoped<IEmployerRatingService, EmployerRatingService>();
builder.Services.AddScoped<IWorkerRatingService, WorkerRatingService>();

builder.Services.AddScoped<IAuthorizationHandler, SameUserOrAdminHandler>();

builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IProfilePictureService, ProfilePictureService>();


builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        var tokenKey = Environment.GetEnvironmentVariable("TOKEN_KEY") ?? throw new Exception("Token key not found");
        //var tokenKey = builder.Configuration["TokenKey"] ?? throw new Exception("Token key not found");
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
    cfg.CreateMap<Job, Job>();
    cfg.CreateMap<Employer, Employer>();
    cfg.CreateMap<Worker, Worker>();
    cfg.CreateMap<EmployerRating, EmployerRating>();
    cfg.CreateMap<WorkerRating, WorkerRating>();
    cfg.CreateMap<WorkerJob, WorkerJob>();
    cfg.CreateMap<User, User>();
    cfg.CreateMap<WorkerJobRequest, WorkerJobRequest>();

    cfg.CreateMap<User, UserDto>().ReverseMap();
    cfg.CreateMap<Employer, EmployerDto>().ReverseMap();
    cfg.CreateMap<Worker, WorkerDto>().ReverseMap();
    cfg.CreateMap<Job, JobDto>().ReverseMap();
    cfg.CreateMap<UpdateUserDto, User>();
    cfg.CreateMap<RegisterDto, Employer>().ReverseMap();
    cfg.CreateMap<RegisterDto, Worker>().ReverseMap();
    cfg.CreateMap<Job, Job>().ReverseMap();


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
    .WithOrigins("https://ergasia-webapp.azurewebsites.net")
    //.WithOrigins("https://localhost:7001")
);


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("https://localhost:7000/swagger/v1/swagger.json", "API v1");
    });
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();