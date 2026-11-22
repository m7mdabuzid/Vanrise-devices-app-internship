phoneApp.service("ClientService", function ($http) {

    this.getAllClients = function () {
        return $http.get("/api/clients/GetAllClients");
    }
        ;

    this.getFilteredClients = function (name, type) {
        return $http.get("/api/clients/GetFilteredClients", {
            params: {
                name: name || "",
                type: type
            }
        });
    };

    this.addClient = function (client) {
        return $http.post("/api/clients/AddClient", client);
    }
        ;

    this.updateClient = function (client) {
        return $http.put("/api/clients/UpdateClient", client);
    }
        ;
    this.deleteClient = function (id) {
        return $http.delete(
            "/api/clients/DeleteClient/" + id
        );
    };

});