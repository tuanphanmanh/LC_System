import { Component, Injector, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { AppComponentBase } from '@shared/common/app-component-base';
import { TenantRegistrationServiceProxy } from '@shared/service-proxies/service-proxies';

@Component({
    selector: 'payment-completed',
    templateUrl: './buy-succeed.component.html',
})
export class BuySucceedComponent extends AppComponentBase implements OnInit {
    constructor(
        _injector: Injector,
        private _router: Router,
        private _activatedRoute: ActivatedRoute,
        private _tenantRegistrationService: TenantRegistrationServiceProxy
        ) {
        super(_injector);
    }

    ngOnInit(): void {
        let paymentId = this._activatedRoute.snapshot.queryParams['paymentId'];
        this._tenantRegistrationService.buyNowSucceed(paymentId).subscribe(() => {
            this._router.navigate([abp.appPath + 'app/admin/subscription-management']);
        });
    }
}
