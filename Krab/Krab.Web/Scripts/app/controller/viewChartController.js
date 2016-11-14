angular
    .module("myApp.controllers")
    .controller("viewChartController", viewChartController);

function viewChartController($scope, modalService, $http) {

    $scope.hasError = false;
    $scope.errorMsg = "";
    $scope.isLoading = false;
    $scope.reportData = [];
    $scope.data = [
        {
            key: "Number of Responses",
            values: [
                {
                    "label": "hello",
                    "value": 30
                },
                {
                    "label": "world",
                    "value": 20
                }
            ]
        }
    ];
    $scope.options = {
        chart: {
            type: "discreteBarChart",
            height: 450,
            x: function(d) {
                return d.label;
            },
            y: function(d) {
                return d.value;
            },
            showValues: true,
            xAxis: {
                axisLabel: "Keyword Response Sets"
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