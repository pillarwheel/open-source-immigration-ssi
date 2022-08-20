import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DisplayVisasComponent } from './display-visas.component';

describe('DisplayVisasComponent', () => {
  let component: DisplayVisasComponent;
  let fixture: ComponentFixture<DisplayVisasComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ DisplayVisasComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(DisplayVisasComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
