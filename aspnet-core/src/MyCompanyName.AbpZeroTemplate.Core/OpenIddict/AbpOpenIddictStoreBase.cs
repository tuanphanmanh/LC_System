using System;
using System.IO;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Threading.Tasks;
using Abp;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace MyCompanyName.AbpZeroTemplate.OpenIddict
{
    public abstract class AbpOpenIddictStoreBase<TRepository>
        where TRepository : IRepository
    {
        public ILogger<AbpOpenIddictStoreBase<TRepository>> Logger { get; set; }

        protected TRepository Repository { get; }
        protected IUnitOfWorkManager UnitOfWorkManager { get; }
        protected IGuidGenerator GuidGenerator { get; }
        protected AbpOpenIddictIdentifierConverter IdentifierConverter { get; }

        protected IOpenIddictDbConcurrencyExceptionHandler ConcurrencyExceptionHandler { get; }
        
        protected AbpOpenIddictStoreBase(
            TRepository repository, 
            IUnitOfWorkManager unitOfWorkManager,
            IGuidGenerator guidGenerator,
            IOpenIddictDbConcurrencyExceptionHandler concurrencyExceptionHandler)
        {
            Repository = repository;
            UnitOfWorkManager = unitOfWorkManager;
            GuidGenerator = guidGenerator;
            ConcurrencyExceptionHandler = concurrencyExceptionHandler;

            Logger = NullLogger<AbpOpenIddictStoreBase<TRepository>>.Instance;
        }

        protected virtual Guid ConvertIdentifierFromString(string identifier)
        {
            return IdentifierConverter.FromString(identifier);
        }

        protected virtual string WriteStream(Action<Utf8JsonWriter> action)
        {
            using (var stream = new MemoryStream())
            {
                using (var writer = new Utf8JsonWriter(stream, new JsonWriterOptions
                       {
                           Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                           Indented = false
                       }))
                {
                    action(writer);
                    writer.Flush();
                    return Encoding.UTF8.GetString(stream.ToArray());
                }
            }
        }

        protected virtual async Task<string> WriteStreamAsync(Func<Utf8JsonWriter, Task> func)
        {
            using (var stream = new MemoryStream())
            {
                using (var writer = new Utf8JsonWriter(stream, new JsonWriterOptions
                       {
                           Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                           Indented = false
                       }))
                {
                    await func(writer);
                    await writer.FlushAsync();
                    return Encoding.UTF8.GetString(stream.ToArray());
                }
            }
        }
    }
}