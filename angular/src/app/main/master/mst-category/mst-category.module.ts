import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MstCategoryComponent } from './mst-category.component';
import { MstCategoryRoutingModule } from './mst-category-routing.module';
import { UtilsModule } from "../../../../shared/utils/utils.module";

@NgModule({
  declarations: [
    MstCategoryComponent
  ],
  imports: [
    CommonModule,
    MstCategoryRoutingModule,
    UtilsModule
]
})
export class MstCategoryModule { }
   