using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.DependencyInjection;

namespace Volo.Abp.Account.Web.Pages.Account;

public class IdentityServerReturnUrlRequestCultureProvider : RequestCultureProvider
{
    public readonly string ReturnUrl = "ReturnUrl";

    public readonly string QueryStringKey = "culture";

    public readonly string UIQueryStringKey = "ui-culture";

    public override async Task<ProviderCultureResult> DetermineProviderCultureResult(HttpContext httpContext)
    {
        if (httpContext == null)
        {
            throw new ArgumentNullException(nameof(httpContext));
        }

        var request = httpContext.Request;
        if (!request.QueryString.HasValue)
        {
            return await NullProviderCultureResult;
        }

        string returnUrl = request.Query[ReturnUrl];
        if (returnUrl.IsNullOrWhiteSpace())
        {
            return await NullProviderCultureResult;
        }

        var interaction = httpContext.RequestServices.GetService<IIdentityServerInteractionService>();
        if (interaction == null)
        {
            return await NullProviderCultureResult;
        }

        var context = await interaction.GetAuthorizationContextAsync(returnUrl);
        if (context == null)
        {
            return await NullProviderCultureResult;
        }

        var queryCulture = context.Parameters.Get(QueryStringKey);
        var queryUICulture = context.Parameters.Get(UIQueryStringKey);

        if (queryCulture == null && queryUICulture == null)
        {
            // No values specified for either so no match
            return await NullProviderCultureResult;
        }

        if (queryCulture != null && queryUICulture == null)
        {
            // Value for culture but not for UI culture so default to culture value for both
            queryUICulture = queryCulture;
        }
        else if (queryCulture == null && queryUICulture != null)
        {
            // Value for UI culture but not for culture so default to UI culture value for both
            queryCulture = queryUICulture;
        }

        return new ProviderCultureResult(queryCulture, queryUICulture);
    }
}
