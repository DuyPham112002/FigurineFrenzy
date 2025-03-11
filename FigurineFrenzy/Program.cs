using Autofac;
using Autofac.Extensions.DependencyInjection;
using DBAccess.Entites;
using DBAccess.UnitOfWork;
using FigurineFrenzy.Controllers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Identity.Client;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Service.AccountService;
using Service.AdminService;
using Service.AuctionService;
using Service.CategoryService;
using Service.HashService;
using Service.Img;
using Service.ImgSet;
using Service.ItemService;
using Service.RoleService;
using Service.TokenService;
using Service.UserService;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy => policy
           .AllowAnyOrigin()
           .AllowAnyHeader()
           .AllowAnyMethod()
          );
});
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "User Auth", Version = "v1", Description = "Services to Authenticate user" });


    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Please enter a valid token in the following format: {your token here} do not add the word 'Bearer' before it."
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                },
                Scheme = "oauth2",
                Name = "Bearer",
                In = ParameterLocation.Header,
            },
            new List<string>()
        }
    });
});


// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();


//Add JWT 
var key = Encoding.UTF8.GetBytes("Cuoasnnlqlql48820938!#*#**....a9/./01002099((**");
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(
    options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key)
        };
    });

builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());

builder.Host.ConfigureContainer<ContainerBuilder>(builder =>
{
    builder.RegisterType<UnitOfWork>().As<IUnitOfWork>();
    builder.RegisterType<AccountService>().As<IAccountService>();
    builder.RegisterType<UserService>().As<IUserService>();
    builder.RegisterType<RoleService>().As<IRoleService>();
    builder.RegisterType<HashService>().As<IHashService>();
    builder.RegisterType<FigurineFrenzyContext>().AsSelf(); 
    builder.RegisterType<Validator>().As<IValidator>();
    builder.RegisterType<TokenService>().As<ITokenService>();
    builder.RegisterType<AdminService>().As<IAdminService>();
    builder.RegisterType<CategoryService>().As<ICategoryService>();
    builder.RegisterType<ItemService>().As<IItemService>();
    builder.RegisterType<ImgService>().As<IImgService>();
    builder.RegisterType<ImgSetService>().As<IImgSetService>();
    builder.RegisterType<AuctionService>().As<IAuctionService>();
});

var app = builder.Build();

//Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    //app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseCors();
if (builder.Environment.IsDevelopment())
{
    app.UseSwaggerUI(options => // UseSwaggerUI is called only in Development.
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
        options.RoutePrefix = string.Empty;
    });
}

app.MapStaticAssets();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
