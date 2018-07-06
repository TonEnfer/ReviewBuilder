using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ReviewBuilder.Models;

namespace ReviewBuilder
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            // services.AddDbContext<ApplicationContext>(opt =>
            //     opt.UseSqlite("Data Source=data.db"));
            // services.AddDbContext<ApplicationContext>(opt =>
            //         opt.UseInMemoryDatabase("AppContext"));
                    
            services.AddMvc()
                    .SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
            //.ConfigureLogging(logging => logging.SetMinimumLevel(LogLevel.Trace));
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseDefaultFiles();
            app.UseStaticFiles();
            app.UseMvc();

        }
    }
}