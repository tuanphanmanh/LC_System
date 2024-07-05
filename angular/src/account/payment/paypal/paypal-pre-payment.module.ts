import { NgModule } from '@angular/core';
import { AppSharedModule } from '@app/shared/app-shared.module';
import { PaypalPrePaymentRoutingModule } from './paypal-pre-payment-routing.module';
import { AccountSharedModule } from '@account/shared/account-shared.module';
import { PayPalPrePaymentComponent } from './paypal-pre-payment.component';

@NgModule({
    declarations: [PayPalPrePaymentComponent],
    imports: [AppSharedModule, AccountSharedModule, PaypalPrePaymentRoutingModule],
})
export class PaypalPrePaymentModule {}
