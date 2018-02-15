# AspNetCoreJTokenModelBinder

Bind to JToken from form POSTS, following the standard ASP.MVC rules from https://haacked.com/archive/2008/10/23/model-binding-to-a-list.aspx/



## Installation

`install-package AspNetCoreJTokenModelBinder`

```

    public class Startup
    {
        ...

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc(options =>
            {
                options.ModelBinderProviders.Insert(0, new AspNetCoreJTokenModelBinder.JTokenFormModelBinderProvider());
            });
        }
        
        ...
        
    }
    
```

## Usage

```

    public class HomeController : Controller
    {
        [ValidateAntiForgeryToken]
        public IActionResult FormReciever(JToken content)
        {
            return Content(content.ToString(), "text/html");
        }
        public IActionResult ApplicationJsonReciever(JToken content)
        {
            return Content(content.ToString(), "text/html");
        }

    }
    
 ```
