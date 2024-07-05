import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { BuySucceedComponent } from './buy-succeed.component';

const routes: Routes = [
    {
        path: '',
        component: BuySucceedComponent,
        pathMatch: 'full',
    },
];

@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule],
})
export class BuySucceedRoutingModule {}
