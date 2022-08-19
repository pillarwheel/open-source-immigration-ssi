import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { DisplayFingerprintInfoComponent } from './components/display-fingerprint-info/display-fingerprint-info.component';
import { DisplayFingerprintsInfoComponent } from './components/display-fingerprints-info/display-fingerprints-info.component';



@NgModule({
  declarations: [
    DisplayFingerprintInfoComponent,
    DisplayFingerprintsInfoComponent
  ],
  imports: [
    CommonModule
  ]
})
export class FingerprintsModule { }
