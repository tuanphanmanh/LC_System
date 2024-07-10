import { ComponentFixture, TestBed } from '@angular/core/testing';

import { MstDocumentComponent } from './mst-document.component';

describe('MstDocumentComponent', () => {
  let component: MstDocumentComponent;
  let fixture: ComponentFixture<MstDocumentComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [MstDocumentComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(MstDocumentComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
