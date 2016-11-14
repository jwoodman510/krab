angular
    .module("myApp.controllers")
    .controller("viewChartController", viewChartController);

function viewChartController($scope, modalService, $http, $timeout) {

    $scope.hasError = false;
    $scope.errorMsg = "";
    $scope.isLoading = false;
    $scope.reportData = [];

    $scope.options = {
        chart: {
            type: "discreteBarChart",
            height: 450,
            x: function (d) {
                return d.label;
            },
            y: function (d) {
                return d.value;
            },
            showValues: true,
            xAxis: {
                axisLabel: ""
            },
            yAxis: {
                axisLabel: "Number of Responses",
                axisLabelDistance: -10
            }
        }
    };

    function init() {
        var sixDaysAgo = new Date();
        sixDaysAgo.setDate(sixDaysAgo.getDate() - 6);
        $scope.startDate = sixDaysAgo;
        $scope.endDate = new Date();
        loadData();
    };

    init();

    $scope.$watch("isLoading", function () {
        if (!$scope.isLoading) {
            $timeout(function () {
                window.dispatchEvent(new Event("resize"));
            }, 500);
        }
    });

    function loadData() {
        $scope.hasError = false;
        $scope.isLoading = true;
        $scope.errorMsg = "";

        var rptType = "StandardAggregate";

        $http.get("/api/keywordResponseSetReport?startDateMs=" + $scope.startDate.getTime() + "&endDateMs=" + $scope.endDate.getTime() + "&reportType=" + rptType)
            .success(function (data) {

                var rptRows = [];

                angular.forEach(data.result, function (value) {
                    rptRows.push({
                        "label": value.keyword,
                        "value": value.numberOfResponses
                    });
                });

                $scope.data = [
                    {
                        key: "Number of Responses",
                        values: rptRows
                    }
                ];

                $scope.hasError = false;
                $scope.errorMsg = "";
            })
            .error(function () {
                $scope.errorMsg = "An error occured.";
                $scope.hasError = true;
            })
            .finally(function() {
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

    $scope.submit = function () {
        loadData();
    }

    $scope.isValid = function () {
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
               ($scope.startDate >= minDate)) &&
                $scope.isLoading === false;
    }
}