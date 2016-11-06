angular
    .module("myApp.controllers")
    .controller("generateReportController", generateReportController);

function generateReportController($scope, modalService, $http, $timeout) {

    $scope.format = "dd-MMMM-yyyy";
    $scope.hasError = false;
    $scope.errorMsg = "";
    $scope.isLoading = false;

    function init() {
        var sixDaysAgo = new Date();
        sixDaysAgo.setDate(sixDaysAgo.getDate() - 6);
        $scope.startDate = sixDaysAgo;

        $scope.endDate = new Date();;
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

    $scope.getHeader = function() {
        return [
            "Id",
            "Report Date UTC",
            "Keyword",
            "Response",
            "Number of Responses"
        ];
    }

    $scope.submit = function () {
        $scope.hasError = false;
        $scope.isLoading = true;
        $scope.errorMsg = "";

        $http.get("/api/keywordResponseSetReport?startDateMs=" + $scope.startDate.getTime() + "&endDateMs=" + $scope.endDate.getTime())
            .success(function (data) {

                $scope.reportData = data.result;

                if (data.result && data.result.length > 0) {
                    $timeout(function() {
                        angular.element("#downloadCsvButton").triggerHandler("click");
                    });

                    $scope.isLoading = false;
                    modalService.close();
                } else {
                    $scope.errorMsg = "No data found.";
                    $scope.hasError = true;
                    $scope.isLoading = false;
                }
            })
            .error(function () {
                $scope.errorMsg = "An error occured.";
                $scope.hasError = true;
                $scope.isLoading = false;
            });
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