angular
    .module("myApp.controllers")
    .controller("viewTrendsController", viewTrendsController);

function viewTrendsController($scope, modalService, $http, $timeout) {

    $scope.hasError = false;
    $scope.errorMsg = "";
    $scope.isLoading = false;
    $scope.dateRanges = [
        {
            "id": "lastSevenDays",
            "label": "Last 7 Days",
            subtractDays: 6
        },
        {
            "id": "lastTwoWeeks",
            "label": "Last 14 Days",
            subtractDays: 13
        },
        {
            "id": "lastThirtyDays",
            "label": "Last 30 Days",
            subtractDays: 29
        },
        {
            "id": "lastSixtyDays",
            "label": "Last 60 Days",
            subtractDays: 59
        },
        {
            "id": "lastNinetyDays",
            "label": "Last 90 Days",
            subtractDays: 89
        }
    ];
    $scope.selectedDateRange = $scope.dateRanges[0];

    function getStartDate() {
        var startDate = new Date();
        startDate.setDate(startDate.getDate() - $scope.selectedDateRange.subtractDays);
        return startDate;
    }

    function getEndDate() {
        return new Date();
    }

    function setOptions() {
        $scope.options = {
            chart: {
                type: "lineWithFocusChart",
                height: 450,
                xAxis: {
                    axisLabel: "",
                    tickFormat: function(addDays) {
                        var date = new Date();
                        date.setDate(date.getDate() + addDays);
                        var day = date.getDate();
                        var month = date.getMonth() + 1;
                        return month + "/" + day;
                    }
                },
                x2Axis: {
                    axisLabel: "",tickFormat: function (addDays) {
                        var date = new Date();
                        date.setDate(date.getDate() + addDays);
                        var day = date.getDate();
                        var month = date.getMonth() + 1;
                        return month + "/" + day;
                    }
                },
                yAxis: {
                    axisLabel: "Number of Responses",
                    axisLabelDistance: -10
                }
            }
        };
    }

    $scope.$watch("isLoading", function () {
        if (!$scope.isLoading) {
            $timeout(function () {
                window.dispatchEvent(new Event("resize"));
            }, 500);
        }
    });

    $scope.$watch("selectedDateRange", function() {
        loadData();
    });

    function loadData() {
        $scope.hasError = false;
        $scope.isLoading = true;
        $scope.errorMsg = "";
        setOptions();

        var rptType = "Standard";
        var startDate = getStartDate();
        var endDate = getEndDate();

        $http.get("/api/keywordResponseSetReport?startDateMs=" + startDate.getTime() + "&endDateMs=" + endDate.getTime() + "&reportType=" + rptType)
            .success(function (data) {

                var reportData = [];
                var groupedKrSets = {};
                
                angular.forEach(data.result, function(rptData) {
                    if (!groupedKrSets[rptData.id]) {
                        groupedKrSets[rptData.id] = [];
                    }
                    groupedKrSets[rptData.id].push(rptData);
                });

                angular.forEach(groupedKrSets, function(krSetGrouping) {
                    var keyword = "";
                    var krResponses = [];

                    var subtractDays = $scope.selectedDateRange.subtractDays;

                    while (0 <= subtractDays) {
                        var thisDate = new Date();
                        thisDate.setDate(thisDate.getDate() - subtractDays);

                        var month = thisDate.getMonth();
                        var days = thisDate.getDate();
                        var numResponses = 0;

                        angular.forEach(krSetGrouping, function(krSetData) {
                            keyword = krSetData.keyword;
                            var d = new Date(krSetData.reportDateUtc + "Z");

                            if (d.getUTCMonth() === month && d.getUTCDate() === days) {
                                numResponses = numResponses + krSetData.numberOfResponses;
                            }
                        });

                        krResponses.push({
                            x: subtractDays * -1,
                            y: numResponses
                        });

                        subtractDays--;
                    }

                    reportData.push({
                        key: keyword,
                        values: krResponses
                    });
                });

                $scope.data = reportData;
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
    
    $scope.cancel = function () {
        modalService.close();
    }

    $scope.submit = function () {
        loadData();
    }
}