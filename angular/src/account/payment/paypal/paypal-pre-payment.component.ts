import { Component, Injector, OnInit } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { AppComponentBase } from '@shared/common/app-component-base';
import { ScriptLoaderService } from '@shared/utils/script-loader.service';
import { accountModuleAnimation } from '@shared/animations/routerTransition';

import {
    PayPalPaymentServiceProxy,
    SubscriptionPaymentDto,
    PaymentServiceProxy,
    PayPalConfigurationDto,
} from '@shared/service-proxies/service-proxies';

@Component({
    selector: 'paypal-pre-payment-component',
    templateUrl: './paypal-pre-payment.component.html',
    animations: [accountModuleAnimation()],
})
export class PayPalPrePaymentComponent extends AppComponentBase implements OnInit {

    config: PayPalConfigurationDto;
    payment: SubscriptionPaymentDto = new SubscriptionPaymentDto();

    paypalIsLoading = true;
    paymentId;
    successCallbackUrl;
    errorCallbackUrl;

    constructor(
        injector: Injector,
        private _activatedRoute: ActivatedRoute,
        private _payPalPaymentAppService: PayPalPaymentServiceProxy,
        private _paymentAppService: PaymentServiceProxy,
        private _router: Router
    ) {
        super(injector);
    }

    ngOnInit(): void {

        this.setTenantIdCookieIfNeeded();

        this.paymentId = this._activatedRoute.snapshot.queryParams['paymentId'];

        this._payPalPaymentAppService.getConfiguration().subscribe((config: PayPalConfigurationDto) => {
            this.config = config;

            let disabledFundings = this.GetDisabledFundingsQueryString(config);
            new ScriptLoaderService()
                .load(
                    'https://www.paypal.com/sdk/js?client-id=' +
                    config.clientId +
                    '&currency=' +
                    this.appSession.application.currency +
                    disabledFundings
                )
                .then(() => {
                    this._paymentAppService.getPayment(this.paymentId).subscribe((result: SubscriptionPaymentDto) => {
                        this.payment = result;
                        this.successCallbackUrl = result.successUrl;
                        this.errorCallbackUrl = result.errorUrl;

                        this.paypalIsLoading = false;
                        this.preparePaypalButton();
                    });
                });
        });
    }

    preparePaypalButton(): void {
        const self = this;
        (<any>window).paypal
            .Buttons({
                createOrder: function (data, actions) {
                    return actions.order.create({
                        purchase_units: [
                            {
                                amount: {
                                    value: self.payment.totalAmount,
                                    currency_code: self.appSession.application.currency,
                                },
                            },
                        ],
                    });
                },
                onApprove: function (data, actions) {
                    self._payPalPaymentAppService.confirmPayment(self.paymentId, data.orderID).subscribe(() => {
                        if (self.successCallbackUrl) {
                            self._router.navigate([self.successCallbackUrl], {
                                queryParams: {
                                    paymentId: self.paymentId
                                },
                            });
                        }
                    });
                },
            })
            .render('#paypal-button');
    }

    GetDisabledFundingsQueryString(config: PayPalConfigurationDto): string {
        if (!config.disabledFundings || config.disabledFundings.length <= 0) {
            return '';
        }

        return '&disable-funding=' + config.disabledFundings.join();
    }

    setTenantIdCookieIfNeeded(): void {
        if (!this._activatedRoute.snapshot.queryParams['tenantId']) {
            return;
        }

        let tenantId = parseInt(this._activatedRoute.snapshot.queryParams['tenantId']);
        abp.multiTenancy.setTenantIdCookie(tenantId);
    }
}
