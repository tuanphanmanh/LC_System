import { NgModule } from '@angular/core';
import { RouterModule } from '@angular/router';

@NgModule({
    imports: [
        RouterModule.forChild([
            {
                path: '',
                children: [
                    {
                        path: 'dashboard',
                        loadChildren: () => import('./dashboard/dashboard.module').then((m) => m.DashboardModule),
                        data: { permission: 'Pages.Tenant.Dashboard' },
                    },
                    // { path: '', redirectTo: 'dashboard', pathMatch: 'full' },   
                    // { path: '**', redirectTo: 'dashboard' },
                    
                    //master
                    {
                        path: 'master/mst-category',
                        loadChildren: () => import('./master/mst-category/mst-category.module').then((m) => m.MstCategoryModule),
                        data: { permission: 'MasterData.MasterCategory' },
                    },

                    {
                        path: 'phonebook',
                        loadChildren: () => import('./phonebook/phonebook.module').then((m) => m.PhonebookModule),
                    },

                    
                    {
                        path: 'master/mst-certificate',
                        loadChildren: () => import('./master/mst-certificate/mst-certificate.module').then((m) => m.MstCertificateModule),
                        data: { permission: 'MasterData.MasterCertificate' },
                    },
 

                ],
            },
        ]),
    ],
    exports: [RouterModule],
})
export class MainRoutingModule {}


