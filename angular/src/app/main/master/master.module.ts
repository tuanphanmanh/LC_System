import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MstCategoryComponent } from './mst-category/mst-category.component';

const tabcode_component_dict = {
  '/app/main/master/mst-category': MstCategoryComponent,

};

@NgModule({
  declarations: [
    MstCategoryComponent
  ],
  imports: [
    CommonModule
  ]
})
export class MasterModule { 
  static getComponent(tabCode: string) {
    return tabcode_component_dict[tabCode];
}
}
