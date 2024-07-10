import { ComponentFixture, TestBed } from '@angular/core/testing';

import { MstContractComponent } from './mst-contract.component';

describe('MstContractComponent', () => {
  let component: MstContractComponent;
  let fixture: ComponentFixture<MstContractComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [MstContractComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(MstContractComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
