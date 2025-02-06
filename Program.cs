using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using TodoApi;
using Microsoft.OpenApi.Models;
using System;

var builder = WebApplication.CreateBuilder(args);
//addcors
builder.Services.AddCors(opt => opt.AddPolicy("MyPolicy", policy =>
{
    policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
}));
//database
builder.Services.AddDbContext<ToDoDbContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("biyrdoodqvbnf3g9axwi"),
        new MySqlServerVersion(new Version(7, 0, 0))
));

//swagger
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "My API",
        Version = "v1"
    });
});


var app = builder.Build();
//swagger
//if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "My API v1");
        options.RoutePrefix = string.Empty;     });

}
//function

app.MapGet("/items",async (ToDoDbContext DBcontext)=>
{
    // var items = await ; // שליפת כל ה-items ממסד הנתונים
    return await DBcontext.Items.ToListAsync();
    // Results.Ok(items);
});
app.MapGet("/items/{Id}",async (int Id,ToDoDbContext DBcontext)=>
{
     var item = await DBcontext.Items.FirstOrDefaultAsync(a=>a.Id==Id); // שליפת כל ה-items ממסד הנתונים
    return Results.Ok(item);
});
app.MapPost("/items",async (Item item,ToDoDbContext Db)=>
{
    item.IsComplete = false;
    Db.Items.Add(item);
    await Db.SaveChangesAsync();
});
app.MapPut("/items/{id}",async (int id,Item item,ToDoDbContext Db)=>
{
   var itemToUpdate=await Db.Items.FirstOrDefaultAsync(a=>a.Id==id);
    if(itemToUpdate!=null)
    {
        itemToUpdate.IsComplete=item.IsComplete;
    }
    await Db.SaveChangesAsync();
});
app.MapDelete("/items/{Id}",async 
(int Id,ToDoDbContext DBcontext)=>
{
    var item = await DBcontext.Items.FirstOrDefaultAsync(a => a.Id == Id); // שליפת כל ה-items ממסד הנתונים
    if (item != null)
    { 
    DBcontext.Items.Remove(item);
    await DBcontext.SaveChangesAsync();
}
});
app.UseCors("MyPolicy");
app.MapGet("/", () => "Application is running");
app.Run();
