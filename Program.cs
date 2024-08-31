using Microsoft.EntityFrameworkCore;
using TodoApi.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Configure PostgreSQL connection
var connectionString = builder.Configuration.GetConnectionString("TodoContext");
builder.Services.AddDbContext<TodoContext>(options =>
    options.UseNpgsql(connectionString));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseHsts();
}
app.UseHttpsRedirection();
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
