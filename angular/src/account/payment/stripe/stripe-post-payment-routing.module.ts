import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { StripePostPaymentComponent } from './stripe-post-payment.component';

const routes: Routes = [
    {
        path: '',
        component: StripePostPaymentComponent,
        pathMatch: 'full',
    },
];

@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule],
})
export class StripePostPaymentRoutingModule {}
