import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AppComponentBaseComponent } from './app-component-base.component';

describe('AppComponentBaseComponent', () => {
  let component: AppComponentBaseComponent;
  let fixture: ComponentFixture<AppComponentBaseComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [AppComponentBaseComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(AppComponentBaseComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
