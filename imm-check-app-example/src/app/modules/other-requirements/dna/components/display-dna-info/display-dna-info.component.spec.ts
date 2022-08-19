import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DisplayDnaInfoComponent } from './display-dna-info.component';

describe('DisplayDnaInfoComponent', () => {
  let component: DisplayDnaInfoComponent;
  let fixture: ComponentFixture<DisplayDnaInfoComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ DisplayDnaInfoComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(DisplayDnaInfoComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
