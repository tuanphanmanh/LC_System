import { ComponentFixture, TestBed } from '@angular/core/testing';

import { MstCertificateComponent } from './mst-certificate.component';

describe('MstCertificateComponent', () => {
  let component: MstCertificateComponent;
  let fixture: ComponentFixture<MstCertificateComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [MstCertificateComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(MstCertificateComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
