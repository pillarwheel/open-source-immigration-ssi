import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { DisplayVisaStampComponent } from './components/display-visa-stamp/display-visa-stamp.component';
import { DisplayVisaStampsComponent } from './components/display-visa-stamps/display-visa-stamps.component';



@NgModule({
  declarations: [
    DisplayVisaStampComponent,
    DisplayVisaStampsComponent
  ],
  imports: [
    CommonModule
  ]
})
export class VisaStampModule { }
