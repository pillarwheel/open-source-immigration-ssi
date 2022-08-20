import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { DisplayVisaComponent } from './components/display-visa/display-visa.component';
import { DisplayVisasComponent } from './components/display-visas/display-visas.component';



@NgModule({
  declarations: [
    DisplayVisaComponent,
    DisplayVisasComponent
  ],
  imports: [
    CommonModule
  ]
})
export class VisaModule { }
