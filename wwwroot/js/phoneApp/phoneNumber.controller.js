phoneApp.controller(
    "PhoneNumberController",
    function (
        $scope,
        $uibModal,
        PhoneNumberService,
        MessageService
    ) {

        $scope.phoneNumbers = [];
        $scope.visiblePhoneNumbers = [];
        $scope.devices = [];

        $scope.deviceFilterApi = {};

        $scope.numberFilter = "";
        $scope.deviceIdFilter = null;

        $scope.appliedNumberFilter = "";
        $scope.appliedDeviceIdFilter = null;

        function normalizeDevice(item) {
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

        function normalizePhoneNumber(item) {
            return {
                id:
                    item.id !== undefined
                        ? item.id
                        : item.Id,

                number:
                    item.number !== undefined
                        ? item.number
                        : item.Number,

                deviceId:
                    item.deviceId !== undefined
                        ? item.deviceId
                        : item.DeviceId,

                deviceName:
                    item.deviceName !== undefined
                        ? item.deviceName
                        : item.DeviceName
            };
        }

        function normalizePhoneNumbers(data) {
            return data.map(function (item) {
                return normalizePhoneNumber(item);
            });
        }

        $scope.loadDevices = function () {

            PhoneNumberService.getDevices()
                .then(function (response) {

                    $scope.devices =
                        response.data.map(
                            function (item) {
                                return normalizeDevice(item);
                            }
                        );

                }, function (error) {

                    console.log(error);

                    MessageService.error(
                        error,
                        "Devices could not be loaded."
                    );
                });
        };

        $scope.loadPhoneNumbers = function () {

            PhoneNumberService
                .getAllPhoneNumbers()
                .then(function (response) {

                    $scope.phoneNumbers =
                        normalizePhoneNumbers(
                            response.data
                        );

                    $scope.visiblePhoneNumbers =
                        angular.copy(
                            $scope.phoneNumbers
                        );

                }, function (error) {

                    console.log(error);

                    MessageService.error(
                        error,
                        "Phone numbers could not be loaded."
                    );
                });
        };

        $scope.loadFilteredPhoneNumbers =
            function () {

                PhoneNumberService
                    .getFilteredPhoneNumbers(
                        $scope.appliedNumberFilter,
                        $scope.appliedDeviceIdFilter
                    )
                    .then(function (response) {

                        $scope.visiblePhoneNumbers =
                            normalizePhoneNumbers(
                                response.data
                            );

                    }, function (error) {

                        console.log(error);

                        MessageService.error(
                            error,
                            "Phone numbers could not be filtered."
                        );
                    });
            };

        $scope.applyFilter = function () {

            $scope.appliedNumberFilter =
                $scope.numberFilter || "";

            if (
                $scope.deviceFilterApi &&
                $scope.deviceFilterApi.getData
            ) {
                $scope.appliedDeviceIdFilter =
                    $scope.deviceFilterApi.getData();
            } else {
                $scope.appliedDeviceIdFilter =
                    $scope.deviceIdFilter;
            }

            $scope.loadFilteredPhoneNumbers();
        };

        $scope.clearFilter = function () {

            $scope.numberFilter = "";
            $scope.deviceIdFilter = null;

            $scope.appliedNumberFilter = "";
            $scope.appliedDeviceIdFilter = null;

            $scope.visiblePhoneNumbers =
                angular.copy(
                    $scope.phoneNumbers
                );
        };

        $scope.reloadGridAfterSave = function () {

            PhoneNumberService
                .getAllPhoneNumbers()
                .then(function (response) {

                    $scope.phoneNumbers =
                        normalizePhoneNumbers(
                            response.data
                        );

                    if (
                        ($scope.appliedNumberFilter || "")
                            .trim() === "" &&
                        (
                            $scope.appliedDeviceIdFilter === null ||
                            $scope.appliedDeviceIdFilter === undefined
                        )
                    ) {
                        $scope.visiblePhoneNumbers =
                            angular.copy(
                                $scope.phoneNumbers
                            );
                    } else {
                        $scope.loadFilteredPhoneNumbers();
                    }

                }, function (error) {

                    console.log(error);

                    MessageService.error(
                        error,
                        "Phone numbers could not be reloaded."
                    );
                });
        };

        $scope.openPhoneNumberModal =
            function (selectedPhoneNumber) {

                var isEditMode =
                    selectedPhoneNumber !== undefined &&
                    selectedPhoneNumber !== null;

                var modalInstance =
                    $uibModal.open({
                        templateUrl:
                            "/templates/phoneNumberModal.html",

                        controller:
                            "PhoneNumberModalController",

                        resolve: {
                            phoneNumberData: function () {

                                if (isEditMode) {
                                    return angular.copy(
                                        selectedPhoneNumber
                                    );
                                }

                                return {
                                    id: 0,
                                    number: "",
                                    deviceId: null
                                };
                            },

                            isEdit: function () {
                                return isEditMode;
                            },

                            devices: function () {
                                return $scope.devices;
                            }
                        }
                    });

                modalInstance.result.then(
                    function (savedPhoneNumber) {

                        if (isEditMode) {

                            PhoneNumberService
                                .updatePhoneNumber(
                                    savedPhoneNumber
                                )
                                .then(
                                    function (response) {

                                        MessageService.success(
                                            response,
                                            "Phone number updated successfully."
                                        );

                                        $scope
                                            .reloadGridAfterSave();
                                    },
                                    function (error) {

                                        console.log(error);

                                        MessageService.error(
                                            error,
                                            "Phone number could not be updated."
                                        );
                                    }
                                );

                        } else {

                            PhoneNumberService
                                .addPhoneNumber(
                                    savedPhoneNumber
                                )
                                .then(
                                    function (response) {

                                        MessageService.success(
                                            response,
                                            "Phone number added successfully."
                                        );

                                        $scope
                                            .reloadGridAfterSave();
                                    },
                                    function (error) {

                                        console.log(error);

                                        MessageService.error(
                                            error,
                                            "Phone number could not be added."
                                        );
                                    }
                                );
                        }
                    }
                );
            };

        $scope.deletePhoneNumber =
            function (phoneNumber) {

                if (
                    !confirm(
                        "Delete this phone number?"
                    )
                ) {
                    return;
                }

                PhoneNumberService
                    .deletePhoneNumber(
                        phoneNumber.id
                    )
                    .then(function (response) {

                        MessageService.success(
                            response,
                            "Phone number deleted successfully."
                        );

                        $scope.reloadGridAfterSave();

                    }, function (error) {

                        console.log(error);

                        MessageService.error(
                            error,
                            "Phone number could not be deleted."
                        );
                    });
            };

        $scope.loadDevices();
        $scope.loadPhoneNumbers();
    }
);


phoneApp.controller(
    "PhoneNumberModalController",
    function (
        $scope,
        $uibModalInstance,
        phoneNumberData,
        isEdit,
        devices
    ) {

        $scope.phoneNumber =
            phoneNumberData;

        $scope.isEdit = isEdit;
        $scope.devices = devices;

        $scope.deviceEditorApi = {};

        if (isEdit) {
            $scope.modalTitle =
                "Edit Phone Number";

            $scope.saveButtonText =
                "Save Changes";
        } else {
            $scope.modalTitle =
                "Add Phone Number";

            $scope.saveButtonText =
                "Add Phone Number";
        }

        $scope.savePhoneNumber = function () {

            var selectedDeviceId =
                $scope.deviceEditorApi &&
                    $scope.deviceEditorApi.getData
                    ? $scope.deviceEditorApi.getData()
                    : $scope.phoneNumber.deviceId;

            if (!selectedDeviceId) {
                return;
            }

            $scope.phoneNumber.deviceId =
                selectedDeviceId;

            $uibModalInstance.close(
                $scope.phoneNumber
            );
        };

        $scope.cancel = function () {

            $uibModalInstance.dismiss(
                "cancel"
            );
        };
    }
);