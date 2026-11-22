phoneApp.controller(
    "PhoneNumberStatusReportController",
    function ($scope, ReportService) {

        $scope.reportRows = [];

        $scope.deviceIdFilter = null;
        $scope.statusFilter = null;

        $scope.deviceFilterApi = {};
        $scope.statusFilterApi = {};

        function normalizeRow(item) {
            return {
                deviceId:
                    item.deviceId !== undefined
                        ? item.deviceId
                        : item.DeviceId,

                deviceName:
                    item.deviceName !== undefined
                        ? item.deviceName
                        : item.DeviceName,

                status:
                    item.status !== undefined
                        ? item.status
                        : item.Status,

                statusName:
                    item.statusName !== undefined
                        ? item.statusName
                        : item.StatusName,

                numberOfPhoneNumbers:
                    item.numberOfPhoneNumbers !== undefined
                        ? item.numberOfPhoneNumbers
                        : item.NumberOfPhoneNumbers
            };
        }

        function normalizeRows(data) {
            return data.map(function (item) {
                return normalizeRow(item);
            });
        }

        $scope.loadReport = function () {

            var deviceId = null;
            var status = null;

            if (
                $scope.deviceFilterApi &&
                $scope.deviceFilterApi.getData
            ) {
                deviceId =
                    $scope.deviceFilterApi.getData();
            }

            if (
                $scope.statusFilterApi &&
                $scope.statusFilterApi.getData
            ) {
                status =
                    $scope.statusFilterApi.getData();
            }

            ReportService
                .getPhoneNumbersPerDevice(
                    deviceId,
                    status
                )
                .then(function (response) {
                    $scope.reportRows =
                        normalizeRows(response.data);
                }, function (error) {
                    console.log(error);

                    alert(
                        "Error loading phone number report."
                    );
                });
        };

        $scope.clearFilter = function () {

            $scope.deviceIdFilter = null;
            $scope.statusFilter = null;

            ReportService
                .getPhoneNumbersPerDevice(
                    null,
                    null
                )
                .then(function (response) {
                    $scope.reportRows =
                        normalizeRows(response.data);
                }, function (error) {
                    console.log(error);

                    alert(
                        "Error loading phone number report."
                    );
                });
        };

        $scope.loadReport();
    }
);