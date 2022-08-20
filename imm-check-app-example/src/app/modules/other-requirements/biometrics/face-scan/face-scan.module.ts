import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { DisplayFaceScanInfoComponent } from './components/display-face-scan-info/display-face-scan-info.component';
import { DisplayFaceScansInfoComponent } from './components/display-face-scans-info/display-face-scans-info.component';



@NgModule({
  declarations: [
    DisplayFaceScanInfoComponent,
    DisplayFaceScansInfoComponent
  ],
  imports: [
    CommonModule
  ]
})
export class FaceScanModule { }
