using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using TodoApi;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);
//addcors
builder.Services.AddCors(opt => opt.AddPolicy("MyPolicy", policy =>
{
    policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
}));
//database
builder.Services.AddDbContext<ToDoDbContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("practicode3"),
        new MySqlServerVersion(new Version(7, 0, 0))
));

//swagger
builder.Services.AddEndpointsApiExplorer();


var app = builder.Build();
//swagger
//if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
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
    Db.Items.Add(item);
    await Db.SaveChangesAsync();
});
app.MapPut("/items/{id}",async (int id,Item item,ToDoDbContext Db)=>
{
   var itemToUpdate=await Db.Items.FirstOrDefaultAsync(a=>a.Id==id);
    if(itemToUpdate!=null)
    {
        itemToUpdate.Name=item.Name;
        itemToUpdate.IsComplete=item.IsComplete;
    }
    else { Db.Items.Add(item); }
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
