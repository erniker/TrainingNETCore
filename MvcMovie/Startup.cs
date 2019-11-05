using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MvcMovie.Localization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Localization.Routing;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using MvcMovie.Models;

namespace MvcMovie
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });
            /* Primero, creamos un array con las culturas soportadas. Si se intenta ajustar una cultura que no esté en este array, 
             * la aplicación utilizará la cultura por defecto. En nuestro caso hemos definido solo dos culturas, español de España 
             * e inglés de Estados Unidos. Fijaos en la forma de definir las culturas, que será la misma forma en la que más adelante 
             * pasaremos la cultura a la aplicación: es-ES y en-US. El formato es {ISO 639-1 código de idioma (dos caracteres)}
             * -{ISO 3166-1 código de país (dos caracteres)}. Así, en-GB es inglés de Gran Bretaña, es-AR español de Argentina...*/
            var supportedCultures = new[] { "es-ES", "en-US" };

            /* Cumplimentamos el objeto RequestLocalizationOptions. Le decimos que las culturas soportadas, tanto para UI como para 
             * métodos son las definidas en el array anterior. Así mismo, le decimos que la cultura por defecto tanto para la 
             * interfaz como para los métodos dependientes de cultura será es-ES (español de España).*/
            var localizationOptions = new RequestLocalizationOptions();
            localizationOptions.AddSupportedCultures(supportedCultures)
                .AddSupportedUICultures(supportedCultures)
                .SetDefaultCulture(supportedCultures[0])
                .RequestCultureProviders.Insert(0, new RouteDataRequestCultureProvider() { Options = localizationOptions });

            /*Guardamos estas opciones en el contenedor de dependencias, pues luego las necesitaremos para satisfacer un parámetro del
             * siguiente método y para pedirlo en uno de nuestros controladores, y así extraer la liD:\ASPNetCore\TrainingNETCore\MvcMovie\Startup.cssta de idiomas soportados:*/
            services.AddSingleton(localizationOptions);

            /*Añadimos todos los servicios necesarios para localizar nuestra aplicación al contenedor de inyección de dependencias, 
             * y le decimos que nuestros archivos de recursos estarán en la carpeta Resources*/
            services.AddLocalization(opt => opt.ResourcesPath = "Resources");

            // Añadimos los servicios de MVC, los servicios para localizar las vistas y las Data Anotations.
            services.AddMvc(mvcOptions =>
            {
                //mvcOptions.Filters.Add(typeof(CultureRedirectFilter));
                //Añadimos el filtro cuando configuramos el middleware de MVC
                mvcOptions.Filters.Add(new MiddlewareFilterAttribute(typeof(LocalizationPipeline)));
            }).SetCompatibilityVersion(CompatibilityVersion.Version_2_2)
              .AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix)
              .AddDataAnnotationsLocalization();


            services.AddDbContext<MvcMovieContext>(options =>
                    options.UseSqlServer(Configuration.GetConnectionString("MvcMovieContext")));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.

        /* Este método se llamará de manera automática en la inicialización de nuestra aplicación. Se utiliza para añadir todos los
        * middlewares que gestionarán una petición HTTP.  Aquí podemos añadir los parámetros que queramos, siempre y cuando sus
        * tipos hayan sido añadidos al contenedor de inyección de dependencias.
        * Por ejemplo, el parámetro de tipo RequestLocalizationOptions será satisfecho con el objeto que añadimos en la línea 51 
        * (services.AddSingleton(localizationOptions);)*/
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, RequestLocalizationOptions options)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();
            /*¡Aqí está la madre del cordero! Añadimos el middleware de localización al pipeline, con las opciones que definimos en el método
             * anterior sobre las culturas soportadas y la cultura por defecto:*/
            //app.UseRequestLocalization(options);
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{culture:regex(^[a-z]{{2}}(\\-[A-Z]{{2}})?$)}/{controller=Home}/{action=Index}/{id?}"
                    );
                routes.MapRoute(
                    //name: "defaultWithoutLanguage",
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}"
                    );
            });
        }
    }
}
