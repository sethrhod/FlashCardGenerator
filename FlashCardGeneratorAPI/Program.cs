using FlashCardGeneratorAPI.Repositories;
using FlashCardGeneratorAPI.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddUserSecrets<Program>();

// Add services to the container.
builder.Services.AddSingleton<IDeckRepository, DeckRepository>();
builder.Services.AddScoped<IDeckService, DeckService>();
builder.Services.AddScoped<IGeneratorService, GeneratorService>(sp => new GeneratorService(
    apiKey: builder.Configuration["OPENAI_API_KEY"] ??
            throw new Exception("OPENAI_API_KEY not set in Environment variables"),
    model: builder.Configuration["OPENAI_MODEL"] ??
           throw new Exception("OPENAI_MODEL not set in Environment variables")
));


builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy
            .WithOrigins("https://localhost:3000") // or "*" for any origin (not recommended for production)
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.UseCors();

app.Run();