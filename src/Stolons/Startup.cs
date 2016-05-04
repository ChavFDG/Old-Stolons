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
using System.Threading;
using Stolons.Tools;

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

            SetConfigurations(context);
            await CreateRoles(serviceProvider);
            await CreateAdminAcount(context, userManager);
            CreateProductCategories(context);
            CreateProductsSamples(context);
            Thread billManager = new Thread(() => BillGenerator.ManageBills(context));
            Configurations.Environment = env;
            billManager.Start();
        }

        private void SetConfigurations(ApplicationDbContext context)
        {

            if (context.ApplicationConfig.Any())
            {
                Configurations.ApplicationConfig = context.ApplicationConfig.First();
            }
            else
            {
                Configurations.ApplicationConfig = new ApplicationConfig();
                //General
                Configurations.ApplicationConfig.StolonsLabel = "Association Stolons";
                Configurations.ApplicationConfig.StolonsAddress = "Chemin de Saint Clair, 07000 PRIVAS";
                Configurations.ApplicationConfig.StolonsPhoneNumber = "06 64 86 66 93";
                Configurations.ApplicationConfig.StolonsAboutPageText = @"Stolons est une struture visant à favoriser ....blablabla";
                //Email
                Configurations.ApplicationConfig.StolonsMailAdress = "asso.stolons@gmail.com";
                Configurations.ApplicationConfig.StolonsMailPassword = "ProjectStolons2016";
                //Message
                Configurations.ApplicationConfig.OrderDeliveryMessage= "Votre panier est disponible jeudi de 16h à 20 au : chemin de Saint Clair 07000 PRIVAS";
                //Préparation commande
                Configurations.ApplicationConfig.PreparationDayStartDate = DayOfWeek.Wednesday;
                Configurations.ApplicationConfig.PreparationHourStartDate = 12;
                Configurations.ApplicationConfig.PreparationMinuteStartDate = 0;
                //Mise à jour stock
                Configurations.ApplicationConfig.StockUpdateDayStartDate = DayOfWeek.Thursday;
                Configurations.ApplicationConfig.StockUpdateHourStartDate = 12;
                Configurations.ApplicationConfig.StockUpdateMinuteStartDate= 0;
                //Commandes
                Configurations.ApplicationConfig.OrderDayStartDate = DayOfWeek.Sunday;
                Configurations.ApplicationConfig.OrderHourStartDate = 0;
                Configurations.ApplicationConfig.OrderMinuteStartDate = 0;
                //Simulation
                Configurations.ApplicationConfig.IsModeSimulated = true;
                Configurations.ApplicationConfig.SimulationMode = ApplicationConfig.Modes.StockUpdate;
                //
                context.Add(Configurations.ApplicationConfig);
                context.SaveChanges();
            }

        }

        private void CreateProductCategories(ApplicationDbContext context)
        {
            ProductType fresh = CreateProductType(context, "Produits frais");
            CreateProductFamily(context, fresh, "Fruits");
            CreateProductFamily(context, fresh, "Légumes");
            CreateProductFamily(context, fresh, "Produits laitiers");
            CreateProductFamily(context, fresh, "Oeufs");

            ProductType bakery = CreateProductType(context, "Boulangerie");
            CreateProductFamily(context, bakery, "Farines");
            CreateProductFamily(context, bakery, "Pains");

            ProductType grocery = CreateProductType(context, "Epicerie");
            CreateProductFamily(context, grocery, "Conserves");

            ProductType bevarages = CreateProductType(context, "Boissons");
            CreateProductFamily(context, bevarages, "Alcools");
            CreateProductFamily(context, bevarages, "Sans alcool");

            ProductType other = CreateProductType(context, "Autres");
            CreateProductFamily(context, other, "Savons");

            context.SaveChanges();
        }

        private ProductType CreateProductType(ApplicationDbContext context, string name)
        {
            ProductType type = context.ProductTypes.FirstOrDefault(x=> x.Name == name);
            if (type == null)
            {
                type = new ProductType(name);
                context.ProductTypes.Add(type);
            }
            return type;
        }

        private ProductFamilly CreateProductFamily(ApplicationDbContext context, ProductType type, string name)
        {
            ProductFamilly family = context.ProductFamillys.FirstOrDefault(x=> x.FamillyName == name);
            if (family == null)
            {
                family = new ProductFamilly(type, name);
                context.ProductFamillys.Add(family);
            }
            return family;
        }

        private void CreateProductsSamples(ApplicationDbContext context)
        {
            if (context.Products.Any())
                return;
            Product pain = new Product();
            pain.Name = "Pain complet";
            pain.Description = "Pain farine complete T80";
            pain.Labels.Add(Product.Label.Ab);
            pain.PicturesSerialized= Path.Combine(Configurations.ProductsStockagePath, "pain.png");
            pain.Price = 4;
            pain.Producer = context.Producers.First();
            pain.ProductUnit = Product.Unit.Kg;
            pain.RemainingStock = 10;
            pain.State = Product.ProductState.Enabled;
            pain.Type = Product.SellType.Piece;
            pain.WeekStock = 10;
            pain.Familly = context.ProductFamillys.First(x => x.FamillyName == "Pains");
            context.Add(pain);
            Product tomate = new Product();
            tomate.Name = "Tomates grappe";
            tomate.Description = "";
            tomate.Labels.Add(Product.Label.Ab);
            tomate.PicturesSerialized = Path.Combine(Configurations.ProductsStockagePath, "tomate.jpg");
            tomate.Price = 2;
            tomate.Producer = context.Producers.First();
            tomate.ProductUnit = Product.Unit.Kg;
            tomate.RemainingStock = 10;
            tomate.Familly = context.ProductFamillys.First(x => x.FamillyName == "Légumes");
            tomate.State = Product.ProductState.Enabled;
            tomate.Type = Product.SellType.Weigh;
            tomate.WeekStock = 10;
            context.Add(tomate);
            Product pommedeterre = new Product();
            pommedeterre.Name = "Pomme de terre";
            pommedeterre.Description = "";
            pommedeterre.Labels.Add(Product.Label.Ab);
            pommedeterre.PicturesSerialized = Path.Combine(Configurations.ProductsStockagePath, "pommedeterre.jpg");
            pommedeterre.Price = 2;
            pommedeterre.Producer = context.Producers.First();
            pommedeterre.ProductUnit = Product.Unit.Kg;
            pommedeterre.RemainingStock = 10;
            pommedeterre.Familly = context.ProductFamillys.First(x => x.FamillyName == "Légumes");
            pommedeterre.State = Product.ProductState.Enabled;
            pommedeterre.Type = Product.SellType.Weigh;
            pommedeterre.WeekStock = 10;
            context.Add(pommedeterre);
            Product radis = new Product();
            radis.Name = "Radis";
            radis.Description = "Pain farine complete T80";
            radis.Labels.Add(Product.Label.Ab);
            radis.PicturesSerialized = Path.Combine(Configurations.ProductsStockagePath, "radis.jpg");
            radis.Price = 4;
            radis.Producer = context.Producers.First();
            radis.ProductUnit = Product.Unit.Kg;
            radis.RemainingStock = 10;
            radis.Familly = context.ProductFamillys.First(x => x.FamillyName == "Légumes");
            radis.State = Product.ProductState.Enabled;
            radis.Type = Product.SellType.Piece;
            radis.WeekStock = 10;
            context.Add(radis);
            Product salade = new Product();
            salade.Name = "Salade";
            salade.Description = "Pain farine complete T80";
            salade.Labels.Add(Product.Label.Ab);
            salade.PicturesSerialized = Path.Combine(Configurations.ProductsStockagePath, "salade.jpg");
            salade.Price = 1;
            salade.Producer = context.Producers.First();
            salade.ProductUnit = Product.Unit.Gr;
            salade.RemainingStock = 10;
            salade.Familly = context.ProductFamillys.First(x => x.FamillyName == "Légumes");
            salade.State = Product.ProductState.Enabled;
            salade.Type = Product.SellType.Piece;
            salade.WeekStock = 10;
            context.Add(salade);

            //
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
                    Configurations.UserType.Consumer);
            await CreateAcount(context,
                    userManager,
                    "MICHON",
                    "Nicolas",
                    "nicolas.michon@zoho.com",
                    "nicolas.michon@zoho.com",
                    Configurations.Role.Administrator,
                    Configurations.UserType.Consumer);
            await CreateAcount(context,
                    userManager,
                    "Maurice",
                    "Robert",
                    "producer@gmail.com",
                    "producer@gmail.com",
                    Configurations.Role.User,
                    Configurations.UserType.Producer);
        }
        private async Task CreateAcount(ApplicationDbContext context, UserManager<ApplicationUser> userManager, string name, string surname, string email, string password, Configurations.Role role, Configurations.UserType userType)
        {

            if (context.Consumers.Any(x => x.Email == email) || context.Producers.Any(x => x.Email == email))
                return;
            User user;
            switch(userType)
            {
                case Configurations.UserType.Producer:
                    user = new Producer();
                    break;
                case Configurations.UserType.Consumer:
                    user = new Consumer();
                    break;
                default:
                    user = new Consumer();
                    break;
            }                
            user.Name = name;
            user.Surname = surname;
            user.Email = email;
            user.Avatar = Path.Combine(Configurations.UserAvatarStockagePath, Configurations.DefaultFileName);
            user.RegistrationDate = DateTime.Now;
            user.Enable = true;

            switch (userType)
            {
                case Configurations.UserType.Producer:
                    Producer producer = user as Producer;
                    producer.CompanyName = "La ferme de " + producer.Name;
                    context.Producers.Add(producer);
                    break;
                case Configurations.UserType.Consumer:
                    context.Consumers.Add(user as Consumer);
                    break;
                default:
                    context.Consumers.Add(user as Consumer);
                    break;
            }


            #region Creating linked application data
            var appUser = new ApplicationUser { UserName = user.Email, Email = user.Email };
            appUser.User = user;

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
