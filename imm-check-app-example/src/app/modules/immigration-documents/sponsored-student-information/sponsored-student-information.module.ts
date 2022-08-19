import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { DisplaySponsoredStudentsComponent } from './components/display-sponsored-students/display-sponsored-students.component';
import { DisplaySponsoredStudentComponent } from './components/display-sponsored-student/display-sponsored-student.component';



@NgModule({
  declarations: [
    DisplaySponsoredStudentsComponent,
    DisplaySponsoredStudentComponent
  ],
  imports: [
    CommonModule
  ]
})
export class SponsoredStudentInformationModule { }
