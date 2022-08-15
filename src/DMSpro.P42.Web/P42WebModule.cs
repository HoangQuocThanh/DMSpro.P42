using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using DMSpro.P42.Localization;
using DMSpro.P42.MultiTenancy;
using DMSpro.P42.Permissions;
using DMSpro.P42.Web.Menus;
using StackExchange.Redis;
using Volo.Abp.Caching.StackExchangeRedis;
using Microsoft.OpenApi.Models;
using Volo.Abp;
using Volo.Abp.Account.Admin.Web;
using Volo.Abp.Account.Public.Web.Impersonation;
using Volo.Abp.AspNetCore.Authentication.OpenIdConnect;
using Volo.Abp.AspNetCore.Mvc.Client;
using Volo.Abp.AspNetCore.Mvc.Localization;
using Volo.Abp.AspNetCore.Mvc.UI;
using Volo.Abp.AspNetCore.Mvc.UI.Bootstrap;
using Volo.Abp.AspNetCore.Mvc.UI.Bundling;
using Volo.Abp.AspNetCore.Mvc.UI.Theme.Commercial;
using Volo.Abp.AspNetCore.Mvc.UI.Theme.Lepton;
using Volo.Abp.AspNetCore.Mvc.UI.Theme.Lepton.Bundling;
using Volo.Abp.AspNetCore.Mvc.UI.Theme.Shared;
using Volo.Abp.AspNetCore.Mvc.UI.Theme.Shared.Toolbars;
using Volo.Abp.AspNetCore.Serilog;
using Volo.Abp.AuditLogging.Web;
using Volo.Abp.Autofac;
using Volo.Abp.AutoMapper;
using Volo.Abp.BackgroundJobs;
using Volo.Abp.Caching;
using Volo.Abp.FeatureManagement;
using Volo.Abp.Gdpr.Web;
using Volo.Abp.Http.Client.IdentityModel.Web;
using Volo.Abp.Identity.Web;
using Volo.Abp.Http.Client.Web;
using Volo.Abp.IdentityServer.Web;
using Volo.Abp.LanguageManagement;
using Volo.Abp.LeptonTheme.Management;
using Volo.Abp.Modularity;
using Volo.Abp.MultiTenancy;
using Volo.Abp.PermissionManagement.Web;
using Volo.Abp.Swashbuckle;
using Volo.Abp.TextTemplateManagement.Web;
using Volo.Abp.UI;
using Volo.Abp.UI.Navigation.Urls;
using Volo.Abp.UI.Navigation;
using Volo.Abp.VirtualFileSystem;
using Volo.Saas.Host;
using DMSpro.P42.SO.Web;
using DMSpro.P42.MDM.Web;

namespace DMSpro.P42.Web;

[DependsOn(
    typeof(P42HttpApiClientModule),
    typeof(P42HttpApiModule),
    typeof(AbpAccountPublicWebImpersonationModule),
    typeof(AbpAspNetCoreMvcClientModule),
    typeof(AbpHttpClientWebModule),
    typeof(AbpAutofacModule),
    typeof(AbpCachingStackExchangeRedisModule),
    typeof(AbpFeatureManagementWebModule),
    typeof(AbpAccountAdminWebModule),
    typeof(AbpHttpClientIdentityModelWebModule),
    typeof(AbpIdentityWebModule),
    typeof(AbpAuditLoggingWebModule),
    typeof(LeptonThemeManagementWebModule),
    typeof(SaasHostWebModule),
    typeof(AbpIdentityServerWebModule),
    typeof(AbpAspNetCoreMvcUiLeptonThemeModule),
    typeof(LanguageManagementWebModule),
    typeof(TextTemplateManagementWebModule),
    typeof(AbpGdprWebModule),
    typeof(AbpSwashbuckleModule),
    typeof(AbpAspNetCoreSerilogModule)
    )]
[DependsOn(typeof(SOWebModule))]
    [DependsOn(typeof(MDMWebModule))]
    public class P42WebModule : AbpModule
{
    public override void PreConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.PreConfigure<AbpMvcDataAnnotationsLocalizationOptions>(options =>
        {
            options.AddAssemblyResource(
                typeof(P42Resource),
                typeof(P42DomainSharedModule).Assembly,
                typeof(P42ApplicationContractsModule).Assembly,
                typeof(P42WebModule).Assembly
            );
        });
    }

    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        //You can disable this setting in production to avoid any potential security risks.
        Microsoft.IdentityModel.Logging.IdentityModelEventSource.ShowPII = true;

        var hostingEnvironment = context.Services.GetHostingEnvironment();
        var configuration = context.Services.GetConfiguration();

        ConfigureBundles();
        ConfigurePages(configuration);
        ConfigureCache(configuration);
        ConfigureDataProtection(context, configuration, hostingEnvironment);
        ConfigureUrls(configuration);
        ConfigureAuthentication(context, configuration);
        ConfigureImpersonation(context, configuration);
        ConfigureAutoMapper();
        ConfigureVirtualFileSystem(hostingEnvironment);
        ConfigureNavigationServices(configuration);
        ConfigureSwaggerServices(context.Services);
        ConfigureMultiTenancy();
        ConfigureBackgroundJobs();
    }

    private void ConfigureBackgroundJobs()
    {
        Configure<AbpBackgroundJobOptions>(options =>
        {
            options.IsJobExecutionEnabled = false;
        });
    }

    private void ConfigureBundles()
    {
        Configure<AbpBundlingOptions>(options =>
        {
            options.StyleBundles.Configure(
                LeptonThemeBundles.Styles.Global,
                bundle =>
                {
                    bundle.AddFiles("/global-styles.css");
                }
            );
        });
    }

    private void ConfigurePages(IConfiguration configuration)
    {
        Configure<RazorPagesOptions>(options =>
        {
            options.Conventions.AuthorizePage("/HostDashboard", P42Permissions.Dashboard.Host);
            options.Conventions.AuthorizePage("/TenantDashboard", P42Permissions.Dashboard.Tenant);
        });
    }

    private void ConfigureCache(IConfiguration configuration)
    {
        Configure<AbpDistributedCacheOptions>(options =>
        {
            options.KeyPrefix = "P42:";
        });
    }

    private void ConfigureUrls(IConfiguration configuration)
    {
        Configure<AppUrlOptions>(options =>
        {
            options.Applications["MVC"].RootUrl = configuration["App:SelfUrl"];
        });
    }

    private void ConfigureMultiTenancy()
    {
        Configure<AbpMultiTenancyOptions>(options => { options.IsEnabled = MultiTenancyConsts.IsEnabled; });
    }

    private void ConfigureAuthentication(ServiceConfigurationContext context, IConfiguration configuration)
    {
        context.Services.AddAuthentication(options =>
            {
                options.DefaultScheme = "Cookies";
                options.DefaultChallengeScheme = "oidc";
            })
            .AddCookie("Cookies", options =>
            {
                options.ExpireTimeSpan = TimeSpan.FromDays(365);
            })
            .AddAbpOpenIdConnect("oidc", options =>
            {
                options.Authority = configuration["AuthServer:Authority"];
                options.RequireHttpsMetadata = Convert.ToBoolean(configuration["AuthServer:RequireHttpsMetadata"]);
                options.ResponseType = OpenIdConnectResponseType.CodeIdToken;

                options.ClientId = configuration["AuthServer:ClientId"];
                options.ClientSecret = configuration["AuthServer:ClientSecret"];

                options.SaveTokens = true;
                options.GetClaimsFromUserInfoEndpoint = true;

                options.Scope.Add("role");
                options.Scope.Add("email");
                options.Scope.Add("phone");
                options.Scope.Add("P42");
            });
            /*
            * This configuration is used when the identityServer is running on docker containers at localhost.
            * Configuring the redirectin URLs for internal network and the web
            */
            if (Convert.ToBoolean(configuration["AuthServer:IsContainerizedOnLocalhost"]))
            {
                context.Services.Configure<OpenIdConnectOptions>("oidc", options =>
                {
                    options.MetadataAddress = configuration["AuthServer:MetaAddress"].EnsureEndsWith('/') +
                                            ".well-known/openid-configuration";

                    var previousOnRedirectToIdentityProvider = options.Events.OnRedirectToIdentityProvider;
                    options.Events.OnRedirectToIdentityProvider = async ctx =>
                    {
                        // Intercept the redirection so the browser navigates to the right URL in your host
                        ctx.ProtocolMessage.IssuerAddress = configuration["AuthServer:Authority"].EnsureEndsWith('/') + "connect/authorize";

                        if (previousOnRedirectToIdentityProvider != null)
                        {
                            await previousOnRedirectToIdentityProvider(ctx);
                        }
                    };
                    var previousOnRedirectToIdentityProviderForSignOut = options.Events.OnRedirectToIdentityProviderForSignOut;
                    options.Events.OnRedirectToIdentityProviderForSignOut = async ctx =>
                    {
                        // Intercept the redirection for signout so the browser navigates to the right URL in your host
                        ctx.ProtocolMessage.IssuerAddress = configuration["AuthServer:Authority"].EnsureEndsWith('/') + "connect/endsession";

                        if (previousOnRedirectToIdentityProviderForSignOut != null)
                        {
                            await previousOnRedirectToIdentityProviderForSignOut(ctx);
                        }
                    };
                });

            }
    }

    private void ConfigureImpersonation(ServiceConfigurationContext context, IConfiguration configuration)
    {
        context.Services.Configure<AbpSaasHostWebOptions>(options =>
        {
            options.EnableTenantImpersonation = true;
        });
        context.Services.Configure<AbpIdentityWebOptions>(options =>
        {
            options.EnableUserImpersonation = true;
        });
    }

    private void ConfigureAutoMapper()
    {
        Configure<AbpAutoMapperOptions>(options =>
        {
            options.AddMaps<P42WebModule>();
        });
    }

    private void ConfigureVirtualFileSystem(IWebHostEnvironment hostingEnvironment)
    {
        Configure<AbpVirtualFileSystemOptions>(options =>
        {
            options.FileSets.AddEmbedded<P42WebModule>();

            if (hostingEnvironment.IsDevelopment())
            {
                options.FileSets.ReplaceEmbeddedByPhysical<P42DomainSharedModule>(Path.Combine(hostingEnvironment.ContentRootPath, string.Format("..{0}DMSpro.P42.Domain.Shared", Path.DirectorySeparatorChar)));
                options.FileSets.ReplaceEmbeddedByPhysical<P42ApplicationContractsModule>(Path.Combine(hostingEnvironment.ContentRootPath, string.Format("..{0}DMSpro.P42.Application.Contracts", Path.DirectorySeparatorChar)));
                options.FileSets.ReplaceEmbeddedByPhysical<P42WebModule>(hostingEnvironment.ContentRootPath);
            }
        });
    }

    private void ConfigureNavigationServices(IConfiguration configuration)
    {
        Configure<AbpNavigationOptions>(options =>
        {
            options.MenuContributors.Add(new P42MenuContributor(configuration));
        });

        Configure<AbpToolbarOptions>(options =>
        {
            options.Contributors.Add(new P42ToolbarContributor());
        });
    }

    private void ConfigureSwaggerServices(IServiceCollection services)
    {
        services.AddAbpSwaggerGen(
            options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo { Title = "P42 API", Version = "v1" });
                options.DocInclusionPredicate((docName, description) => true);
                options.CustomSchemaIds(type => type.FullName);
            }
        );
    }

    private void ConfigureDataProtection(
        ServiceConfigurationContext context,
        IConfiguration configuration,
        IWebHostEnvironment hostingEnvironment)
    {
        var dataProtectionBuilder = context.Services.AddDataProtection().SetApplicationName("P42");
        if (!hostingEnvironment.IsDevelopment())
        {
            var redis = ConnectionMultiplexer.Connect(configuration["Redis:Configuration"]);
            dataProtectionBuilder.PersistKeysToStackExchangeRedis(redis, "P42-Protection-Keys");
        }
    }

    public override void OnApplicationInitialization(ApplicationInitializationContext context)
    {
        var app = context.GetApplicationBuilder();
        var env = context.GetEnvironment();

        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        app.UseAbpRequestLocalization();

        if (!env.IsDevelopment())
        {
            app.UseErrorPage();
        }

        app.UseAbpSecurityHeaders();
        app.UseStaticFiles();
        app.UseRouting();
        app.UseAuthentication();

        if (MultiTenancyConsts.IsEnabled)
        {
            app.UseMultiTenancy();
        }

        app.UseAuthorization();
        app.UseSwagger();
        app.UseAbpSwaggerUI(options =>
        {
            options.SwaggerEndpoint("/swagger/v1/swagger.json", "P42 API");
        });
        app.UseAbpSerilogEnrichers();
        app.UseConfiguredEndpoints();
    }
}
