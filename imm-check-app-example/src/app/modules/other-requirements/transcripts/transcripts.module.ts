import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { DisplayTranscriptsInfoComponent } from './components/display-transcripts-info/display-transcripts-info.component';
import { DisplayTranscriptInfoComponent } from './components/display-transcript-info/display-transcript-info.component';



@NgModule({
  declarations: [
    DisplayTranscriptsInfoComponent,
    DisplayTranscriptInfoComponent
  ],
  imports: [
    CommonModule
  ]
})
export class TranscriptsModule { }
