using ASM_C_3.Data;
using ASM_C_3.Interface;
using ASM_C_3.Models;
using ASM_C_3.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;


var builder = WebApplication.CreateBuilder(args);

// =============================
// 1?? Logging
// =============================
builder.Logging.ClearProviders();
builder.Logging.AddConsole();

// =============================
// 2?? MVC + Razor
// =============================
builder.Services.AddControllersWithViews();
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

// =============================
// 3?? Database context
// =============================
builder.Services.AddDbContext<TraNgheDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// =============================
// 4?? Identity configuration
// =============================
builder.Services
    .AddIdentity<ApplicationUser, IdentityRole>(options =>
    {
        options.Password.RequireDigit = false;
        options.Password.RequireLowercase = false;
        options.Password.RequireUppercase = false;
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequiredLength = 6;
        options.User.RequireUniqueEmail = false; // ch? yêu c?u username là duy nh?t
    })
    .AddEntityFrameworkStores<TraNgheDbContext>()
    .AddDefaultTokenProviders();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.AccessDeniedPath = "/Account/AccessDenied";
});

// Require authentication by default for all endpoints.
// Actions/controllers decorated with [AllowAnonymous] remain public (e.g. Account/Login, Register).
builder.Services.AddAuthorization(options =>
{
    options.FallbackPolicy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();
});

// =============================
// 5?? Register custom services (DI)
// =============================
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IInvoiceService, InvoiceService>();
builder.Services.AddScoped<ICartService, CartService>();
builder.Services.AddScoped<IVariantService, VariantService>();


// =============================
// 6?? Build app
// =============================
var app = builder.Build();

// =============================
// 7?? Apply migrations & seed data
// =============================
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var db = services.GetRequiredService<TraNgheDbContext>();
    db.Database.Migrate();

    // Optional: seeding d? li?u m?c ??nh (Admin, Role, Category, ...)
    await IdentitySeeder.SeedAsync(services);
}

// =============================
// 8?? Configure middleware
// =============================
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

// =============================
// 9?? Default Route
// =============================
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
