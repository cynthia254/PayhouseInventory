using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Org.BouncyCastle.Pkix;
using PayhouseDragonFly.CORE.Models.Emails;
using PayhouseDragonFly.CORE.Models.UserRegistration;
using PayhouseDragonFly.INFRASTRUCTURE.DataContext;
using PayhouseDragonFly.INFRASTRUCTURE.Services.ExtraServices;
using PayhouseDragonFly.INFRASTRUCTURE.Services.IServiceCoreInterfaces.IEmailServices;
using PayhouseDragonFly.INFRASTRUCTURE.Services.IServiceCoreInterfaces.IExtraServices;
using PayhouseDragonFly.INFRASTRUCTURE.Services.IServiceCoreInterfaces.IticketsCoreServices;

using PayhouseDragonFly.INFRASTRUCTURE.Services.IServiceCoreInterfaces.IUserServices;
using PayhouseDragonFly.INFRASTRUCTURE.Services.RoleServices;
using PayhouseDragonFly.INFRASTRUCTURE.Services.ServiceCore.EmailService;
using PayhouseDragonFly.INFRASTRUCTURE.Services.ServiceCore.ExtraServices.LoggedIuserServices;
using PayhouseDragonFly.INFRASTRUCTURE.Services.ServiceCore.TicketService;
using PayhouseDragonFly.INFRASTRUCTURE.Services.ServiceCore.UserServices;
using System.Text;
using Quartz;
using PayhouseDragonFly.INFRASTRUCTURE.Services.ExtraServices.jobs;
using PayhouseDragonFly.INFRASTRUCTURE.Services.ExtraServices.RoleChecker;
using PayhouseDragonFly.INFRASTRUCTURE.Services.IServiceCoreInterfaces.IExtraServices.IMathServices;
using PayhouseDragonFly.INFRASTRUCTURE.Services.ServiceCore.ExtraServices.MathServices;
using PayhouseDragonFly.INFRASTRUCTURE.Services.IServiceCoreInterfaces.IStockServices;
using PayhouseDragonFly.INFRASTRUCTURE.Services.StockServices;

var builder = WebApplication.CreateBuilder(args);
builder.Services.Configure<EmailConfiguration>(builder.Configuration.GetSection("EmailConfiguration"));
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc(
        "v1",
        new OpenApiInfo { Title = "PayhouseDragonFly.API", Version = "v1" }
    );
    c.AddSecurityDefinition(
        "Bearer",
        new OpenApiSecurityScheme
        {
            In = ParameterLocation.Header,
            Description = "Please Insert token",
            Name = "Authorization",
            Type = SecuritySchemeType.Http,
            BearerFormat = "Jwt",
            Scheme = "bearer"
        }
    );
    c.AddSecurityRequirement(
        new OpenApiSecurityRequirement
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
                            new string[] { }
                        }
        }
    );
    ;
});

builder.Services.AddScoped<IUserServices, UserServices>();
builder.Services.AddScoped<IticketsCoreServices, TicketService>();
builder.Services.AddScoped<IEExtraServices, ExtraServices>();
builder.Services.AddScoped<IEmailServices, EmailServices>();
builder.Services.AddScoped<IRoleServices, RoleServices>();
builder.Services.AddScoped<ILoggeinUserServices, LoggeinUserServices>();
builder.Services.AddScoped<IRoleChecker,RoleChecker>();
builder.Services.AddScoped<IMathServices, MathServices>();
builder.Services.AddScoped<IStockServices, StockServices>();
builder.Services
    .AddAuthentication(opt =>
    {
        opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(opt =>
    {
        opt.RequireHttpsMetadata = true;
        opt.SaveToken = true;
        opt.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.ASCII.GetBytes(PayhouseDragonFly.CORE.ConnectorClasses.TokenConstants.Constants.JWT_SECURITY_KEY)
            ),
            ValidateIssuer = false,
            ValidateAudience = false
        };
    });


builder.Services.AddDbContext<DragonFlyContext>(
    x => x.UseSqlServer(builder.Configuration.GetConnectionString("DevConnectiions"))
);
var devCorsPolicy = "devCorsPolicy";
builder.Services.AddCors(options =>
{
    options.AddPolicy(devCorsPolicy, builder =>
    {
        builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();

    });

});
// Add services to the container.
builder.Services.AddIdentity<PayhouseDragonFlyUsers, IdentityRole>(options => options.SignIn.RequireConfirmedAccount = true)
                    .AddEntityFrameworkStores<DragonFlyContext>()
                    .AddRoles<IdentityRole>();
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();
app.UseCors(devCorsPolicy);
// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "PayhouseDragonFly.API V1");
    c.RoutePrefix = "swagger"; // Swagger will be at /swagger
});



//app.UseAuthentication();
//app.UseAuthorization();


app.MapControllers();

app.Run();
