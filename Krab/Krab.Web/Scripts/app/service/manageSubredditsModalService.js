angular
    .module("myApp.services")
    .factory("manageSubredditsModalService", manageSubredditsModalService);

function manageSubredditsModalService($location, $uibModal) {
    var svc = this;
    var krSet = null;
    var modalInstance = null;
    
    svc.getKeywordResponseSet = function () {
        return krSet;
    }

    svc.setKeywordResponseSet = function(set) {
        krSet = set;
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