import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { DisplayPassportsComponent } from './components/display-passports/display-passports.component';
import { DisplayPassportComponent } from './components/display-passport/display-passport.component';



@NgModule({
  declarations: [
    DisplayPassportsComponent,
    DisplayPassportComponent
  ],
  imports: [
    CommonModule
  ]
})
export class PassportModule { }
