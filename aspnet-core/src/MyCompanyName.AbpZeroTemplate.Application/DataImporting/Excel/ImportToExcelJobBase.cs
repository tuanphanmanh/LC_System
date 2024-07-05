using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.BackgroundJobs;
using Abp.Dependency;
using Abp.Domain.Uow;
using Abp.Localization;
using Abp.UI;
using MyCompanyName.AbpZeroTemplate.Notifications;
using MyCompanyName.AbpZeroTemplate.Storage;

namespace MyCompanyName.AbpZeroTemplate.DataImporting.Excel;

public abstract class ImportToExcelJobBase<TEntityDto, TDataReader, TInvalidEntityExporter>(
    IAppNotifier appNotifier,
    IBinaryObjectManager binaryObjectManager,
    IUnitOfWorkManager unitOfWorkManager,
    TDataReader dataReader,
    TInvalidEntityExporter invalidEntityExporter)
    : AsyncBackgroundJob<ImportFromExcelJobArgs>, ITransientDependency
    where TEntityDto : ImportFromExcelDto
    where TDataReader : IExcelDataReader<TEntityDto>
    where TInvalidEntityExporter : IExcelInvalidEntityExporter<TEntityDto>
{
    public abstract string ErrorMessageKey { get; }

    public abstract string SuccessMessageKey { get; }

    public override async Task ExecuteAsync(ImportFromExcelJobArgs args)
    {
        var rows = await GetDataFromExcelOrNullAsync(args);
        if (rows == null || rows.Count == 0)
        {
            await SendInvalidExcelNotificationAsync(args);
            return;
        }

        await CreateEntitiesAsync(args, rows);
    }

    protected async Task<List<TEntityDto>> GetDataFromExcelOrNullAsync(ImportFromExcelJobArgs args)
    {
        using var uow = UnitOfWorkManager.Begin();
        using (CurrentUnitOfWork.SetTenantId(args.TenantId))
        {
            try
            {
                var file = await binaryObjectManager.GetOrNullAsync(args.BinaryObjectId);
                return dataReader.GetEntitiesFromExcel(file.Bytes);
            }
            catch (Exception)
            {
                return null;
            }
            finally
            {
                await uow.CompleteAsync();
            }
        }
    }

    protected async Task CreateEntitiesAsync(ImportFromExcelJobArgs args, List<TEntityDto> rows)
    {
        var invalidEntities = new List<TEntityDto>();

        foreach (var row in rows)
        {
            using var uow = unitOfWorkManager.Begin();

            using (CurrentUnitOfWork.SetTenantId(args.TenantId))
            {
                if (row.CanBeImported())
                {
                    try
                    {
                        await CreateEntityAsync(row);
                    }
                    catch (UserFriendlyException exception)
                    {
                        row.Exception = exception.Message;
                        invalidEntities.Add(row);
                    }
                    catch (Exception exception)
                    {
                        row.Exception = exception.ToString();
                        invalidEntities.Add(row);
                    }
                }
                else
                {
                    invalidEntities.Add(row);
                }
            }

            await uow.CompleteAsync();
        }

        using (var uow = unitOfWorkManager.Begin())
        {
            using (CurrentUnitOfWork.SetTenantId(args.TenantId))
            {
                await ProcessImportEntitiesResultAsync(args, invalidEntities);
            }

            await uow.CompleteAsync();
        }
    }

    protected abstract Task CreateEntityAsync(TEntityDto entity);

    protected async Task ProcessImportEntitiesResultAsync(ImportFromExcelJobArgs args,
        List<TEntityDto> invalidEntities)
    {
        if (invalidEntities.Count != 0)
        {
            var file = invalidEntityExporter.ExportToFile(invalidEntities);
            await appNotifier.SomeUsersCouldntBeImported(args.ExcelImporter, file.FileToken, file.FileType,
                file.FileName);
        }
        else
        {
            await appNotifier.SendMessageAsync(
                args.ExcelImporter,
                new LocalizableString(
                    SuccessMessageKey,
                    AbpZeroTemplateConsts.LocalizationSourceName
                ),
                null,
                Abp.Notifications.NotificationSeverity.Success
            );
        }
    }

    private async Task SendInvalidExcelNotificationAsync(ImportFromExcelJobArgs args)
    {
        using var uow = unitOfWorkManager.Begin();

        using (CurrentUnitOfWork.SetTenantId(args.TenantId))
        {
            await appNotifier.SendMessageAsync(
                args.ExcelImporter,
                new LocalizableString(
                    ErrorMessageKey,
                    AbpZeroTemplateConsts.LocalizationSourceName
                ),
                null,
                Abp.Notifications.NotificationSeverity.Warn
            );
        }

        await uow.CompleteAsync();
    }
}