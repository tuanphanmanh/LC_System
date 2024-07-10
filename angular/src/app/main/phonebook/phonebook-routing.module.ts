import {NgModule} from '@angular/core';
import {RouterModule, Routes} from '@angular/router';
import { PhoneBookComponent } from './phonebook.component';



const routes: Routes = [{
    path: '',
    component: PhoneBookComponent,
    pathMatch: 'full'
}];

@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule],
})
export class PhoneBookRoutingModule {}
