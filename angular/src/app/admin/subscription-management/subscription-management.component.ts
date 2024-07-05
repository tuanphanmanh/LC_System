import { Component, Injector, OnInit, ViewChild } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { AppComponentBase } from '@shared/common/app-component-base';
import {
    ApplicationInfoDto,
    InvoiceServiceProxy,
    PaymentServiceProxy,
    SessionServiceProxy,
    SubscriptionServiceProxy,
    TenantLoginInfoDto,
    UserLoginInfoDto,
    CreateInvoiceDto,
    SubscriptionStartType,
    SubscriptionPaymentType,
    TenantRegistrationServiceProxy,
    EditionWithFeaturesDto,
    StartExtendSubscriptionInput,
    StartUpgradeSubscriptionInput,
    PaymentPeriodType,
} from '@shared/service-proxies/service-proxies';

import { LazyLoadEvent } from 'primeng/api';
import { Paginator } from 'primeng/paginator';
import { Table } from 'primeng/table';
import { finalize } from 'rxjs/operators';

@Component({
    templateUrl: './subscription-management.component.html',
    animations: [appModuleAnimation()],
})
export class SubscriptionManagementComponent extends AppComponentBase implements OnInit {
    @ViewChild('dataTable', { static: true }) dataTable: Table;
    @ViewChild('paginator', { static: true }) paginator: Paginator;

    subscriptionStartType: typeof SubscriptionStartType = SubscriptionStartType;
    subscriptionPaymentType: typeof SubscriptionPaymentType = SubscriptionPaymentType;

    loading: boolean;
    user: UserLoginInfoDto = new UserLoginInfoDto();
    tenant: TenantLoginInfoDto = new TenantLoginInfoDto();
    application: ApplicationInfoDto = new ApplicationInfoDto();

    filterText = '';
    editions: EditionWithFeaturesDto[];

    constructor(
        injector: Injector,
        private _sessionService: SessionServiceProxy,
        private _paymentServiceProxy: PaymentServiceProxy,
        private _invoiceServiceProxy: InvoiceServiceProxy,
        private _subscriptionServiceProxy: SubscriptionServiceProxy,
        private _tenantRegistrationAppService: TenantRegistrationServiceProxy,
        private _activatedRoute: ActivatedRoute,
        private _router: Router
    ) {
        super(injector);
        this.filterText = this._activatedRoute.snapshot.queryParams['filterText'] || '';
    }

    ngOnInit(): void {
        this.getSettings();
    }

    createOrShowInvoice(paymentId: number, invoiceNo: string): void {
        if (invoiceNo) {
            window.open('/app/admin/invoice/' + paymentId, '_blank');
        } else {
            this._invoiceServiceProxy
                .createInvoice(new CreateInvoiceDto({ subscriptionPaymentId: paymentId }))
                .subscribe(() => {
                    this.getPaymentHistory();
                    window.open('/app/admin/invoice/' + paymentId, '_blank');
                });
        }
    }

    getSettings(): void {
        this.loading = true;
        this.appSession.init().then(() => {
            this.loading = false;
            this.user = this.appSession.user;
            this.tenant = this.appSession.tenant;
            this.application = this.appSession.application;
        });

        this._tenantRegistrationAppService.getEditionsForSelect().subscribe((result) => {
            this.editions = result.editionsWithFeatures;
        });
    }

    getPaymentHistory(event?: LazyLoadEvent) {
        if (this.primengTableHelper.shouldResetPaging(event)) {
            this.paginator.changePage(0);

            if (this.primengTableHelper.records && this.primengTableHelper.records.length > 0) {
                return;
            }
        }

        this.primengTableHelper.showLoadingIndicator();

        this._paymentServiceProxy
            .getPaymentHistory(
                this.primengTableHelper.getSorting(this.dataTable),
                this.primengTableHelper.getMaxResultCount(this.paginator, event),
                this.primengTableHelper.getSkipCount(this.paginator, event)
            )
            .pipe(finalize(() => this.primengTableHelper.hideLoadingIndicator()))
            .subscribe((result) => {
                this.primengTableHelper.totalRecordsCount = result.totalCount;
                this.primengTableHelper.records = result.items;
                this.primengTableHelper.hideLoadingIndicator();
            });
    }

    disableRecurringPayments() {
        this._subscriptionServiceProxy.disableRecurringPayments().subscribe((result) => {
            this.tenant.subscriptionPaymentType = this.subscriptionPaymentType.RecurringManual;
        });
    }

    enableRecurringPayments() {
        this._subscriptionServiceProxy.enableRecurringPayments().subscribe((result) => {
            this.tenant.subscriptionPaymentType = this.subscriptionPaymentType.RecurringAutomatic;
        });
    }

    hasRecurringSubscription(): boolean {
        return this.tenant.subscriptionPaymentType !== this.subscriptionPaymentType.Manual;
    }

    startUpdateSubscription(editionId: number, paymentPeriodType?: string): void {
        let input = new StartUpgradeSubscriptionInput();
        input.targetEditionId = editionId;
        input.paymentPeriodType = PaymentPeriodType[paymentPeriodType];
        input.successUrl = abp.appPath + 'account/upgrade-succeed';
        input.errorUrl = abp.appPath + 'account/payment-failed';
        this._subscriptionServiceProxy.startUpgradeSubscription(input).subscribe((result) => {
            if (result.upgraded) {
                this.message.success(this.l('YourAccountIsUpgraded'));
            } else {
                this._router.navigate(
                    ['account/gateway-selection'],
                    {
                        queryParams: {
                            paymentId: result.paymentId
                        },
                    }
                );
            }

        });
    }

    startExtendSubscription(): void {
        let input = new StartExtendSubscriptionInput();
        input.successUrl = abp.appPath + 'account/extend-succeed';
        input.errorUrl = abp.appPath + 'account/payment-failed';
        this._subscriptionServiceProxy.startExtendSubscription(input).subscribe((paymentId) => {
            this._router.navigate(
                ['account/gateway-selection'],
                {
                    queryParams: {
                        paymentId: paymentId
                    },
                }
            );
        });
    }
}
