phoneApp.controller(
    "ClientTypeReportController",
    function ($scope, ReportService) {

        $scope.reportRows = [];

        $scope.typeFilter = null;
        $scope.clientTypeFilterApi = {};

        function normalizeRow(item) {
            return {
                type:
                    item.type !== undefined
                        ? item.type
                        : item.Type,

                typeName:
                    item.typeName !== undefined
                        ? item.typeName
                        : item.TypeName,

                numberOfClients:
                    item.numberOfClients !== undefined
                        ? item.numberOfClients
                        : item.NumberOfClients
            };
        }

        function normalizeRows(data) {
            return data.map(function (item) {
                return normalizeRow(item);
            });
        }

        $scope.loadReport = function () {

            var selectedType = null;

            if (
                $scope.clientTypeFilterApi &&
                $scope.clientTypeFilterApi.getData
            ) {
                selectedType =
                    $scope.clientTypeFilterApi.getData();
            }

            ReportService
                .getClientsPerType(selectedType)
                .then(function (response) {
                    $scope.reportRows =
                        normalizeRows(response.data);
                }, function (error) {
                    console.log(error);
                    alert("Error loading report.");
                });
        };

        $scope.clearFilter = function () {
            $scope.typeFilter = null;

            ReportService
                .getClientsPerType(null)
                .then(function (response) {
                    $scope.reportRows =
                        normalizeRows(response.data);
                }, function (error) {
                    console.log(error);
                    alert("Error loading report.");
                });
        };

        $scope.loadReport();
    }
);