angular
    .module("myApp", ["ui.grid", "ui.grid.selection", "ngRoute", "myApp.services", "myApp.controllers"])
    .config(function ($routeProvider) {
         $routeProvider
             .when("/Add", {
                 templateUrl: "Function_Views/add.html",
                 controller: "krController"
             })
             .when("/Edit", {
                 templateUrl: "Function_Views/edit.html",
                 controller: "editKrController"
             })
             .otherwise({
                 redirectTo: "/"
             });
});
