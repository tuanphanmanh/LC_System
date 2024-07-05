import { NgModule } from '@angular/core';
import { AppSharedModule } from '@app/shared/app-shared.module';
import { ExtendSucceedRoutingModule } from './extend-succeed-routing.module';
import { ExtendSucceedComponent } from './extend-succeed.component';
import { AccountSharedModule } from '@account/shared/account-shared.module';

@NgModule({
    declarations: [ExtendSucceedComponent],
    imports: [AppSharedModule, AccountSharedModule, ExtendSucceedRoutingModule],
})
export class ExtendSucceedModule {}
