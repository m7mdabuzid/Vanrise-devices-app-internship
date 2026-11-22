phoneApp.service(
    "ReportService",
    function ($http) {

        this.getClientsPerType = function (type) {

            var url =
                "/api/reports/GetClientsPerType";

            if (
                type !== null &&
                type !== undefined &&
                type !== ""
            ) {
                url += "?type=" + type;
            }

            return $http.get(url);
        };

        this.getPhoneNumbersPerDevice =
            function (deviceId, status) {

                var url =
                    "/api/reports/GetPhoneNumbersPerDevice";

                var parameters = [];

                if (
                    deviceId !== null &&
                    deviceId !== undefined &&
                    deviceId !== ""
                ) {
                    parameters.push(
                        "deviceId=" + deviceId
                    );
                }

                if (
                    status !== null &&
                    status !== undefined &&
                    status !== ""
                ) {
                    parameters.push(
                        "status=" + status
                    );
                }

                if (parameters.length > 0) {
                    url += "?" + parameters.join("&");
                }

                return $http.get(url);
            };
    }
);