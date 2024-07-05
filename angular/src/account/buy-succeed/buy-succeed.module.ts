import { NgModule } from '@angular/core';
import { AppSharedModule } from '@app/shared/app-shared.module';
import { BuySucceedRoutingModule } from './buy-succeed-routing.module';
import { AccountSharedModule } from '@account/shared/account-shared.module';
import { BuySucceedComponent } from './buy-succeed.component';

@NgModule({
    declarations: [BuySucceedComponent],
    imports: [AppSharedModule, AccountSharedModule, BuySucceedRoutingModule],
})
export class BuySucceedModule {}
