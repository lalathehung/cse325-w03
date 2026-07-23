using BlazingPizza.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddHttpClient();

builder.Services.AddControllers();

builder.Services.AddSqlite<PizzaStoreContext>(
    "Data Source=pizza.db");

builder.Services.AddScoped<OrderState>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
}

app.UseStaticFiles();
app.UseRouting();

app.MapControllers();
app.MapRazorPages();
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

// Initialize the database
using (var scope = app.Services.CreateScope())
{
    var db =
        scope.ServiceProvider.GetRequiredService<PizzaStoreContext>();

    if (db.Database.EnsureCreated())
    {
        SeedData.Initialize(db);
    }
}

app.Run();