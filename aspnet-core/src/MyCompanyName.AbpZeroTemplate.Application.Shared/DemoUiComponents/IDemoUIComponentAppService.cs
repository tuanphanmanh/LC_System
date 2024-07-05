using System;
using System.Collections.Generic;
using Abp;
using Abp.Application.Services;
using MyCompanyName.AbpZeroTemplate.DemoUiComponents.Dto;

namespace MyCompanyName.AbpZeroTemplate.DemoUiComponents
{
    public interface IDemoUiComponentsAppService: IApplicationService
    {
        DateFieldOutput SendAndGetDate(DateTime date);

        DateFieldOutput SendAndGetDateTime(DateTime date);

        DateRangeFieldOutput SendAndGetDateRange(DateTime startDate, DateTime endDate);

        DateWithTextFieldOutput SendAndGetDateWithText(SendAndGetDateWithTextInput input);

        List<NameValue<string>> GetCountries(string searchTerm);

        List<NameValue<string>> SendAndGetSelectedCountries(List<NameValue<string>> selectedCountries);

        StringOutput SendAndGetValue(string input);
    }
}