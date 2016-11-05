angular
    .module("myApp.services")
    .factory("locationService", locationService);

function locationService($location) {
    var svc = this;

    svc.goHome = function() {
        $location.path("/");
    }

    return svc;
}