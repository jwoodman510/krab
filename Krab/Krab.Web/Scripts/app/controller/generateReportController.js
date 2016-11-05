angular
    .module("myApp.controllers")
    .controller("generateReportController", generateReportController);

function generateReportController($scope, modalService) {
    $scope.submit = function () {
        modalService.close();
    }

    $scope.cancel = function () {
        modalService.close();
    }
}