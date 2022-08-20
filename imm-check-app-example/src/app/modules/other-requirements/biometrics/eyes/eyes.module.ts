import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { DisplayEyeInfoComponent } from './components/display-eye-info/display-eye-info.component';
import { DisplayEyesInfoComponent } from './components/display-eyes-info/display-eyes-info.component';



@NgModule({
  declarations: [
    DisplayEyeInfoComponent,
    DisplayEyesInfoComponent
  ],
  imports: [
    CommonModule
  ]
})
export class EyesModule { }
