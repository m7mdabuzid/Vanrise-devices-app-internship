phoneApp.service(
    "PhoneNumberReservationService",
    function ($http) {

        this.getAllReservations = function () {
            return $http.get(
                "/api/phonenumberreservations/GetAllPhoneNumberReservations"
            );
        };

        this.getFilteredReservations = function (
            clientId,
            phoneNumberId) {

            return $http.get(
                "/api/phonenumberreservations/GetFilteredPhoneNumberReservations",
                {
                    params: {
                        clientId: clientId,
                        phoneNumberId: phoneNumberId
                    }
                }
            );
        };

        this.getClients = function () {
            return $http.get(
                "/api/clients/GetAllClients"
            );
        };

        this.getPhoneNumbers = function () {
            return $http.get(
                "/api/phonenumbers/GetAllPhoneNumbers"
            );
        };

        this.getAvailablePhoneNumbers = function () {
            return $http.get(
                "/api/phonenumberreservations/GetAvailablePhoneNumbers"
            );
        };

        this.getActivePhoneNumbersByClient = function (clientId) {
            return $http.get(
                "/api/phonenumberreservations/GetActivePhoneNumbersByClient/" +
                clientId
            );
        };

        this.reservePhoneNumber = function (data) {
            return $http.post(
                "/api/phonenumberreservations/ReservePhoneNumber",
                data
            );
        };

        this.unreservePhoneNumber = function (data) {
            return $http.put(
                "/api/phonenumberreservations/UnreservePhoneNumber",
                data
            );
        };
    }
);