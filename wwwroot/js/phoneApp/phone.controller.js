phoneApp.controller(
    "PhoneController",
    function (
        $scope,
        $uibModal,
        PhoneService,
        MessageService
    ) {

        $scope.phones = [];
        $scope.visiblePhones = [];

        $scope.searchBox = "";
        $scope.appliedSearch = "";

        function normalizePhone(item) {
            return {
                id:
                    item.id !== undefined
                        ? item.id
                        : item.Id,

                name:
                    item.name !== undefined
                        ? item.name
                        : item.Name
            };
        }

        function normalizePhones(data) {
            return data.map(function (item) {
                return normalizePhone(item);
            });
        }

        $scope.loadPhones = function () {

            PhoneService.getAllPhones()
                .then(function (response) {

                    $scope.phones =
                        normalizePhones(response.data);

                    $scope.visiblePhones =
                        angular.copy($scope.phones);

                }, function (error) {

                    console.log(error);

                    MessageService.error(
                        error,
                        "Devices could not be loaded."
                    );
                });
        };

        $scope.loadFilteredPhones =
            function (searchValue) {

                PhoneService
                    .getFilteredPhones(searchValue)
                    .then(function (response) {

                        $scope.visiblePhones =
                            normalizePhones(
                                response.data
                            );

                    }, function (error) {

                        console.log(error);

                        MessageService.error(
                            error,
                            "Devices could not be filtered."
                        );
                    });
            };

        $scope.applySearch = function () {

            $scope.appliedSearch =
                $scope.searchBox || "";

            $scope.loadFilteredPhones(
                $scope.appliedSearch
            );
        };

        $scope.clearSearch = function () {

            $scope.searchBox = "";
            $scope.appliedSearch = "";

            $scope.visiblePhones =
                angular.copy($scope.phones);
        };

        $scope.reloadGridAfterSave = function () {

            PhoneService.getAllPhones()
                .then(function (response) {

                    $scope.phones =
                        normalizePhones(response.data);

                    if (
                        ($scope.appliedSearch || "")
                            .trim() === ""
                    ) {
                        $scope.visiblePhones =
                            angular.copy($scope.phones);
                    } else {
                        $scope.loadFilteredPhones(
                            $scope.appliedSearch
                        );
                    }

                }, function (error) {

                    console.log(error);

                    MessageService.error(
                        error,
                        "Devices could not be reloaded."
                    );
                });
        };

        $scope.openPhoneModal =
            function (selectedPhone) {

                var isEditMode =
                    selectedPhone !== undefined &&
                    selectedPhone !== null;

                var modalInstance =
                    $uibModal.open({
                        templateUrl:
                            "/templates/phoneModal.html",

                        controller:
                            "PhoneModalController",

                        resolve: {
                            phoneData: function () {

                                if (isEditMode) {
                                    return angular.copy(
                                        selectedPhone
                                    );
                                }

                                return {
                                    id: 0,
                                    name: ""
                                };
                            },

                            isEdit: function () {
                                return isEditMode;
                            }
                        }
                    });

                modalInstance.result.then(
                    function (savedPhone) {

                        if (isEditMode) {

                            PhoneService
                                .updatePhone(savedPhone)
                                .then(
                                    function (response) {

                                        MessageService
                                            .success(
                                                response,
                                                "Device updated successfully."
                                            );

                                        $scope
                                            .reloadGridAfterSave();
                                    },
                                    function (error) {

                                        console.log(error);

                                        MessageService
                                            .error(
                                                error,
                                                "Device could not be updated."
                                            );
                                    }
                                );

                        } else {

                            PhoneService
                                .addPhone(savedPhone)
                                .then(
                                    function (response) {

                                        MessageService
                                            .success(
                                                response,
                                                "Device added successfully."
                                            );

                                        $scope
                                            .reloadGridAfterSave();
                                    },
                                    function (error) {

                                        console.log(error);

                                        MessageService
                                            .error(
                                                error,
                                                "Device could not be added."
                                            );
                                    }
                                );
                        }
                    }
                );
            };

        $scope.deletePhone = function (phone) {

            if (!confirm("Delete this device?")) {
                return;
            }

            PhoneService.deletePhone(phone.id)
                .then(function (response) {

                    MessageService.success(
                        response,
                        "Device deleted successfully."
                    );

                    $scope.reloadGridAfterSave();

                }, function (error) {

                    console.log(error);

                    MessageService.error(
                        error,
                        "Device could not be deleted."
                    );
                });
        };

        $scope.loadPhones();
    }
);


phoneApp.controller(
    "PhoneModalController",
    function (
        $scope,
        $uibModalInstance,
        phoneData,
        isEdit
    ) {

        $scope.phone = phoneData;
        $scope.isEdit = isEdit;

        if (isEdit) {
            $scope.modalTitle = "Edit Device";
            $scope.saveButtonText = "Save Changes";
        } else {
            $scope.modalTitle = "Add Device";
            $scope.saveButtonText = "Add Device";
        }

        $scope.savePhone = function () {
            $uibModalInstance.close(
                $scope.phone
            );
        };

        $scope.cancel = function () {
            $uibModalInstance.dismiss(
                "cancel"
            );
        };
    }
);