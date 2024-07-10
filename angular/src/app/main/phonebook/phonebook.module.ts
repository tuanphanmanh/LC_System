import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { PhoneBookComponent } from './phonebook.component';
import { AppSharedModule } from '@app/shared/app-shared.module';
import { PhoneBookRoutingModule } from './phonebook-routing.module';




@NgModule({
  declarations: [PhoneBookComponent],
  imports: [
    AppSharedModule, PhoneBookRoutingModule
  ]
})
export class PhonebookModule { }
