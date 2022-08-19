import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DisplayDnaInfoListComponent } from './display-dna-info-list.component';

describe('DisplayDnaInfoListComponent', () => {
  let component: DisplayDnaInfoListComponent;
  let fixture: ComponentFixture<DisplayDnaInfoListComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ DisplayDnaInfoListComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(DisplayDnaInfoListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
