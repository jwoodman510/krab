angular
    .module("myApp", [
        "ui.grid",
        "ui.grid.selection",
        "ui.grid.resizeColumns",
        "ngRoute",
        "myApp.services",
        "myApp.controllers",
        "ui.bootstrap",
        "ngSanitize",
        "ngCsv",
        "nvd3"
    ])
    .config(function ($routeProvider) {
         $routeProvider
             .when("/Add", {
                 templateUrl: "Function_Views/add.html",
                 controller: "addKrController"
             })
             .when("/Edit", {
                 templateUrl: "Function_Views/edit.html",
                 controller: "editKrController"
             })
             .otherwise({
                 redirectTo: "/"
             });
});
