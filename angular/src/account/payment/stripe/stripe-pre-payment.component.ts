import { Component, Input, Injector, OnInit } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { AppComponentBase } from '@shared/common/app-component-base';
import { ScriptLoaderService } from '@shared/utils/script-loader.service';
import { accountModuleAnimation } from '@shared/animations/routerTransition';

import {
    StripePaymentServiceProxy,
    PaymentServiceProxy,
    SubscriptionPaymentDto,
    StripeConfigurationDto,
    SubscriptionPaymentGatewayType,
    SubscriptionStartType,
    StripeCreatePaymentSessionInput,
    SubscriptionPaymentProductDto,
} from '@shared/service-proxies/service-proxies';
import { AppConsts } from '@shared/AppConsts';

@Component({
    selector: 'stripe-pre-payment-component',
    templateUrl: './stripe-pre-payment.component.html',
    animations: [accountModuleAnimation()],
})
export class StripePrePaymentComponent extends AppComponentBase implements OnInit {

    amount = 0;
    description = '';

    subscriptionPayment: SubscriptionPaymentDto;
    stripeIsLoading = true;
    subscriptionPaymentGateway = SubscriptionPaymentGatewayType;
    subscriptionStartType = SubscriptionStartType;

    paymentId;
    successCallbackUrl;
    errorCallbackUrl;

    payment: SubscriptionPaymentDto = new SubscriptionPaymentDto();

    constructor(
        injector: Injector,
        private _activatedRoute: ActivatedRoute,
        private _stripePaymentAppService: StripePaymentServiceProxy,
        private _paymentAppService: PaymentServiceProxy
    ) {
        super(injector);
    }

    ngOnInit(): void {
        this.spinnerService.show();
        this.paymentId = this._activatedRoute.snapshot.queryParams['paymentId'];
        this.stripeIsLoading = true;

        new ScriptLoaderService()
            .load('https://js.stripe.com/v3')
            .then(() => {
                this._stripePaymentAppService.getConfiguration().subscribe(
                    (config: StripeConfigurationDto) => {
                        this._stripePaymentAppService
                            .createPaymentSession(
                                new StripeCreatePaymentSessionInput({
                                    paymentId: this.paymentId,
                                    successUrl: AppConsts.appBaseUrl + '/account/stripe-post-payment',
                                    cancelUrl: AppConsts.appBaseUrl + '/account/stripe-cancel-payment',
                                })
                            )
                            .subscribe(
                                (sessionId) => {
                                    this._paymentAppService.getPayment(this.paymentId).subscribe(
                                        (result: SubscriptionPaymentDto) => {
                                            this.payment = result;

                                            this.spinnerService.hide();

                                            let stripe = (<any>window).Stripe(config.publishableKey);
                                            let checkoutButton = document.getElementById('stripe-checkout');
                                            checkoutButton.addEventListener('click', function () {
                                                stripe.redirectToCheckout({ sessionId: sessionId });
                                            });

                                            this.stripeIsLoading = false;
                                        },
                                        (err) => {
                                            this.spinnerService.hide();
                                        }
                                    );
                                },
                                (err) => {
                                    this.spinnerService.hide();
                                }
                            );
                    },
                    (err) => {
                        this.spinnerService.hide();
                    }
                );
            })
            .catch((err) => {
                this.spinnerService.hide();
            });
    }
}
