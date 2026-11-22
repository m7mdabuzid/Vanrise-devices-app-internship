phoneApp.service(
    "MessageService",
    function () {

        this.success = function (
            response,
            defaultMessage
        ) {
            var message = defaultMessage;

            if (
                response &&
                response.data &&
                response.data.message
            ) {
                message =
                    response.data.message;
            }

            alert(message);
        };

        this.error = function (
            error,
            defaultMessage
        ) {
            var message = defaultMessage;

            if (!error || !error.data) {
                alert(message);
                return;
            }

            if (typeof error.data === "string") {
                alert(error.data);
                return;
            }

            if (error.data.message) {
                alert(error.data.message);
                return;
            }

            if (error.data.errors) {

                var validationMessages = [];

                angular.forEach(
                    error.data.errors,
                    function (messages) {

                        angular.forEach(
                            messages,
                            function (item) {
                                validationMessages.push(item);
                            }
                        );
                    }
                );

                if (validationMessages.length > 0) {
                    message =
                        validationMessages.join("\n");
                }
            }

            alert(message);
        };
    }
);