import { CUSTOM_ELEMENTS_SCHEMA, NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MstCategoryComponent } from './mst-category.component';
import { MstCategoryRoutingModule } from './mst-category-routing.module';
import { UtilsModule } from "../../../../shared/utils/utils.module";
import { AppComponent } from '@app/app.component';
import { BrowserModule } from '@angular/platform-browser';
import { FormsModule } from '@angular/forms';

@NgModule({
  declarations: [
    MstCategoryComponent
  ],
  imports: [
    CommonModule,
    MstCategoryRoutingModule,
    UtilsModule,
    BrowserModule,
    FormsModule  
],
   bootstrap: [AppComponent],
  schemas: [CUSTOM_ELEMENTS_SCHEMA]
})
export class MstCategoryModule { }
   