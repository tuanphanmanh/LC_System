using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Abp.Authorization.Users;
using Abp.Domain.Uow;
using Abp.IdentityFramework;
using Abp.Extensions;
using Abp.ObjectMapping;
using Microsoft.AspNetCore.Identity;
using MyCompanyName.AbpZeroTemplate.Authorization.Roles;
using MyCompanyName.AbpZeroTemplate.Authorization.Users.Importing.Dto;
using MyCompanyName.AbpZeroTemplate.DataImporting.Excel;
using MyCompanyName.AbpZeroTemplate.Notifications;
using MyCompanyName.AbpZeroTemplate.Storage;

namespace MyCompanyName.AbpZeroTemplate.Authorization.Users.Importing;

public class ImportUsersToExcelJob(
    IObjectMapper objectMapper,
    IUserPolicy userPolicy,
    RoleManager roleManager,
    IEnumerable<IPasswordValidator<User>> passwordValidators,
    IPasswordHasher<User> passwordHasher,
    IUnitOfWorkManager unitOfWorkManager,
    UserListExcelDataReader dataReader,
    InvalidUserExporter invalidEntityExporter,
    IAppNotifier appNotifier,
    IBinaryObjectManager binaryObjectManager)
    : ImportToExcelJobBase<ImportUserDto, UserListExcelDataReader, InvalidUserExporter>(appNotifier,
        binaryObjectManager, unitOfWorkManager, dataReader, invalidEntityExporter)
{
    public UserManager UserManager { get; set; }

    public override string ErrorMessageKey => "FileCantBeConvertedToUserList";
    
    public override string SuccessMessageKey => "AllUsersSuccessfullyImportedFromExcel";

    protected override async Task CreateEntityAsync(ImportUserDto entity)
    {
        var tenantId = CurrentUnitOfWork.GetTenantId();

        if (tenantId.HasValue)
        {
            await userPolicy.CheckMaxUserCountAsync(tenantId.Value);
        }

        var user = objectMapper.Map<User>(entity); //Passwords is not mapped (see mapping configuration)
        user.Password = entity.Password;
        user.TenantId = tenantId;

        if (!entity.Password.IsNullOrEmpty())
        {
            await UserManager.InitializeOptionsAsync(tenantId);
            foreach (var validator in passwordValidators)
            {
                (await validator.ValidateAsync(UserManager, user, entity.Password)).CheckErrors();
            }

            user.Password = passwordHasher.HashPassword(user, entity.Password);
        }

        user.Roles = new List<UserRole>();
        var roleList = roleManager.Roles.ToList();

        foreach (var roleName in entity.AssignedRoleNames)
        {
            var correspondingRoleName = GetRoleNameFromDisplayName(roleName, roleList);
            var role = await roleManager.GetRoleByNameAsync(correspondingRoleName);
            user.Roles.Add(new UserRole(tenantId, user.Id, role.Id));
        }

        (await UserManager.CreateAsync(user)).CheckErrors();
    }

    private string GetRoleNameFromDisplayName(string displayName, List<Role> roleList)
    {
        return roleList.FirstOrDefault(
            r => r.DisplayName?.ToLowerInvariant() == displayName?.ToLowerInvariant()
        )?.Name;
    }
}