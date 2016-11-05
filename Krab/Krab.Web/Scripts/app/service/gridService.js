angular
    .module("myApp.services")
    .factory("gridService", gridService);

function gridService($rootScope) {
    var svc = this;
    var selectedRow = null;

    svc.krDataChanged = "KR_DATA_CHANGED";
    
    svc.refreshGrid = function() {
        $rootScope.$broadcast(svc.krDataChanged);
    }

    svc.getSelectedRow = function() {
        return selectedRow;
    }

    svc.setSelectedRow = function(row) {
        selectedRow = row;
    }

    return svc;
}