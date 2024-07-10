import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MstCertificateComponent } from './mst-certificate.component';
import { MstCertificateRoutingModule } from './mst-certificate-routing.module';
import { UtilsModule } from '@shared/utils/utils.module';

@NgModule({
  declarations: [
    MstCertificateComponent
  ],
  imports: [
    CommonModule,
    MstCertificateRoutingModule,
    UtilsModule
]
})
export class MstCertificateModule { }
   
 



 
   