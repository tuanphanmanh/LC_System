import { NgModule } from '@angular/core';
import { AppSharedModule } from '@app/shared/app-shared.module';
import { StripePostPaymentRoutingModule } from './stripe-post-payment-routing.module';
import { AccountSharedModule } from '@account/shared/account-shared.module';
import { StripePostPaymentComponent } from './stripe-post-payment.component';

@NgModule({
    declarations: [StripePostPaymentComponent],
    imports: [AppSharedModule, AccountSharedModule, StripePostPaymentRoutingModule],
})
export class StripePostPaymentModule {}
