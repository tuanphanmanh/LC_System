import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { UpgradeSucceedComponent } from './upgrade-succeed.component';

const routes: Routes = [
    {
        path: '',
        component: UpgradeSucceedComponent,
        pathMatch: 'full',
    },
];

@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule],
})
export class UpgradeSucceedRoutingModule {}
