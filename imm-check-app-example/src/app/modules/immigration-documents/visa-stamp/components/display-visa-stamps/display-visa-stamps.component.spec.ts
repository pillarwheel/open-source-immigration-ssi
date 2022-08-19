import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DisplayVisaStampsComponent } from './display-visa-stamps.component';

describe('DisplayVisaStampsComponent', () => {
  let component: DisplayVisaStampsComponent;
  let fixture: ComponentFixture<DisplayVisaStampsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ DisplayVisaStampsComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(DisplayVisaStampsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
