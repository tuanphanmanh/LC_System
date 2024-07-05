import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { ExtendSucceedComponent } from './extend-succeed.component';

const routes: Routes = [
    {
        path: '',
        component: ExtendSucceedComponent,
        pathMatch: 'full',
    },
];

@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule],
})
export class ExtendSucceedRoutingModule {}
