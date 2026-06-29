using Microsoft.EntityFrameworkCore;
using KeyVerse.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

// 1. ¡AQUÍ ESTÁ EL NEXO CON NEONTECH Y POSTGRESQL!
builder.Services.AddDbContext<KeyVerseContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("NeonConnection")));

// 2. Tu configuración del carrito (intacta y perfecta)
builder.Services.AddSession(options => {
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseSession(); // El carrito en memoria
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();