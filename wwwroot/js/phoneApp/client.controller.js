phoneApp.controller(
    "ClientController",
    function (
        $scope,
        $uibModal,
        ClientService,
        PhoneNumberReservationService,
        MessageService
    ) {

        $scope.clientTypeFilterApi = {};

        $scope.clients = [];
        $scope.visibleClients = [];

        $scope.nameFilter = "";
        $scope.typeFilter = null;

        $scope.appliedNameFilter = "";
        $scope.appliedTypeFilter = null;

        $scope.clientTypes = [
            { value: 1, text: "Individual" },
            { value: 2, text: "Organization" }
        ];

        function formatDate(value) {
            if (!value) {
                return null;
            }

            return value.toString().substring(0, 10);
        }

        function normalizeClient(item) {
            return {
                id:
                    item.id !== undefined
                        ? item.id
                        : item.Id,

                name:
                    item.name !== undefined
                        ? item.name
                        : item.Name,

                type:
                    item.type !== undefined
                        ? item.type
                        : item.Type,

                birthDate: formatDate(
                    item.birthDate !== undefined
                        ? item.birthDate
                        : item.BirthDate
                ),

                hasActiveReservation:
                    item.hasActiveReservation !== undefined
                        ? item.hasActiveReservation
                        : item.HasActiveReservation
            };
        }

        function normalizeClients(data) {
            return data.map(function (item) {
                return normalizeClient(item);
            });
        }

        $scope.getClientTypeText = function (typeValue) {
            return typeValue === 1
                ? "Individual"
                : "Organization";
        };

        $scope.loadClients = function () {

            ClientService.getAllClients()
                .then(function (response) {

                    $scope.clients =
                        normalizeClients(response.data);

                    $scope.visibleClients =
                        angular.copy($scope.clients);

                }, function (error) {

                    console.log(error);

                    MessageService.error(
                        error,
                        "Clients could not be loaded."
                    );
                });
        };

        $scope.loadFilteredClients = function () {

            ClientService.getFilteredClients(
                $scope.appliedNameFilter,
                $scope.appliedTypeFilter
            )
                .then(function (response) {

                    $scope.visibleClients =
                        normalizeClients(response.data);

                }, function (error) {

                    console.log(error);

                    MessageService.error(
                        error,
                        "Clients could not be filtered."
                    );
                });
        };

        $scope.applyFilter = function () {

            $scope.appliedNameFilter =
                $scope.nameFilter || "";

            if (
                $scope.clientTypeFilterApi &&
                $scope.clientTypeFilterApi.getData
            ) {
                $scope.appliedTypeFilter =
                    $scope.clientTypeFilterApi.getData();
            } else {
                $scope.appliedTypeFilter =
                    $scope.typeFilter;
            }

            $scope.loadFilteredClients();
        };

        $scope.clearFilter = function () {

            $scope.nameFilter = "";
            $scope.typeFilter = null;

            $scope.appliedNameFilter = "";
            $scope.appliedTypeFilter = null;

            $scope.visibleClients =
                angular.copy($scope.clients);
        };

        $scope.reloadGridAfterSave = function () {

            ClientService.getAllClients()
                .then(function (response) {

                    $scope.clients =
                        normalizeClients(response.data);

                    if (
                        ($scope.appliedNameFilter || "")
                            .trim() === "" &&
                        (
                            $scope.appliedTypeFilter === null ||
                            $scope.appliedTypeFilter === undefined
                        )
                    ) {
                        $scope.visibleClients =
                            angular.copy($scope.clients);
                    } else {
                        $scope.loadFilteredClients();
                    }

                }, function (error) {

                    console.log(error);

                    MessageService.error(
                        error,
                        "Clients could not be reloaded."
                    );
                });
        };

        $scope.openReservationModal =
            function (client, mode) {

                var modalInstance =
                    $uibModal.open({
                        templateUrl:
                            "/templates/phoneNumberReservationModal.html",

                        controller:
                            "PhoneNumberReservationModalController",

                        resolve: {
                            clientData: function () {
                                return angular.copy(client);
                            },

                            reservationMode: function () {
                                return mode;
                            }
                        }
                    });

                modalInstance.result.then(
                    function (data) {

                        var request = {
                            clientId: client.id,
                            phoneNumberId:
                                data.phoneNumberId
                        };

                        var operation;

                        if (mode === "reserve") {
                            operation =
                                PhoneNumberReservationService
                                    .reservePhoneNumber(
                                        request
                                    );
                        } else {
                            operation =
                                PhoneNumberReservationService
                                    .unreservePhoneNumber(
                                        request
                                    );
                        }

                        operation.then(
                            function (response) {

                                MessageService.success(
                                    response,
                                    mode === "reserve"
                                        ? "Phone number reserved successfully."
                                        : "Phone number unreserved successfully."
                                );

                                $scope
                                    .reloadGridAfterSave();
                            },
                            function (error) {

                                console.log(error);

                                MessageService.error(
                                    error,
                                    mode === "reserve"
                                        ? "Phone number could not be reserved."
                                        : "Phone number could not be unreserved."
                                );
                            }
                        );
                    }
                );
            };

        $scope.openClientModal =
            function (selectedClient) {

                var isEditMode =
                    selectedClient !== undefined &&
                    selectedClient !== null;

                var modalInstance =
                    $uibModal.open({
                        templateUrl:
                            "/templates/clientModal.html",

                        controller:
                            "ClientModalController",

                        resolve: {
                            clientData: function () {

                                if (isEditMode) {
                                    return angular.copy(
                                        selectedClient
                                    );
                                }

                                return {
                                    id: 0,
                                    name: "",
                                    type: 1,
                                    birthDate: null
                                };
                            },

                            isEdit: function () {
                                return isEditMode;
                            },

                            clientTypes: function () {
                                return $scope.clientTypes;
                            }
                        }
                    });

                modalInstance.result.then(
                    function (savedClient) {

                        if (savedClient.type === 2) {
                            savedClient.birthDate = null;
                        }

                        if (isEditMode) {

                            ClientService
                                .updateClient(savedClient)
                                .then(
                                    function (response) {

                                        MessageService.success(
                                            response,
                                            "Client updated successfully."
                                        );

                                        $scope
                                            .reloadGridAfterSave();
                                    },
                                    function (error) {

                                        console.log(error);

                                        MessageService.error(
                                            error,
                                            "Client could not be updated."
                                        );
                                    }
                                );

                        } else {

                            ClientService
                                .addClient(savedClient)
                                .then(
                                    function (response) {

                                        MessageService.success(
                                            response,
                                            "Client added successfully."
                                        );

                                        $scope
                                            .reloadGridAfterSave();
                                    },
                                    function (error) {

                                        console.log(error);

                                        MessageService.error(
                                            error,
                                            "Client could not be added."
                                        );
                                    }
                                );
                        }
                    }
                );
            };

        $scope.deleteClient = function (client) {

            if (!confirm("Delete this client?")) {
                return;
            }

            ClientService.deleteClient(client.id)
                .then(function (response) {

                    MessageService.success(
                        response,
                        "Client deleted successfully."
                    );

                    $scope.reloadGridAfterSave();

                }, function (error) {

                    console.log(error);

                    MessageService.error(
                        error,
                        "Client could not be deleted."
                    );
                });
        };

        $scope.loadClients();
    }
);


phoneApp.controller(
    "ClientModalController",
    function (
        $scope,
        $uibModalInstance,
        clientData,
        isEdit,
        clientTypes
    ) {

        $scope.client = clientData;
        $scope.isEdit = isEdit;
        $scope.clientTypes = clientTypes;

        $scope.clientTypeEditorApi = {};

        $scope.currentYear =
            new Date().getFullYear();

        $scope.birthDateParts = {
            day: null,
            month: null,
            year: null
        };

        if ($scope.client.birthDate) {

            var existingDate =
                $scope.client.birthDate
                    .toString()
                    .substring(0, 10)
                    .split("-");

            if (existingDate.length === 3) {

                $scope.birthDateParts.year =
                    parseInt(existingDate[0], 10);

                $scope.birthDateParts.month =
                    parseInt(existingDate[1], 10);

                $scope.birthDateParts.day =
                    parseInt(existingDate[2], 10);
            }
        }

        if (isEdit) {
            $scope.modalTitle = "Edit Client";
            $scope.saveButtonText = "Save Changes";
        } else {
            $scope.modalTitle = "Add Client";
            $scope.saveButtonText = "Add Client";
        }

        function isWholeNumber(value) {
            return angular.isNumber(value) &&
                isFinite(value) &&
                Math.floor(value) === value;
        }

        $scope.isBirthDateMissing = function () {

            return $scope.client.type === 1 &&
                (
                    $scope.birthDateParts.day === null ||
                    $scope.birthDateParts.day === undefined ||
                    $scope.birthDateParts.month === null ||
                    $scope.birthDateParts.month === undefined ||
                    $scope.birthDateParts.year === null ||
                    $scope.birthDateParts.year === undefined
                );
        };

        $scope.isDayValid = function () {

            var day = $scope.birthDateParts.day;

            return isWholeNumber(day) &&
                day >= 1 &&
                day <= 31;
        };

        $scope.isMonthValid = function () {

            var month =
                $scope.birthDateParts.month;

            return isWholeNumber(month) &&
                month >= 1 &&
                month <= 12;
        };

        $scope.isYearValid = function () {

            var year =
                $scope.birthDateParts.year;

            return isWholeNumber(year) &&
                year >= 1900 &&
                year <= $scope.currentYear;
        };

        $scope.isCalendarDateValid = function () {

            if (
                !$scope.isDayValid() ||
                !$scope.isMonthValid() ||
                !$scope.isYearValid()
            ) {
                return false;
            }

            var day =
                $scope.birthDateParts.day;

            var month =
                $scope.birthDateParts.month;

            var year =
                $scope.birthDateParts.year;

            var date =
                new Date(
                    year,
                    month - 1,
                    day
                );

            return date.getFullYear() === year &&
                date.getMonth() === month - 1 &&
                date.getDate() === day;
        };

        $scope.isAgeValid = function () {

            if ($scope.client.type !== 1) {
                return true;
            }

            if (!$scope.isCalendarDateValid()) {
                return false;
            }

            var birthDate =
                new Date(
                    $scope.birthDateParts.year,
                    $scope.birthDateParts.month - 1,
                    $scope.birthDateParts.day
                );

            var eighteenYearsAgo =
                new Date();

            eighteenYearsAgo.setFullYear(
                eighteenYearsAgo.getFullYear() - 18
            );

            return birthDate < eighteenYearsAgo;
        };

        $scope.isBirthDateValid = function () {

            if ($scope.client.type !== 1) {
                return true;
            }

            return !$scope.isBirthDateMissing() &&
                $scope.isCalendarDateValid() &&
                $scope.isAgeValid();
        };

        function twoDigits(value) {

            return value < 10
                ? "0" + value
                : value.toString();
        }

        $scope.saveClient = function () {

            if (
                $scope.clientTypeEditorApi &&
                $scope.clientTypeEditorApi.getData
            ) {
                $scope.client.type =
                    $scope.clientTypeEditorApi.getData();
            }

            if ($scope.client.type === 2) {

                $scope.client.birthDate = null;

            } else {

                if (!$scope.isBirthDateValid()) {
                    return;
                }

                $scope.client.birthDate =
                    $scope.birthDateParts.year +
                    "-" +
                    twoDigits(
                        $scope.birthDateParts.month
                    ) +
                    "-" +
                    twoDigits(
                        $scope.birthDateParts.day
                    );
            }

            $uibModalInstance.close(
                $scope.client
            );
        };

        $scope.cancel = function () {

            $uibModalInstance.dismiss(
                "cancel"
            );
        };
    }
);


phoneApp.controller(
    "PhoneNumberReservationModalController",
    function (
        $scope,
        $uibModalInstance,
        clientData,
        reservationMode
    ) {

        $scope.client = clientData;
        $scope.mode = reservationMode;

        $scope.isReserve =
            reservationMode === "reserve";

        $scope.formData = {
            phoneNumberId: null
        };

        $scope.phoneNumberSelectorApi = {};

        $scope.modalTitle =
            $scope.isReserve
                ? "Reserve Phone Number"
                : "Unreserve Phone Number";

        $scope.saveButtonText =
            $scope.isReserve
                ? "Reserve"
                : "Unreserve";

        $scope.save = function () {

            var phoneNumberId =
                $scope.phoneNumberSelectorApi &&
                    $scope.phoneNumberSelectorApi.getData
                    ? $scope
                        .phoneNumberSelectorApi
                        .getData()
                    : $scope.formData.phoneNumberId;

            if (!phoneNumberId) {
                return;
            }

            $uibModalInstance.close({
                phoneNumberId: phoneNumberId
            });
        };

        $scope.cancel = function () {

            $uibModalInstance.dismiss(
                "cancel"
            );
        };
    }
);