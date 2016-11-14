angular
    .module("myApp.controllers")
    .controller("viewChartController", viewChartController);

function viewChartController($scope, modalService, $http) {

    $scope.hasError = false;
    $scope.errorMsg = "";
    $scope.isLoading = false;
    $scope.reportData = [];

    function init() {
        var sixDaysAgo = new Date();
        sixDaysAgo.setDate(sixDaysAgo.getDate() - 6);
        $scope.startDate = sixDaysAgo;
        $scope.endDate = new Date();
        loadData();
    };

    init();

    function loadData() {
        $scope.hasError = false;
        $scope.isLoading = true;
        $scope.errorMsg = "";

        var rptType = ($scope.breakoutBySubreddit && $scope.breakoutBySubreddit === true)
            ? "subreddit"
            : "standard";

        $http.get("/api/keywordResponseSetReport?startDateMs=" + $scope.startDate.getTime() + "&endDateMs=" + $scope.endDate.getTime() + "&reportType=" + rptType)
            .success(function (data) {
                $scope.reportData = data.result;
                $scope.hasError = false;
                $scope.errorMsg = "";
                $scope.isLoading = false;
            })
            .error(function (error) {
                $scope.errorMsg = "An error occured.";
                $scope.hasError = true;
                $scope.isLoading = false;
            });
    }

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
    
    $scope.cancel = function () {
        modalService.close();
    }
}