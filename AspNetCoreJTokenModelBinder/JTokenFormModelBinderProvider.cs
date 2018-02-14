using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AspNetCoreJTokenModelBinder
{
    public class JTokenFormModelBinderProvider : IModelBinderProvider
    {
        public IModelBinder GetBinder(ModelBinderProviderContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (context.Metadata.ModelType == typeof(JToken))
            {
                return new JTokenModelBinder();
            }

            return null;
        }

        private class JTokenModelBinder : IModelBinder
        {

            public async Task BindModelAsync(ModelBindingContext bindingContext)
            {
                if (bindingContext.HttpContext.Request.HasFormContentType)
                {
                    var formBinder = new FormCollectionModelBinder();
                    await formBinder.BindModelAsync(bindingContext);
                    if (bindingContext.Result.IsModelSet)
                        bindingContext.Result =
                            ModelBindingResult.Success(ToJToken((IFormCollection) bindingContext.Result.Model));
                }
                else
                {
                    if (bindingContext.HttpContext.Request.ContentType == "application/json")
                    {
                        try
                        {
                            var readStreamTask = new StreamReader(bindingContext.HttpContext.Request.Body).ReadToEndAsync();
                            var json = await readStreamTask;
                            using (var sr = new StringReader(json))
                            using (var jtr = new JsonTextReader(sr) {DateParseHandling = DateParseHandling.None})
                            {
                                bindingContext.Result = ModelBindingResult.Success(JToken.ReadFrom(jtr));
                            }
                        }
                        catch (Exception ex)
                        {
                            var bindingContextResult = ModelBindingResult.Failed();
                            bindingContext.Result = bindingContextResult;
                        }
                    }
                }
            }

            private static JToken ToJToken(IFormCollection form)
            {
                JObject target = new JObject();
                foreach (var element in form)
                {
                    Add(target, element.Key, element.Value);
                }
                if (target.Count == 1 && target[""] != null) return target[""];
                return target;

                void Add(JObject jo, string key, StringValues value)
                {
                    var chars = new[] { '.', '[' };

                    var x = key.IndexOfAny(chars);
                    if (x == -1)
                        jo[key] = value.LastOrDefault();
                    else
                    {
                        var name = key.Substring(0, x);
                        if (key[x] == '.')
                        {
                            var subJo = jo[name] as JObject ?? (JObject)(jo[name] = new JObject());
                            Add(subJo, key.Substring(x + 1), value);
                        }
                        else
                        {
                            var subJa = jo[name] as JArray ?? (JArray)(jo[name] = new JArray());

                            var closeBracketsIndex = key.IndexOf(']', x + 1);
                            var itemIndex = int.Parse(key.Substring(x + 1, closeBracketsIndex - x - 1));
                            while (subJa.Count < itemIndex + 1) subJa.Add(null);

                            if (closeBracketsIndex == key.Length - 1)
                            {
                                subJa[itemIndex] = value.LastOrDefault();
                                return;
                            }
                            if (key[closeBracketsIndex + 1] != '.') throw new Exception();
                            var remainder = key.Substring(closeBracketsIndex + 2);
                            var subJo = subJa[itemIndex] as JObject ?? (JObject)(subJa[itemIndex] = new JObject());
                            Add(subJo, remainder, value);
                        }
                    }
                }
            }
        }
    }
}
