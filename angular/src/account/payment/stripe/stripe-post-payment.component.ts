import { Component, OnInit, Injector } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import { StripePaymentServiceProxy } from '@shared/service-proxies/service-proxies';
import { ActivatedRoute, Router } from '@angular/router';

@Component({
    selector: 'stripe-post-payment',
    templateUrl: './stripe-post-payment.component.html',
})
export class StripePostPaymentComponent extends AppComponentBase implements OnInit {
    paymentId: number;
    controlTimeout = 1000 * 5;
    maxControlCount = 5;

    constructor(
        _injector: Injector,
        private _stripePaymentService: StripePaymentServiceProxy,
        private _activatedRoute: ActivatedRoute,
        private _router: Router,
    ) {
        super(_injector);
    }

    ngOnInit() {
        this.paymentId = this._activatedRoute.snapshot.queryParams['paymentId'];
        this.getPaymentResult();
    }

    getPaymentResult(): void {
        this._stripePaymentService.getPaymentResult(this.paymentId).subscribe((paymentResult) => {
            if (paymentResult.paymentDone) {
                if (paymentResult.callbackUrl) {
                    this._router.navigate([paymentResult.callbackUrl], {
                        queryParams: {
                            paymentId: this.paymentId
                        },
                    });
                }
            } else {
                this.controlAgain();
            }
        },
            (err) => {
                this.controlAgain();
            }
        );
    }

    controlAgain() {
        if (this.maxControlCount === 0) {
            return;
        }

        setTimeout(() => {
            this.getPaymentResult();
        }, this.controlTimeout);
        this.controlTimeout *= 2;
        this.maxControlCount--;
    }
}
