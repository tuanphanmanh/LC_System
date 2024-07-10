import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { MstCertificateComponent } from './mst-certificate.component';

const routes: Routes = [
    {
        path: '',
        component: MstCertificateComponent,
        pathMatch: 'full',
    },
];

@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule],
})
export class  MstCertificateRoutingModule {}
