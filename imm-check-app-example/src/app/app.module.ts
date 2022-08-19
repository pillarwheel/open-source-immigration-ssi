// LOAD NG MODULES
import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { AppRoutingModule } from './app-routing.module';
import { HttpClientModule, HttpClient, HttpHeaders } from '@angular/common/http';
// LOAD OUR MODULES
import { Ds2019Module } from './modules/immigration-documents/ds2019/ds2019.module';
import { I20Module } from './modules/immigration-documents/i20/i20.module';
import { I94Module } from './modules/immigration-documents/i94/i94.module';
import { PassportModule } from './modules/immigration-documents/passport/passport.module';
import { SponsoredStudentInformationModule } from './modules/immigration-documents/sponsored-student-information/sponsored-student-information.module';
import { VisaModule } from './modules/immigration-documents/visa/visa.module';
import { VisaStampModule } from './modules/immigration-documents/visa-stamp/visa-stamp.module';
import { EyesModule } from './modules/other-requirements/biometrics/eyes/eyes.module';
import { FaceScanModule } from './modules/other-requirements/biometrics/face-scan/face-scan.module';
import { FingerprintsModule } from './modules/other-requirements/biometrics/fingerprints/fingerprints.module';
import { DnaModule } from './modules/other-requirements/dna/dna.module';
import { TranscriptsModule } from './modules/other-requirements/transcripts/transcripts.module';
// LOAD NG COMPONENTS
import { AppComponent } from './app.component';
// LOAD OUR COMPONENTS
import { DisplayDs2019sComponent } from './modules/immigration-documents/ds2019/components/display-ds2019s/display-ds2019s.component';
import { DisplayI20sComponent } from './modules/immigration-documents/i20/components/display-i20s/display-i20s.component';
import { DisplayI94sComponent } from './modules/immigration-documents/i94/components/display-i94s/display-i94s.component';
import { DisplayPassportsComponent } from './modules/immigration-documents/passport/components/display-passports/display-passports.component';
import { DisplaySponsoredStudentsComponent } from './modules/immigration-documents/sponsored-student-information/components/display-sponsored-students/display-sponsored-students.component';
import { DisplayVisasComponent } from './modules/immigration-documents/visa/components/display-visas/display-visas.component';
import { DisplayVisaStampsComponent } from './modules/immigration-documents/visa-stamp/components/display-visa-stamps/display-visa-stamps.component';



@NgModule({
  declarations: [
    AppComponent,
    DisplayI94sComponent
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    HttpClientModule,
    Ds2019Module,
    I20Module,
    I94Module,
    PassportModule,
    SponsoredStudentInformationModule,
    VisaStampModule,
    VisaModule,
    EyesModule,
    FaceScanModule,
    FingerprintsModule,
    DnaModule,
    TranscriptsModule,
//    DisplayDs2019sComponent,
//    DisplayI20sComponent,
//    DisplayPassportsComponent,
//    DisplaySponsoredStudentsComponent,
//    DisplayVisasComponent,
//    DisplayVisaStampsComponent
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }
