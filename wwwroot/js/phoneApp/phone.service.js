phoneApp.service("PhoneService", function ($http) {

    this.getAllPhones = function () {
        return $http.get("/api/devices/GetAllDevices");
    };

    this.getFilteredPhones = function (searchValue) {
        return $http.get("/api/devices/GetFilteredDevices", {
            params: {
                searchValue: searchValue
            }
        });
    };

    this.addPhone = function (phone) {
        return $http.post("/api/devices/AddDevice", phone);
    };

    this.updatePhone = function (phone) {
        return $http.put("/api/devices/UpdateDevice", phone);
    };

    this.deletePhone = function (id) {
        return $http.delete(
            "/api/devices/DeleteDevice/" + id
        );
    };

});