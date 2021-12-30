namespace GroceryListApi.Endpoints;

public static class SwaggerEndpoints
{
    public static WebApplication MapSwaggerEndpoints(this WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        return app;
    }
}