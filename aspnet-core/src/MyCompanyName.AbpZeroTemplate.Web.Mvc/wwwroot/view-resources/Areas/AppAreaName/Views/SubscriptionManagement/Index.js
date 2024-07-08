(function () {
    $(function () {
        var _$paymentHistoryTable = $('#PaymentHistoryTable');
        var _paymentService = abp.services.app.payment;
        var _invoiceService = abp.services.app.invoice;
        var _subscriptionService = abp.services.app.subscription;

        const _editPersonModal = new app.ModalManager({
            viewUrl: abp.appPath + 'AppAreaName/SubscriptionManagement/ShowDetailModal',
            scriptUrl: abp.appPath + 'view-resources/Areas/AppAreaName/Views/SubscriptionManagement/_ShowDetailModal.js',
            modalClass: 'ShowDetailModalViewModel'
        });

        var _dataTable;

        function createDatatable() {
            var dataTable = _$paymentHistoryTable.DataTable({
                paging: true,
                serverSide: true,
                processing: true,
                listAction: {
                    ajaxFunction: _paymentService.getPaymentHistory,
                },
                columnDefs: [
                    {
                        className: 'dtr-control responsive',
                        orderable: false,
                        render: function () {
                            return '';
                        },
                        targets: 0,
                    },
                    {
                        targets: 1,
                        data: null,
                        orderable: false,
                        autoWidth: false,
                        defaultContent: '',
                        rowAction: {
                            text:
                                '<i class="fa fa-cog"></i> <span class="d-none d-md-inline-block d-lg-inline-block d-xl-inline-block">' +
                                app.localize('Actions') +
                                '</span> <span class="caret"></span>',
                            items: [
                                {
                                    text: app.localize('ShowInvoice'),
                                    action: function (data) {
                                        createOrShowInvoice(data.record);
                                    },
                                },
                                {
                                    text: app.localize('ShowDetail'),
                                    action: function (data) {
                                        showDetail(data.record);
                                    },
                                },
                            ],
                        },
                    },
                    {
                        targets: 2,
                        data: 'creationTime',
                        render: function (creationTime) {
                            return moment(creationTime).format('L');
                        },
                    },
                    {
                        targets: 3,
                        data: 'gateway',
                        render: function (gateway) {
                            return app.localize('SubscriptionPaymentGatewayType_' + gateway);
                        },
                    },
                    {
                        targets: 4,
                        data: 'totalAmount',
                        render: function (totalAmount) {
                            return $.fn.dataTable.render.number(',', '.', 2).display(totalAmount);
                        }
                    },
                    {
                        targets: 5,
                        data: 'status',
                        render: function (status) {
                            return app.localize('SubscriptionPaymentStatus_' + status);
                        },
                    },
                    {
                        targets: 6,
                        data: 'paymentPeriodType',
                        render: function (paymentPeriodType) {
                            return app.localize('PaymentPeriodType_' + paymentPeriodType);
                        },
                    },
                    {
                        targets: 7,
                        data: 'dayCount',
                    },
                    {
                        targets: 8,
                        data: 'externalPaymentId',
                    },
                    {
                        targets: 9,
                        data: 'invoiceNo',
                    },
                    {
                        targets: 10,
                        visible: false,
                        data: 'id',
                    }
                ],
            });

            return dataTable;
        }

        $('a[data-bs-toggle="tab"]').on('shown.bs.tab', function (e) {
            var target = $(e.target).attr('href');
            if (target === '#SubscriptionManagementPaymentHistoryTab') {
                if (_dataTable) {
                    return;
                }

                _dataTable = createDatatable();
            }
        });

        $('#btnDisableRecurringPayments').click(function () {
            abp.ui.setBusy();

            _subscriptionService.disableRecurringPayments({}).done(function () {
                abp.ui.clearBusy();
                $('#btnEnableRecurringPayments').closest('.mb-5').removeClass('d-none');
                $('#btnDisableRecurringPayments').closest('.mb-5').addClass('d-none');
                if ($('#btnExtend')) {
                    $('#btnExtend').removeClass('d-none');
                }
            });
        });

        $('#btnEnableRecurringPayments').click(function () {
            abp.ui.setBusy();

            _subscriptionService.enableRecurringPayments({}).done(function () {
                abp.ui.clearBusy();
                $('#btnDisableRecurringPayments').closest('.mb-5').removeClass('d-none');
                $('#btnEnableRecurringPayments').closest('.mb-5').addClass('d-none');
                $('#btnExtend').addClass('d-none');
            });
        });

        $('#btnExtend').click(function () {
            abp.ui.setBusy();

            _subscriptionService.startExtendSubscription({
                SuccessUrl: abp.appPath + "TenantRegistration/ExtendSucceed",
                ErrorUrl: abp.appPath + "Payment/PaymentFailed"
            })
                .done(function (paymentId) {
                    abp.ui.clearBusy();
                    document.location.href = abp.appPath + 'Payment/GatewaySelection?paymentId=' + paymentId;
                })
                .always(function () {
                    abp.ui.clearBusy();
                });
        });

        $('.upgrade').click(function () {
            abp.ui.setBusy();
            var targetEditionId = $(this).attr('data-target-edition-id');
            var paymentPeriodType = $(this).attr('data-payment-period-type');
            _subscriptionService.startUpgradeSubscription({
                targetEditionId: targetEditionId,
                paymentPeriodType: paymentPeriodType,
                successUrl: abp.appPath + 'TenantRegistration/UpgradeSucceed',
                errorUrl: abp.appPath + 'Payment/PaymentFailed',
            }).done(function (result) {
                abp.ui.clearBusy();
                if(result.upgraded){
                    abp.notify.success(app.localize('UpgradeSucceed'));
                }else{
                    document.location.href = abp.appPath + 'Payment/GatewaySelection?paymentId=' + result.paymentId;   
                }
            }).always(function () {
                abp.ui.clearBusy();
            });
        });
        
        $('.trial-buy-now').click(function () {
            abp.ui.setBusy();
            var paymentPeriodType = $(this).attr('data-payment-period-type');
            _subscriptionService.startTrialToBuySubscription({
                paymentPeriodType: paymentPeriodType,
                successUrl: abp.appPath + 'TenantRegistration/BuyNowSucceed',
                errorUrl: abp.appPath + 'Payment/PaymentFailed',
            }).done(function (paymentId) {
                abp.ui.clearBusy();
                document.location.href = abp.appPath + 'Payment/GatewaySelection?paymentId=' + paymentId;
            }).always(function () {
                abp.ui.clearBusy();
            });
        });

        function createOrShowInvoice(data) {
            var invoiceNo = data['invoiceNo'];
            var paymentId = data['id'];

            if (invoiceNo) {
                window.open('/AppAreaName/Invoice?paymentId=' + paymentId, '_blank');
            } else {
                _invoiceService
                    .createInvoice({
                        subscriptionPaymentId: paymentId,
                    })
                    .done(function () {
                        _dataTable.ajax.reload();
                        window.open('/AppAreaName/Invoice?paymentId=' + paymentId, '_blank');
                    });
            }
        }

        function showDetail(data) {
            const paymentId = data['id'];

            _editPersonModal.open({id: paymentId});
        }
    })
})();
