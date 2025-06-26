using FileUploadDownload.Models;
using FileUploadDownload.Hubs;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<FileUploadDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddSignalR();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

// Đặt controller routes trước SignalR hub
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

// Thêm route cụ thể cho ChatHub controller
app.MapControllerRoute(
    name: "chathub",
    pattern: "ChatHub",
    defaults: new { controller = "ChatHub", action = "Index" });

// Đặt SignalR hub sau cùng với pattern khác
app.MapHub<ChatHub>("/signalr/chatHub");



app.Run();
