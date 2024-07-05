import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { GatewaySelectionComponent } from './gateway-selection.component';

const routes: Routes = [
    {
        path: '',
        component: GatewaySelectionComponent,
        pathMatch: 'full',
    },
];

@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule],
})
export class BuyRoutingModule {}
