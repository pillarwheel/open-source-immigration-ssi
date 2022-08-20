import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DisplayTranscriptsInfoComponent } from './display-transcripts-info.component';

describe('DisplayTranscriptsInfoComponent', () => {
  let component: DisplayTranscriptsInfoComponent;
  let fixture: ComponentFixture<DisplayTranscriptsInfoComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ DisplayTranscriptsInfoComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(DisplayTranscriptsInfoComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
