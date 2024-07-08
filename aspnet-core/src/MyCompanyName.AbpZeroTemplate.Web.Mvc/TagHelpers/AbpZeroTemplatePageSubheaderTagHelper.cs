using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Abp.Extensions;
using Microsoft.AspNetCore.Razor.TagHelpers;
using MyCompanyName.AbpZeroTemplate.UiCustomization;
using MyCompanyName.AbpZeroTemplate.UiCustomization.Dto;

namespace MyCompanyName.AbpZeroTemplate.Web.TagHelpers
{
    public class BreadcrumbItem
    {
        public string Url { get; set; }
        public string Text { get; set; }

        public BreadcrumbItem(string url, string text)
        {
            Url = url;
            Text = text;
        }

        public BreadcrumbItem(string text)
        {
            Text = text;
        }
    }

    [HtmlTargetElement("abp-page-subheader")]
    public class AbpZeroTemplatePageSubheaderTagHelper : TagHelper
    {
        private readonly IUiThemeCustomizerFactory _uiThemeCustomizerFactory;

        public string Description { get; set; }

        public string Title { get; set; }

        public List<BreadcrumbItem> Breadcrumbs { get; set; }

        public AbpZeroTemplatePageSubheaderTagHelper(IUiThemeCustomizerFactory uiThemeCustomizerFactory)
        {
            _uiThemeCustomizerFactory = uiThemeCustomizerFactory;
        }

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            var currentTheme = await _uiThemeCustomizerFactory.GetCurrentUiCustomizer();
            var settings = await currentTheme.GetUiSettings();

            var tagContent = await output.GetChildContentAsync();

            var template = GetTemplate(settings, tagContent.GetContent());
            output.Content.AppendHtml(template);
        }

        private string GetTemplate(UiCustomizationSettingsDto settings, string content)
        {
            var subContainerStyle = GetContainerStyle(settings);

            return
                $@"<div class='{settings.BaseSettings.SubHeader.ContainerStyle}' id='kt_app_toolbar'>
                        <div id='kt_app_toolbar_container' class='{subContainerStyle} app-container d-flex flex-stack {settings.BaseSettings.SubHeader.SubContainerStyle}'>
                            <!--begin::Info-->
                            {GetInfoArea(settings)}
                            <!--end::Info-->

                            <!--begin::Toolbar-->
                            <div class='d-flex align-items-center'>
                               {content}
                            </div>
                            <!--end::Toolbar-->
                        </div>
                    </div>
                    ";
        }

        private string GetContainerStyle(UiCustomizationSettingsDto settings)
        {
            if (settings.BaseSettings.Layout.LayoutType == "fluid")
            {
                return "container-fluid";
            }

            return settings.BaseSettings.Layout.LayoutType.IsIn("fixed", "fluid-xxl")
                ? "container-xxl"
                : "container";
        }

        private string GetInfoArea(UiCustomizationSettingsDto settings)
        {
            return $@"<div class='page-title d-flex flex-column justify-content-center flex-wrap me-3'>
                        <!--begin::Page Title-->
                        <h{settings.BaseSettings.SubHeader.SubheaderSize} class='{settings.BaseSettings.SubHeader.TitleStyle}'>
                            {Title}
                        </h{settings.BaseSettings.SubHeader.SubheaderSize}>
                        <!--end::Page Title-->
                        {GetDescription()}
                        {GetBreadcrumbs()}
                    </div>";
        }

        private string GetDescription()
        {
            if (Description.IsNullOrWhiteSpace())
            {
                return "";
            }

            return $@"<span class='text-muted me-4'>{Description}</span>";
        }

        private string GetBreadcrumbs()
        {
            if (Breadcrumbs == null || Breadcrumbs.Count == 0)
            {
                return "";
            }

            StringBuilder sb = new StringBuilder();
            sb.Append($@"<!--begin::Breadcrumb-->
		                <ul class='breadcrumb breadcrumb-transparent breadcrumb-dot fw-bold p-0 my-2 fs-sm'>");
            foreach (var breadcrumbItem in Breadcrumbs)
            {
                sb.Append("<li class='breadcrumb-item'>");
                if (breadcrumbItem.Url.IsNullOrWhiteSpace())
                {
                    sb.Append($@"<span class='text-muted'>
					                {breadcrumbItem.Text}	                        	
				                </span>");
                }
                else
                {
                    sb.Append($@"<a href='{breadcrumbItem.Url}'>
					                {breadcrumbItem.Text}	                        	
				                </a>");
                }

                sb.Append("</li>");
            }

            sb.Append("</ul>" +
                      "<!--end::Breadcrumb-->");

            return sb.ToString();
        }
    }
}