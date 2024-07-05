using System.Threading.Tasks;
using Abp.Domain.Uow;

namespace MyCompanyName.AbpZeroTemplate.OpenIddict
{
    public interface IOpenIddictDbConcurrencyExceptionHandler
    {
        Task HandleAsync(AbpDbConcurrencyException exception);
    }
}