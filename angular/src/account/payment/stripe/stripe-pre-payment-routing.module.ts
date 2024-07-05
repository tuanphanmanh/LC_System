import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { StripePrePaymentComponent } from './stripe-pre-payment.component';

const routes: Routes = [
    {
        path: '',
        component: StripePrePaymentComponent,
        pathMatch: 'full',
    },
];

@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule],
})
export class StripePrePaymentRoutingModule {}
