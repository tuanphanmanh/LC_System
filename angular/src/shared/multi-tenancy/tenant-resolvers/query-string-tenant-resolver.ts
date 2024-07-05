import { UrlHelper } from '@shared/helpers/UrlHelper';

export class QueryStringTenantResolver {

    resolve(): string {
        let queryParams = UrlHelper.getQueryParameters();
        return queryParams['abp_tenancy_name'];
    }

}
