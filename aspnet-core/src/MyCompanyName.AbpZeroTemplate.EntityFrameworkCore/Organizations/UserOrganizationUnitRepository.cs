using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Abp;
using Abp.Authorization.Users;
using Abp.Collections.Extensions;
using Abp.EntityFrameworkCore;
using Abp.Organizations;
using Abp.UI;
using Castle.Core.Internal;
using Microsoft.EntityFrameworkCore;
using MyCompanyName.AbpZeroTemplate.EntityFrameworkCore;
using MyCompanyName.AbpZeroTemplate.EntityFrameworkCore.Repositories;

namespace MyCompanyName.AbpZeroTemplate.Organizations
{
    public class UserOrganizationUnitRepository : AbpZeroTemplateRepositoryBase<UserOrganizationUnit, long>,
        IUserOrganizationUnitRepository
    {
        public UserOrganizationUnitRepository(IDbContextProvider<AbpZeroTemplateDbContext> dbContextProvider)
            : base(dbContextProvider)
        {
        }

        public async Task<List<UserIdentifier>> GetAllUsersInOrganizationUnitHierarchical(long[] organizationUnitIds)
        {
            if (organizationUnitIds.IsNullOrEmpty())
            {
                return new List<UserIdentifier>();
            }

            var context = await GetContextAsync();

            var selectedOrganizationUnitCodes = await context.OrganizationUnits
                .Where(ou => organizationUnitIds.Contains(ou.Id))
                .ToListAsync();

            if (selectedOrganizationUnitCodes == null)
            {
                throw new UserFriendlyException("Can not find an organization unit");
            }

            var predicate = PredicateBuilder.New<OrganizationUnit>();

            foreach (var selectedOrganizationUnitCode in selectedOrganizationUnitCodes)
            {
                predicate = predicate.Or(ou => ou.Code.StartsWith(selectedOrganizationUnitCode.Code));
            }

            var userIdQueryHierarchical = await context.UserOrganizationUnits
                .Join(
                    context.OrganizationUnits.Where(predicate),
                    uo => uo.OrganizationUnitId,
                    ou => ou.Id,
                    (uo, ou) => new {uo.UserId, uo.TenantId}
                )
                .ToListAsync();

            return userIdQueryHierarchical
                .DistinctBy(x => x.UserId)
                .Select(ou => new UserIdentifier(ou.TenantId, ou.UserId))
                .ToList();
        }
    }
}