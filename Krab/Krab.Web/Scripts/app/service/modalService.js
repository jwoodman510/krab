angular
    .module("myApp.services")
    .factory("modalService", modalService);

function modalService($location, $uibModal) {
    var svc = this;
    var data = null;
    var modalInstance = null;
    
    svc.getData = function () {
        return data;
    }

    svc.setData = function(input) {
        data = input;
    }

    svc.open = function (options) {
        modalInstance = $uibModal.open(options);
    }

    svc.close = function () {
        modalInstance.close();
        modalInstance = null;
    }

    return svc;
}