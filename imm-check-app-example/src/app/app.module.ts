import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { PassportComponent } from './modules/immigration-documents/passport/passport.component';
import { VisaComponent } from './modules/immigration-documents/visa/visa.component';
import { VisaStampComponent } from './modules/immigration-documents/visa-stamp/visa-stamp.component';
// import { Ds2019Component } from './modules/immigration-documents/ds2019/ds2019.component';
import { I20Component } from './modules/immigration-documents/i20/i20.component';
import { I94Component } from './modules/immigration-documents/i94/i94.component';
import { SponsoredStudentsComponent } from './modules/immigration-documents/sponsored-students/sponsored-students.component';
import { EyesComponent } from './modules/other-requirements/biometrics/eyes/eyes.component';
import { FingerprintsComponent } from './modules/other-requirements/biometrics/fingerprints/fingerprints.component';
import { FaceScanComponent } from './modules/other-requirements/biometrics/face-scan/face-scan.component';
import { DnaComponent } from './modules/other-requirements/dna/dna.component';
import { DisplayDs2019Component } from './modules/immigration-documents/ds2019s/components/display-ds2019/display-ds2019.component';

@NgModule({
  declarations: [
    AppComponent,
    PassportComponent,
    VisaComponent,
    VisaStampComponent,
    I20Component,
    I94Component,
    SponsoredStudentsComponent,
    EyesComponent,
    FingerprintsComponent,
    FaceScanComponent,
    DnaComponent,
    DisplayDs2019Component
  ],
  imports: [
    BrowserModule,
    AppRoutingModule
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }
