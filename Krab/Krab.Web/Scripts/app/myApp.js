﻿angular
    .module("myApp", ["ui.grid", "ui.grid.selection", "ngRoute", "myApp.services", "myApp.controllers", "ui.bootstrap"])
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
