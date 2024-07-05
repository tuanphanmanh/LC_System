import { NgModule } from '@angular/core';
import { AdminSharedModule } from '@app/admin/shared/admin-shared.module';
import { AppSharedModule } from '@app/shared/app-shared.module';
import { SubscriptionManagementRoutingModule } from './subscription-management-routing.module';
import { SubscriptionManagementComponent } from './subscription-management.component';
import { ShowDetailModalComponent } from './show-detail-modal.component';

@NgModule({
    declarations: [SubscriptionManagementComponent, ShowDetailModalComponent],
    imports: [AppSharedModule, AdminSharedModule, SubscriptionManagementRoutingModule],
})
export class SubscriptionManagementModule {}
