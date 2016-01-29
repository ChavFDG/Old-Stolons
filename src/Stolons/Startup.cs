using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Hosting;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Data.Entity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Stolons.Models;
using Stolons.Services;
using Microsoft.AspNet.Identity;
using System.IO;

namespace Stolons
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            // Set up configuration sources.
            var builder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true);

            if (env.IsDevelopment())
            {
                // For more details on using the user secret store see http://go.microsoft.com/fwlink/?LinkID=532709
                builder.AddUserSecrets();
            }

            builder.AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; set; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services.AddEntityFramework()
                .AddSqlite()
                .AddDbContext<ApplicationDbContext>(options =>
                    options.UseSqlite(Configuration["Data:DefaultConnection:ConnectionString"]));

            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            services.AddMvc();

            // Add application services.
            services.AddTransient<IEmailSender, AuthMessageSender>();
            services.AddTransient<ISmsSender, AuthMessageSender>();

            //Password policy
            //stackoverflow.com/questions/27831597/how-do-i-define-the-password-rules-for-identity-in-asp-net-5-mvc-6-vnext
            services.AddIdentity<ApplicationUser, IdentityRole>(o => {
                // configure identity options
                o.Password.RequireDigit = false;
                o.Password.RequireLowercase = false;
                o.Password.RequireUppercase = false;
                o.Password.RequireNonLetterOrDigit = false; ;
                o.Password.RequiredLength = 6;
            }).AddDefaultTokenProviders();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public async void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory, IServiceProvider serviceProvider, ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            if (env.IsDevelopment())
            {
                app.UseBrowserLink();
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");

                // For more details on creating database during deployment see http://go.microsoft.com/fwlink/?LinkID=615859
                try
                {
                    using (var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>()
                        .CreateScope())
                    {
                        serviceScope.ServiceProvider.GetService<ApplicationDbContext>()
                             .Database.Migrate();
                    }
                }
                catch { }
            }

            app.UseIISPlatformHandler(options => options.AuthenticationDescriptions.Clear());

            app.UseStaticFiles();

            app.UseIdentity();

            // To configure external authentication please see http://go.microsoft.com/fwlink/?LinkID=532715

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });

            await CreateRoles(serviceProvider);
            await CreateAdminAcount(context, userManager);
            CreateProductCategories(context);
        }

        private void CreateProductCategories(ApplicationDbContext context)
        {
            ProductType fresh = new ProductType("Produits frais");
            context.ProductTypes.Add(fresh);
            context.ProductFamillys.Add(new ProductFamilly(fresh, "Fruits"));
            context.ProductFamillys.Add(new ProductFamilly(fresh, "Légumes"));
            context.ProductFamillys.Add(new ProductFamilly(fresh, "Produits laitiers"));
            context.ProductFamillys.Add(new ProductFamilly(fresh, "Oeufs"));
            //ProductFamilly fruits = new ProductFamilly(fresh, "Fruits");
            //ProductFamilly vegetables = new ProductFamilly(fresh, "Légumes");
            //ProductFamilly dairy = new ProductFamilly(fresh, "Produits laitiers");
            //ProductFamilly eggs = new ProductFamilly(fresh, "Oeufs");

            ProductType bakery = new ProductType("Boulangerie");
            context.ProductTypes.Add(bakery);
            context.ProductFamillys.Add(new ProductFamilly(bakery, "Farines"));

            ProductType grocery = new ProductType("Epicerie");
            context.ProductTypes.Add(grocery);
            context.ProductFamillys.Add(new ProductFamilly(grocery, "Conserves"));

            ProductType bevarages = new ProductType("Boissons");
            context.ProductTypes.Add(bevarages);
            context.ProductFamillys.Add(new ProductFamilly(bevarages, "Alcools"));
            context.ProductFamillys.Add(new ProductFamilly(bevarages, "Sans alcool"));

            ProductType other = new ProductType("Autres");
            context.ProductTypes.Add(other);
            context.ProductFamillys.Add(new ProductFamilly(other, "Savon"));
            context.SaveChanges();
        }

        private async Task CreateRoles(IServiceProvider serviceProvider)
        {
            var RoleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            List<string> roleNames = new List<string>();
            //Adding Role
            roleNames.AddRange(Configurations.GetRoles());
            //Adding UserType
            roleNames.AddRange(Configurations.GetUserTypes());
            IdentityResult roleResult;
            foreach (var roleName in roleNames)
            {
                var roleExist = await RoleManager.RoleExistsAsync(roleName);
                if (!roleExist)
                {
                    roleResult = await RoleManager.CreateAsync(new IdentityRole(roleName));
                }
            }
        }

        private async Task CreateAdminAcount(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            await CreateAcount(context,
                                userManager,
                                "Admin",
                                "Admin",
                                "admin@admin.com",
                                "admin@admin.com",
                                Configurations.Role.Administrator,
                                Configurations.UserType.SimpleUser);
            await CreateAcount(context,
                    userManager,
                    "PARAVEL",
                    "Damien",
                    "damien.paravel@gmail.com",
                    "damien.paravel@gmail.com",
                    Configurations.Role.Administrator,
                    Configurations.UserType.Producer);
            await CreateAcount(context,
                    userManager,
                    "MICHON",
                    "Nicolas",
                    "nicolas.michon@zoho.com",
                    "nicolas.michon@zoho.com",
                    Configurations.Role.Administrator,
                    Configurations.UserType.Consumer);
        }
        private async Task CreateAcount(ApplicationDbContext context, UserManager<ApplicationUser> userManager, string name, string surname, string email, string password, Configurations.Role role, Configurations.UserType userType)
        {
            if (context.Consumers.Any(x => x.Email == email))
                return;
            Consumer consumer = new Consumer();
            consumer.Name = name;
            consumer.Surname = surname;
            consumer.Email = email;
            consumer.Avatar = Path.Combine(Configurations.UserAvatarStockagePath, Configurations.DefaultFileName);
            consumer.RegistrationDate = DateTime.Now;
            consumer.Enable = true;
            context.Consumers.Add(consumer);


            #region Creating linked application data
            var appUser = new ApplicationUser { UserName = consumer.Email, Email = consumer.Email };
            appUser.User = consumer;

            var result = await userManager.CreateAsync(appUser, password);
            if (result.Succeeded)
            {
                //Add user role
                result = await userManager.AddToRoleAsync(appUser, role.ToString());
                //Add user type
                result = await userManager.AddToRoleAsync(appUser, userType.ToString());
            }
            #endregion Creating linked application data

            context.SaveChanges();

        }

        // Entry point for the application.
        public static void Main(string[] args) => WebApplication.Run<Startup>(args);
    }
}
