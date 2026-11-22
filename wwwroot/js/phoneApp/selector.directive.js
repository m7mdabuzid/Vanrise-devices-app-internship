phoneApp.directive("clientTypeSelector", function () {
    return {
        restrict: "E",

        scope: {
            selectedValue: "=ngModel",
            selectorApi: "=?selectorApi",
            emptyText: "@?"
        },

        template:
            '<select class="form-control modern-input" ' +
            'ng-model="selectedValue" ' +
            'ng-options="type.value as type.text for type in clientTypes">' +
            '<option value="">{{ emptyText || "Select Type" }}</option>' +
            '</select>',

        link: function (scope) {
            scope.clientTypes = [
                { value: 1, text: "Individual" },
                { value: 2, text: "Organization" }
            ];

            scope.selectorApi = scope.selectorApi || {};

            scope.selectorApi.getData = function () {
                return scope.selectedValue;
            };
        }
    };
});


phoneApp.directive("deviceSelector", function (PhoneNumberService) {
    return {
        restrict: "E",

        scope: {
            selectedValue: "=ngModel",
            selectorApi: "=?selectorApi",
            emptyText: "@?"
        },

        template:
            '<select class="form-control modern-input" ' +
            'ng-model="selectedValue" ' +
            'ng-options="device.id as device.name for device in devices">' +
            '<option value="">{{ emptyText || "Select Device" }}</option>' +
            '</select>',

        link: function (scope) {
            scope.devices = [];

            PhoneNumberService.getDevices()
                .then(function (response) {
                    scope.devices = response.data.map(function (item) {
                        return {
                            id: item.id !== undefined ? item.id : item.Id,
                            name: item.name !== undefined ? item.name : item.Name
                        };
                    });
                });

            scope.selectorApi = scope.selectorApi || {};

            scope.selectorApi.getData = function () {
                return scope.selectedValue;
            };
        }
    };
});
phoneApp.directive(
    "clientSelector",
    function (PhoneNumberReservationService) {

        return {
            restrict: "E",

            scope: {
                selectedValue: "=ngModel",
                selectorApi: "=?selectorApi",
                emptyText: "@?"
            },

            template:
                '<select class="form-control modern-input" ' +
                'ng-model="selectedValue" ' +
                'ng-options="client.id as client.name for client in clients">' +
                '<option value="">{{ emptyText || "Select Client" }}</option>' +
                '</select>',

            link: function (scope) {
                scope.clients = [];

                PhoneNumberReservationService.getClients()
                    .then(function (response) {
                        scope.clients = response.data.map(function (item) {
                            return {
                                id: item.id !== undefined
                                    ? item.id
                                    : item.Id,

                                name: item.name !== undefined
                                    ? item.name
                                    : item.Name
                            };
                        });
                    });

                scope.selectorApi =
                    scope.selectorApi || {};

                scope.selectorApi.getData = function () {
                    return scope.selectedValue;
                };
            }
        };
    }
);


phoneApp.directive(
    "phoneNumberSelector",
    function (PhoneNumberReservationService) {

        return {
            restrict: "E",

            scope: {
                selectedValue: "=ngModel",
                selectorApi: "=?selectorApi",
                emptyText: "@?",
                mode: "@?",
                clientId: "=?"
            },

            template:
                '<select class="form-control modern-input" ' +
                'ng-model="selectedValue" ' +
                'ng-options="item.id as item.number for item in phoneNumbers">' +
                '<option value="">{{ emptyText || "Select Phone Number" }}</option>' +
                '</select>',

            link: function (scope) {
                scope.phoneNumbers = [];

                function normalize(data) {
                    scope.phoneNumbers =
                        data.map(function (item) {
                            return {
                                id: item.id !== undefined
                                    ? item.id
                                    : item.Id,

                                number: item.number !== undefined
                                    ? item.number
                                    : item.Number
                            };
                        });
                }

                function loadData() {
                    var request;

                    if (scope.mode === "available") {
                        request =
                            PhoneNumberReservationService
                                .getAvailablePhoneNumbers();
                    }
                    else if (scope.mode === "active") {
                        request =
                            PhoneNumberReservationService
                                .getActivePhoneNumbersByClient(
                                    scope.clientId);
                    }
                    else {
                        request =
                            PhoneNumberReservationService
                                .getPhoneNumbers();
                    }

                    request.then(function (response) {
                        normalize(response.data);
                    });
                }

                scope.selectorApi =
                    scope.selectorApi || {};

                scope.selectorApi.getData = function () {
                    return scope.selectedValue;
                };

                loadData();
            }
        };
    }
);

phoneApp.directive(
    "phoneNumberStatusSelector",
    function () {

        return {
            restrict: "E",

            scope: {
                selectedValue: "=ngModel",
                selectorApi: "=?selectorApi",
                emptyText: "@?"
            },

            template:
                '<select class="form-control" ' +
                'ng-model="selectedValue" ' +
                'ng-options="item.value as item.text for item in items">' +
                '<option value="">{{ emptyText || "All Statuses" }}</option>' +
                '</select>',

            link: function (scope) {

                scope.items = [
                    {
                        value: 1,
                        text: "Reserved"
                    },
                    {
                        value: 2,
                        text: "Unreserved"
                    }
                ];

                scope.selectorApi =
                    scope.selectorApi || {};

                scope.selectorApi.getData =
                    function () {
                        return scope.selectedValue;
                    };
            }
        };
    }
);