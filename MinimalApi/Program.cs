using DataAccess;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using MinimalApi.Endpoints;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    var docFile = $"{typeof(Program).Assembly.GetName().Name}.xml";

    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, docFile));
});

builder.Services.AddDbContext<ReviewContext>(options =>
                options.UseSqlite("Filename=../Reviews.db"));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

//app.MapBookReviews();
app.MapBookReviewsWithLambdas();

app.Run();
