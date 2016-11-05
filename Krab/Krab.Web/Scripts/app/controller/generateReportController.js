angular
    .module("myApp.controllers")
    .controller("generateReportController", generateReportController);

function generateReportController($scope, modalService) {

    $scope.format = "dd-MMMM-yyyy";

    function init() {
        var yesterday = new Date();
        yesterday.setDate(yesterday.getDate() - 1);
        $scope.endDate = yesterday;
        $scope.startDate = yesterday;
    };

    init();

    $scope.startDatePopup = {
        opened: false
    };

    $scope.openStartDate = function () {
        $scope.startDatePopup.opened = true;
    };

    $scope.endDatePopup = {
        opened: false
    };

    $scope.openEndDate = function () {
        $scope.endDatePopup.opened = true;
    };

    $scope.submit = function () {
        modalService.close();
    }

    $scope.cancel = function () {
        modalService.close();
    }

    $scope.isValid = function() {
        var today = new Date();
        var minDate = new Date();
        minDate.setDate(minDate.getDate() - 368);

        if (!$scope.endDate || $scope.endDate === null || $scope.endDate === "") {
            return false;
        }

        if (!$scope.startDate || $scope.startDate === null || $scope.startDate === "") {
            return false;
        }

        return ($scope.endDate <= today) &&
               ($scope.endDate >= $scope.startDate &&
               ($scope.startDate >= minDate));
    }
}