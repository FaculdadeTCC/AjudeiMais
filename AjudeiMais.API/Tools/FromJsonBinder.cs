using Microsoft.AspNetCore.Mvc.ModelBinding;
using Newtonsoft.Json;

public class FromJsonBinder<T> : IModelBinder
{
    public Task BindModelAsync(ModelBindingContext bindingContext)
    {
        var value = bindingContext.ValueProvider.GetValue(bindingContext.ModelName).FirstValue;

        if (string.IsNullOrEmpty(value))
        {
            bindingContext.Result = ModelBindingResult.Success(default(T));
            return Task.CompletedTask;
        }

        try
        {
            var result = JsonConvert.DeserializeObject<T>(value);
            bindingContext.Result = ModelBindingResult.Success(result);
        }
        catch (Exception)
        {
            bindingContext.ModelState.AddModelError(bindingContext.ModelName, "Invalid JSON.");
        }

        return Task.CompletedTask;
    }
}
