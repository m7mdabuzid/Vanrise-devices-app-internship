phoneApp.controller(
    "PhoneNumberReservationController",
    function (
        $scope,
        PhoneNumberReservationService) {

        $scope.reservations = [];
        $scope.visibleReservations = [];

        $scope.clientIdFilter = null;
        $scope.phoneNumberIdFilter = null;

        $scope.appliedClientIdFilter = null;
        $scope.appliedPhoneNumberIdFilter = null;

        $scope.clientFilterApi = {};
        $scope.phoneNumberFilterApi = {};

        function formatDate(value) {
            if (!value) {
                return null;
            }

            return value.toString().substring(0, 10);
        }

        function normalizeReservation(item) {
            return {
                id: item.id !== undefined
                    ? item.id
                    : item.Id,

                clientId: item.clientId !== undefined
                    ? item.clientId
                    : item.ClientId,

                clientName: item.clientName !== undefined
                    ? item.clientName
                    : item.ClientName,

                phoneNumberId: item.phoneNumberId !== undefined
                    ? item.phoneNumberId
                    : item.PhoneNumberId,

                phoneNumber: item.phoneNumber !== undefined
                    ? item.phoneNumber
                    : item.PhoneNumber,

                bed: formatDate(
                    item.bed !== undefined
                        ? item.bed
                        : item.BED
                ),

                eed: formatDate(
                    item.eed !== undefined
                        ? item.eed
                        : item.EED
                )
            };
        }

        function normalizeReservations(data) {
            return data.map(function (item) {
                return normalizeReservation(item);
            });
        }

        $scope.loadReservations = function () {
            PhoneNumberReservationService
                .getAllReservations()
                .then(function (response) {
                    $scope.reservations =
                        normalizeReservations(response.data);

                    $scope.visibleReservations =
                        angular.copy($scope.reservations);
                }, function (error) {
                    console.log(error);
                    alert("Error loading reservations.");
                });
        };

        $scope.loadFilteredReservations = function () {
            PhoneNumberReservationService
                .getFilteredReservations(
                    $scope.appliedClientIdFilter,
                    $scope.appliedPhoneNumberIdFilter)
                .then(function (response) {
                    $scope.visibleReservations =
                        normalizeReservations(response.data);
                }, function (error) {
                    console.log(error);
                    alert("Error filtering reservations.");
                });
        };

        $scope.applyFilter = function () {
            $scope.appliedClientIdFilter =
                $scope.clientFilterApi.getData();

            $scope.appliedPhoneNumberIdFilter =
                $scope.phoneNumberFilterApi.getData();

            $scope.loadFilteredReservations();
        };

        $scope.clearFilter = function () {
            $scope.clientIdFilter = null;
            $scope.phoneNumberIdFilter = null;

            $scope.appliedClientIdFilter = null;
            $scope.appliedPhoneNumberIdFilter = null;

            $scope.visibleReservations =
                angular.copy($scope.reservations);
        };

        $scope.loadReservations();
    }
);