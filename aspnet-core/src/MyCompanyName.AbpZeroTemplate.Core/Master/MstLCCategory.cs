﻿using Abp.Domain.Entities.Auditing;
using Abp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCompanyName.AbpZeroTemplate.Master
{
    public class MstLCCategory : FullAuditedEntity<long>, IEntity<long>
    {
        public string Name { get; set; }
    }
}
