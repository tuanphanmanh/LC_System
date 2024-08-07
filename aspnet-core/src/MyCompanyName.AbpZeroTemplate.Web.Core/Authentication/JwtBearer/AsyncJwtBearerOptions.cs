﻿using System.Collections.Generic;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace MyCompanyName.AbpZeroTemplate.Web.Authentication.JwtBearer
{
    public class AsyncJwtBearerOptions : JwtBearerOptions
    {
        public readonly List<IAsyncSecurityTokenValidator> AsyncSecurityTokenValidators;
        
        private readonly AbpZeroTemplateAsyncJwtSecurityTokenHandler _defaultAsyncHandler = new AbpZeroTemplateAsyncJwtSecurityTokenHandler();

        public AsyncJwtBearerOptions()
        {
            AsyncSecurityTokenValidators = new List<IAsyncSecurityTokenValidator>() {_defaultAsyncHandler};
        }
    }

}
