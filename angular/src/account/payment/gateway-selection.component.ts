import { Component, Injector, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { accountModuleAnimation } from '@shared/animations/routerTransition';
import { AppComponentBase } from '@shared/common/app-component-base';
import {
    PaymentServiceProxy,
    TenantRegistrationServiceProxy,
    PaymentGatewayModel,
    CreatePaymentDto,
    SubscriptionPaymentDto,
    UpdatePaymentDto,
} from '@shared/service-proxies/service-proxies';
import { AppConsts } from '@shared/AppConsts';
import { PaymentHelperService } from './payment-helper.service';

@Component({
    templateUrl: './gateway-selection.component.html',
    animations: [accountModuleAnimation()],
})
export class GatewaySelectionComponent extends AppComponentBase implements OnInit {
    payment: SubscriptionPaymentDto = new SubscriptionPaymentDto();
    tenantId: number = abp.session.tenantId;
    paymentId: number;

    paymentGateways: PaymentGatewayModel[];
    supportsRecurringPayments = false;
    recurringPaymentEnabled = false;

    constructor(
        injector: Injector,
        private _activatedRoute: ActivatedRoute,
        private _router: Router,
        private _paymnetHelperService: PaymentHelperService,
        private _paymentAppService: PaymentServiceProxy,
        private _tenantRegistrationService: TenantRegistrationServiceProxy
    ) {
        super(injector);
    }

    ngOnInit(): void {
        this.paymentId = parseInt(this._activatedRoute.snapshot.queryParams['paymentId']);
        this._paymentAppService.getPayment(this.paymentId).subscribe((result: SubscriptionPaymentDto) => {
            this.payment = result;
            abp.multiTenancy.setTenantIdCookie(result.tenantId);
        });

        this._paymentAppService.getActiveGateways(undefined).subscribe((result: PaymentGatewayModel[]) => {
            this.paymentGateways = result;
            this.supportsRecurringPayments = result.some((pg) => pg.supportsRecurringPayments);
        });
    }

    checkout(gatewayType) {
        let input = {} as UpdatePaymentDto;
        input.paymentId = this.paymentId;
        input.isRecurring = this.recurringPaymentEnabled;
        input.gateway = gatewayType;

        this._paymentAppService.updatePayment(input).subscribe(() => {
            this._router.navigate(
                ['account/' + this.getPaymentGatewayType(gatewayType).toLocaleLowerCase() + '-pre-payment'],
                {
                    queryParams: {
                        paymentId: this.paymentId
                    },
                }
            );
        });
    }

    getPaymentGatewayType(gatewayType): string {
        return this._paymnetHelperService.getPaymentGatewayType(gatewayType);
    }
}
