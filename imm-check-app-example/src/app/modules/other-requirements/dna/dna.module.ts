import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { DisplayDnaInfoComponent } from './components/display-dna-info/display-dna-info.component';
import { DisplayDnaInfoListComponent } from './components/display-dna-info-list/display-dna-info-list.component';

@NgModule({
  declarations: [
    DisplayDnaInfoComponent,
    DisplayDnaInfoListComponent
  ],
  imports: [
    CommonModule
  ]
})

export class DnaModule { }
