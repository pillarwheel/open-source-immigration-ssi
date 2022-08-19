import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DisplayDs2019sComponent } from './display-ds2019s.component';

describe('DisplayDs2019sComponent', () => {
  let component: DisplayDs2019sComponent;
  let fixture: ComponentFixture<DisplayDs2019sComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ DisplayDs2019sComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(DisplayDs2019sComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
