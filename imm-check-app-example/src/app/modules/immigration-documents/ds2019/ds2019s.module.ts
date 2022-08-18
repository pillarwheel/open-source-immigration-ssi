import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HttpClientModule } from '@angular/common/http';

import { DisplayDs2019sComponent } from './display-ds2019s/display-ds2019s.component';
import { DisplayDs2019Component } from './display-ds2019/display-ds2019.component';

@NgModule({
  declarations: [
    DisplayDs2019sComponent,
    DisplayDs2019Component
  ],
  imports: [
    CommonModule,
    HttpClientModule
  ],
  exports: [
    DisplayDs2019sComponent
  ]
})
export class Ds2019sModule { }
