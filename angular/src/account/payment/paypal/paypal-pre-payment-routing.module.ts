import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { PayPalPrePaymentComponent } from './paypal-pre-payment.component';

const routes: Routes = [
    {
        path: '',
        component: PayPalPrePaymentComponent,
        pathMatch: 'full',
    },
];

@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule],
})
export class PaypalPrePaymentRoutingModule {}
