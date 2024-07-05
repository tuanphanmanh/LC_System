using Microsoft.EntityFrameworkCore;
using MyCompanyName.AbpZeroTemplate.OpenIddict.Applications;
using MyCompanyName.AbpZeroTemplate.OpenIddict.Authorizations;
using MyCompanyName.AbpZeroTemplate.OpenIddict.Scopes;
using MyCompanyName.AbpZeroTemplate.OpenIddict.Tokens;

namespace MyCompanyName.AbpZeroTemplate.EntityFrameworkCore
{
    public interface IOpenIddictDbContext
    {
        DbSet<OpenIddictApplication> Applications { get; }

        DbSet<OpenIddictAuthorization> Authorizations { get; }

        DbSet<OpenIddictScope> Scopes { get; }

        DbSet<OpenIddictToken> Tokens { get; }
    }

}