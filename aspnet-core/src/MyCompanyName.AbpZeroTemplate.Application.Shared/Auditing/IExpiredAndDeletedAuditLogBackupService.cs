using System.Collections.Generic;
using Abp.Auditing;

namespace MyCompanyName.AbpZeroTemplate.Auditing
{
    public interface IExpiredAndDeletedAuditLogBackupService
    {
        bool CanBackup();
        
        void Backup(List<AuditLog> auditLogs);
    }
}