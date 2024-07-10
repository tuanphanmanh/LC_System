import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { MstCategoryComponent } from './mst-category.component';

const routes: Routes = [
    {
        path: '',
        component: MstCategoryComponent,
        pathMatch: 'full',
    },
];

@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule],
})
export class  MstCategoryRoutingModule {}
