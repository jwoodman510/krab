angular
    .module("myApp.controllers")
    .controller("manageSubredditsController", manageSubredditsController);

function manageSubredditsController($scope, manageSubredditsModalService, krService) {
    $scope.subredditNames = [
        "",
        "",
        "",
        "",
        ""
    ];
    $scope.hasError = false;
    $scope.errorMsg = "";
    $scope.isLoading = true;

    init();

    function init() {
        $scope.krSet = manageSubredditsModalService.getKeywordResponseSet();
        $scope.keyword = $scope.krSet.keyword;
        krService.getSubreddits($scope.krSet.id)
            .success(function (data) {
                $scope.subreddits = data.result;
                var count = 1;
                angular.forEach($scope.subreddits, function (value) {
                    if (count === 1) {
                        $scope.subredditNames[0] = value.subredditName;
                    }
                    else if (count === 2) {
                        $scope.subredditNames[1] = value.subredditName;
                    }
                    else if (count === 3) {
                        $scope.subredditNames[2] = value.subredditName;
                    }
                    else if (count === 4) {
                        $scope.subredditNames[3] = value.subredditName;
                    }
                    else if (count === 5) {
                        $scope.subredditNames[4] = value.subredditName;
                    }
                    count++;
                });
                $scope.isLoading = false;
            })
            .error(function() {
                $scope.errorMsg = "Failed to load subreddits.";
                $scope.hasError = true;
                $scope.isLoading = false;
            });
    }

    $scope.save = function () {
        $scope.isLoading = true;
        $scope.errorMsg = "";
        $scope.hasError = false;

        krService.updateSubreddits($scope.krSet.id, $scope.subredditNames)
            .success(function () {
                $scope.isLoading = false;
                manageSubredditsModalService.close();
            })
            .error(function () {
                $scope.errorMsg = "An error occured.";
                $scope.hasError = true;
                $scope.isLoading = false;
            });
    }

    $scope.cancel = function () {
        manageSubredditsModalService.close();
    }
}