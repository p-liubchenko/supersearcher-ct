#define MSSQL

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using SuperSearcher.BAWeb.Areas.Identity;
using SuperSearcher.BLL;
using SuperSearcher.DAL.Contexts;
using SuperSearcher.DAL.Entities;

namespace SuperSearcher.BAWeb
{
	public class Startup
	{
		public Startup(IConfiguration configuration)
		{
			Configuration = configuration;
		}

		public IConfiguration Configuration { get; }

		// This method gets called by the runtime. Use this method to add services to the container.
		// For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
		public void ConfigureServices(IServiceCollection services)
		{


#if DOCKER || MSSQL
			services.AddDbContext<ApplicationContext>(options =>
				options.UseSqlServer(
					Configuration.GetConnectionString("Smarter")));

			services.AddDbContext<ClientContext>(options =>
				options.UseSqlServer(
					Configuration.GetConnectionString("Smarter")));
#else
			services.AddDbContext<ApplicationContext>(options =>
				options.UseSqlite(
					Configuration.GetConnectionString("LiteDB")));

			services.AddDbContext<ClientContext>(options =>
				options.UseSqlite(
					Configuration.GetConnectionString("LiteDB")));

#endif

			services.AddDefaultIdentity<User>(options => options.SignIn.RequireConfirmedAccount = true)
				.AddEntityFrameworkStores<ClientContext>();

			services.AddRazorPages();
			services.AddServerSideBlazor();
			services.AddScoped<AuthenticationStateProvider, RevalidatingIdentityAuthenticationStateProvider<User>>();
			services.ConfigureBLL();
			
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
				app.UseDatabaseErrorPage();
			}
			else
			{
				app.UseExceptionHandler("/Error");
				// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
				app.UseHsts();
			}

			app.UseHttpsRedirection();
			app.UseStaticFiles();

			app.UseRouting();

			app.UseAuthentication();
			app.UseAuthorization();

			app.UseEndpoints(endpoints =>
			{
				endpoints.MapControllers();
				endpoints.MapBlazorHub();
				endpoints.MapFallbackToPage("/_Host");
			});
		}
	}
}
