import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { CommonModule } from '@angular/common';
//IMPORT OUR COMPONENTS
import { DisplayDs2019sComponent } from './modules/immigration-documents/ds2019/components/display-ds2019s/display-ds2019s.component';
import { DisplayDs2019Component } from './modules/immigration-documents/ds2019/components/display-ds2019/display-ds2019.component';
import { DisplayI20Component } from './modules/immigration-documents/i20/components/display-i20/display-i20.component';
import { DisplayI20sComponent } from './modules/immigration-documents/i20/components/display-i20s/display-i20s.component';
import { DisplayI94Component } from './modules/immigration-documents/i94/components/display-i94/display-i94.component';
import { DisplayI94sComponent } from './modules/immigration-documents/i94/components/display-i94s/display-i94s.component';
import { DisplayPassportComponent } from './modules/immigration-documents/passport/components/display-passport/display-passport.component';
import { DisplayPassportsComponent } from './modules/immigration-documents/passport/components/display-passports/display-passports.component';
import { DisplaySponsoredStudentsComponent } from './modules/immigration-documents/sponsored-student-information/components/display-sponsored-students/display-sponsored-students.component';
import { DisplaySponsoredStudentComponent } from './modules/immigration-documents/sponsored-student-information/components/display-sponsored-student/display-sponsored-student.component';
import { DisplayVisasComponent } from './modules/immigration-documents/visa/components/display-visas/display-visas.component';
import { DisplayVisaComponent } from './modules/immigration-documents/visa/components/display-visa/display-visa.component';
import { DisplayVisaStampsComponent } from './modules/immigration-documents/visa-stamp/components/display-visa-stamps/display-visa-stamps.component';
import { DisplayVisaStampComponent } from './modules/immigration-documents/visa-stamp/components/display-visa-stamp/display-visa-stamp.component';

const routes: Routes = [
  {
    path: 'ds2019',
    component: DisplayDs2019Component
  },
  {
    path: 'ds2019s',
    component: DisplayDs2019sComponent
  },
  {
    path: 'i20',
    component: DisplayI20Component
  },
  {
    path: 'i20s',
    component: DisplayI20sComponent
  },
  {
    path: 'i94',
    component: DisplayI94Component
  },
  {
    path: 'i94s',
    component: DisplayI94sComponent
  },
  {
    path: 'passport',
    component: DisplayPassportComponent
  },
  {
    path: 'passports',
    component: DisplayPassportsComponent
  },
  {
    path: 'sponsoredstudent',
    component: DisplaySponsoredStudentComponent
  },
  {
    path: 'sponsoredstudents',
    component: DisplaySponsoredStudentsComponent
  },
  {
    path: 'visa',
    component: DisplayVisaComponent
  },
  {
    path: 'visas',
    component: DisplayVisasComponent
  },
  {
    path: 'visa-stamp',
    component: DisplayVisaStampComponent
  },
  {
    path: 'visa-stamps',
    component: DisplayVisaStampsComponent
  }
];

@NgModule({
  imports: [
    CommonModule,
    RouterModule.forRoot(routes)
  ],
  exports: [RouterModule],
  declarations: []
})
export class AppRoutingModule { }
