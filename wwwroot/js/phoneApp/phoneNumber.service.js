phoneApp.service("PhoneNumberService", function ($http) {

    this.getAllPhoneNumbers = function () {
        return $http.get("/api/phonenumbers/GetAllPhoneNumbers");
    };

    this.getFilteredPhoneNumbers = function (number, deviceId) {
        return $http.get("/api/phonenumbers/GetFilteredPhoneNumbers", {
            params: {
                number: number || "",
                deviceId: deviceId
            }
        });
    };

    this.addPhoneNumber = function (phoneNumber) {
        return $http.post("/api/phonenumbers/AddPhoneNumber", phoneNumber);
    };

    this.updatePhoneNumber = function (phoneNumber) {
        return $http.put("/api/phonenumbers/UpdatePhoneNumber", phoneNumber);
    };

    this.getDevices = function () {
        return $http.get("/api/devices/GetAllDevices");
    };

    this.deletePhoneNumber = function (id) {
        return $http.delete(
            "/api/phonenumbers/DeletePhoneNumber/" + id
        );
    };

});