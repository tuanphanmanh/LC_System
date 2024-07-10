import { Component, OnInit, ViewChild, Input, ChangeDetectorRef, ElementRef, HostListener, ViewRef } from '@angular/core';

@Component({
  selector: 'esign-tooltip',
  templateUrl: './esign-tooltip.component.html',
  styleUrls: ['./esign-tooltip.component.less']
})
export class EsignTooltipComponent implements OnInit {

  @ViewChild('tooltip', { static: true }) tooltip;
  @Input() contentElem;
  top: number;
  left: number;
  @Input() style;

  distanceTop = 10;
  distanceLeft = 22;
  numberMatrix = 115;

  constructor(
    private cdr: ChangeDetectorRef,
    private elem: ElementRef
  ) {
    // setTimeout(() => {
    //   this.elem.nativeElement.addEventListener('mousedown', (event) => {
    //     this.onMouseMove(event);
    //   });
    // }, 100);
  }

  ngOnInit() {
    const contentElemRect = this.contentElem ? this.contentElem.getBoundingClientRect() : null;
    const elemRect = this.tooltip ? this.tooltip.nativeElement.getBoundingClientRect() : null;
    this.top = contentElemRect !== null && elemRect !== null ? contentElemRect?.top - elemRect.height - this.distanceTop : 0;

    if (contentElemRect !== null) {
      let left_style = parseFloat(this.contentElem.style.left.split('%')[0]);
      // 118 là 2/3 độ rộng của tootltip
      // tslint:disable-next-line: curly
      if (left_style >= 0) this.left = contentElemRect.left;
      // tslint:disable-next-line: curly
      else left_style = contentElemRect.left + contentElemRect.width - 118;
    }
  }

  // tslint:disable-next-line: use-lifecycle-interface
  ngOnDestroy(): void {
    // this.elem.nativeElement.removeEventListener('mousedown', (event) => {
    //   this.onMouseMove(event);
    // });
  }

  @HostListener('document:mousemove', ['$event'])
  onMouseMove(event: MouseEvent) {
    // if (!this.left) {
    this.left = event.clientX - this.numberMatrix;
    if (!(this.cdr as ViewRef).destroyed) {
      this.cdr.detectChanges();
    }
    // }
  }

}
