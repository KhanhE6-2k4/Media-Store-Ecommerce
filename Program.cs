using MediaStore.Data;
using MediaStore.Services;
using MediaStore.Services.Email;
using Microsoft.Build.Execution;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Cookies;
using MediaStore.Services.Shipping;
using MediaStore.Services.Invoices;
using MediaStore.Services.Order;
using MediaStore.Services.Province;
using MediaStore.Services.Session;
using MediaStore.Services.Transaction;
using MediaStore.Services.Payment;
using MediaStore.Services.Payment.Vnpay;
using MediaStore.Subsystem.Payment;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.HttpOverrides;


var builder = WebApplication.CreateBuilder(args);

// Load config mặc định appsettings.json
builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

// Load config appsettings.Development.json nếu môi trường là Development
builder.Configuration.AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true);

// Đọc environment variables (override appsettings, dùng __ để phân cấp)
builder.Configuration.AddEnvironmentVariables();

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<AimsContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("Aims"));
});

// Add service to VnPay - Singleton() 
// Note:
// 1. VnPayService được đăng ký Singleton => chỉ 1 instance duy nhất trong suốt vòng đời app.
// 2. Tuyệt đối không lưu trạng thái liên quan đến HttpContext, Session bên trong VnPayService.
// 3. Mọi method trong VnPayService phải đảm bảo thread-safe.
// 4. Phù hợp cho việc build URL thanh toán, validate SecureHash, lấy config VNPAY.
builder.Services.AddSingleton<IVnPayService, VnPayService>();

// Cấu hình DataProtection lưu keys vào SQL Server thông qua EF Core.
// Giải quyết cảnh báo: "Storing keys in a directory that may not be persisted outside of the container"
// và "No XML encryptor configured" khi deploy trên Render (container ephemeral storage).
builder.Services.AddDataProtection()
    .PersistKeysToDbContext<AimsContext>()
    .SetApplicationName("MediaStore");

builder.Services.AddScoped<IShippingService, ShippingService>(); // DI

builder.Services.AddScoped<IInvoiceService, InvoiceService>(); // DI

builder.Services.AddScoped<IOrderService, OrderService>();  // DI

builder.Services.AddScoped<IProvinceService, ProvinceService>();  // DI

builder.Services.AddScoped<ISessionService, SessionService>();  // DI

builder.Services.AddScoped<ITransactionService, TransactionService>();  // DI

builder.Services.AddSingleton<PaymentCreator>();

// Đăng ký từng loại thanh toán
builder.Services.AddSingleton<IPaymentFactory, VnPayPaymentFactory>();

// Phan them vao
builder.Services.AddAuthentication("MyCookieAuth")
    .AddCookie("MyCookieAuth", options =>
    {
        options.LoginPath = "/Account/Login";
        // options.AccessDeniedPath = "/Account/AccessDenied";
        options.Events = new CookieAuthenticationEvents
        {
            OnRedirectToAccessDenied = context =>
            {
                context.Response.StatusCode = 403; // Trả 403 thay vì redirect
                return Task.CompletedTask;
            },
            OnRedirectToLogin = context =>
            {
                context.Response.StatusCode = 401; // Trả 401 thay vì redirect
                return Task.CompletedTask;
            }
        };
    });

builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
builder.Services.AddTransient<IEmailService, EmailService>();


builder.Services.AddAuthorization(); // Phan them vao


// Thiet lap de lua gio hang theo session
builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromSeconds(1200);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// dịch vụ singleton toàn hệ thống để có thể lấy được HttpContext ở bất kỳ đâu
// HttpContext chỉ có thể sử dụng trong Controller
builder.Services.AddHttpContextAccessor();


var app = builder.Build();

// Tự động apply migrations khi startup (bao gồm tạo bảng DataProtectionKeys).
// Cần thiết trên Render nơi không thể chạy lệnh migration thủ công.
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AimsContext>();
    db.Database.Migrate();
}

// Cấu hình ForwardedHeaders PHẢI đặt ĐẦU TIÊN trước tất cả middleware khác.
// Render xử lý TLS ở thiết bị bảo mật rồi chuyển tiếp HTTP nội bộ với header X-Forwarded-Proto.
// Nếu đặt sau UseSession/UseRouting, scheme/IP sẽ sai khi set session cookie → session bị mất.
app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
});

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();

    // Trên Render, HTTPS được reverse proxy xử lý nên không dùng UseHttpsRedirection.
    // Thay vào đó, dùng middleware tự redirect dựa theo header X-Forwarded-Proto.
    app.Use(async (context, next) =>
    {
        if (context.Request.Headers.TryGetValue("X-Forwarded-Proto", out var proto)
            && proto == "http")
        {
            var httpsUrl = $"https://{context.Request.Host}{context.Request.Path}{context.Request.QueryString}";
            context.Response.Redirect(httpsUrl, permanent: true);
            return;
        }
        await next();
    });
}
else
{
    app.UseHttpsRedirection();
}

app.UseStaticFiles();
app.UseRouting();
app.UseSession(); // Su dung Session

app.UseAuthentication(); // Phan them vao
app.UseAuthorization();
app.MapStaticAssets();

app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Dashboard}/{action=Index}/{id?}"
); // Phan them vao


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
app.Urls.Add($"http://*:{port}");

app.Run();
