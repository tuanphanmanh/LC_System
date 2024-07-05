import { Injectable } from '@angular/core';
import {
    SubscriptionPaymentGatewayType
} from '@shared/service-proxies/service-proxies';

@Injectable()
export class PaymentHelperService {
    getPaymentGatewayType(gatewayType) {
        if (parseInt(gatewayType) === SubscriptionPaymentGatewayType.Paypal) {
            return 'Paypal';
        }

        return 'Stripe';
    }
}
