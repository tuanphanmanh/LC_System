var _$publishedMassNotificationsTableData = {};
(function () {
    $(function () {
        var _$publishedMassNotificationsTable = $('#PublishedMassNotificationsTable');
        var _notificationService = abp.services.app.notification;

        var _permissions = {
            massNotification: abp.auth.hasPermission('Pages.Administration.MassNotification')
        };

        var _createMassNotificationModal = new app.ModalManager({
            viewUrl: abp.appPath + 'AppAreaName/Notifications/CreateMassNotificationModal',
            scriptUrl: abp.appPath + 'view-resources/Areas/AppAreaName/Views/Notifications/_CreateMassNotificationModal.js',
            modalClass: 'CreateMassNotificationModal',
        });

        var _selectedDateRangeNotification = {
            startDate: moment().startOf('day').subtract(7, 'days'),
            endDate: moment().endOf('day'),
        };

        $(document)
            .find('#PublishedNotifications_StartEndRange')
            .daterangepicker(
                $.extend(true, app.createDateRangePickerOptions(), _selectedDateRangeNotification),
                function (start, end) {
                    _selectedDateRangeNotification.startDate = start.format('YYYY-MM-DDT00:00:00Z');
                    _selectedDateRangeNotification.endDate = end.format('YYYY-MM-DDT23:59:59.999Z');

                    getNotifications();
                }
            );

        var dataTable = _$publishedMassNotificationsTable
            .DataTable({
                paging: true,
                serverSide: true,
                processing: true,
                listAction: {
                    ajaxFunction: _notificationService.getNotificationsPublishedByUser,
                    inputFilter: function () {
                        _$publishedMassNotificationsTableData = {};//before every call clean previous data
                        return {
                            startDate: _selectedDateRangeNotification.startDate,
                            endDate: _selectedDateRangeNotification.endDate
                        };
                    },
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
                        data: 'data',
                        name: 'data',
                        orderable: false,
                        render: function (data) {
                            return getNotificationMessageDisplay(data, 50);
                        },
                    },
                    {
                        targets: 2,
                        data: 'severity',
                        name: 'severity',
                        orderable: false,
                        render: function (severity) {
                            var $span = $('<span/>').addClass('label');
                            var severityClass = getSeverityClass(severity);
                            $span.addClass(severityClass).text(app.localize('Enum_NotificationSeverity_' + severity));
                            return $span[0].outerHTML;
                            
                        },
                    },
                    {
                        targets: 3,
                        data: 'creationTime',
                        orderable: false,
                        render: function (creationTime) {
                            return moment(creationTime).format('YYYY-MM-DD HH:mm:ss');
                        },
                    },
                    {
                        targets: 4,
                        data: 'isPublished',
                        name: 'isPublished',
                        orderable: false,
                        render: function (isPublished) {
                            var $span = $('<span/>').addClass('label');
                            if (isPublished) {
                                $span.addClass('badge badge-success').text(app.localize('Yes'));
                            } else {
                                $span.addClass('badge badge-dark').text(app.localize('No'));
                            }
                            return $span[0].outerHTML;
                        }
                    },

                ],
            });

        function getNotifications() {
            dataTable.ajax.reload();
        }
        
        function getSeverityClass(severity){
            if(severity == abp.notifications.severity.SUCCESS){
                return "badge badge-success";
            }

            if(severity == abp.notifications.severity.WARN){
                return "badge badge-warning";
            }

            if(severity == abp.notifications.severity.ERROR){
                return "badge badge-danger";
            }

            if(severity == abp.notifications.severity.FATAL){
                return "badge badge-danger";
            }

            return "badge badge-info";
        }

        $('#CreateNewMassNotificationButton').click(function () {
            if (_permissions.massNotification) {
                _createMassNotificationModal.open();
            }
        });

        $('#RefreshPublishedMassNotificationsButton').click(function () {
            getNotifications();
        });

        abp.event.on('app.createMassNotificationModalSaved', function () {
            getNotifications();
        });

        function getNotificationMessageDisplay(data, maxLength) {
            var message = $("<textarea/>")
                .html(JSON.parse(data).Message)
                .text();

            if (isHTML(message)) {
                return getHtmlDetailModalButton(message);
            }

            return getLengthSafetyDataOrDetailModal(message, maxLength);
        }

        function getHtmlDetailModalButton(data) {
            if (typeof data != 'string') {
                return data;
            }

            let randomId = getRandomId();
            _$publishedMassNotificationsTableData[randomId] = data;

            return `<button class="btn btn-secondary btn-sm" onclick="ShowMassNotificationDetailModal('${randomId}')">${app.localize('ShowHTMLData')}</button>`;
        }

        function getLengthSafetyDataOrDetailModal(data, maxLength) {
            if (typeof data != 'string') {
                return data;
            }

            if (data.length <= maxLength) {
                return data;
            }

            let randomId = getRandomId();
            _$publishedMassNotificationsTableData[randomId] = data;

            return `<span>${data.substring(0, maxLength)}...</span>
                    <button class="btn btn-secondary btn-sm" onclick="ShowMassNotificationDetailModal('${randomId}')" >${app.localize('ShowFullData')}</button>`;
        }

        function isHTML(str) {
            let doc = new DOMParser().parseFromString(str, "text/html");
            return [].slice.call(doc.body.childNodes).some(node => node.nodeType === 1);
        }

        function getRandomId() {
            return 'detail-' + parseInt(Math.random() * 10000000) + parseInt(Math.random() * 10000000);
        }
    });
})();

function ShowMassNotificationDetailModal(detailId) {
    let data = _$publishedMassNotificationsTableData[detailId];
    if (!data) {
        return;
    }

    var detailModal = $('#DataDetailModal');
    detailModal.find('.modal-body').html(data);
    detailModal.modal('show');
}