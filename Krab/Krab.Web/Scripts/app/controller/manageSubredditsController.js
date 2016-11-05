angular
    .module("myApp.controllers")
    .controller("manageSubredditsController", manageSubredditsController);

function manageSubredditsController($scope, manageSubredditsModalService, krService) {
    $scope.subreddit1 = null;
    $scope.subreddit2 = null;
    $scope.subreddit3 = null;
    $scope.subreddit4 = null;
    $scope.subreddit5 = null;
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
                        $scope.subreddit1 = value.subredditName;
                    }
                    else if (count === 2) {
                        $scope.subreddit2 = value.subredditName;
                    }
                    else if (count === 3) {
                        $scope.subreddit3 = value.subredditName;
                    }
                    else if (count === 4) {
                        $scope.subreddit4 = value.subredditName;
                    }
                    else if (count === 5) {
                        $scope.subreddit5 = value.subredditName;
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
        var subredditNames = [
            $scope.subreddit1,
            $scope.subreddit1 && $scope.subreddit1.length > 0
                ? $scope.subreddit2
                : "",
            $scope.subreddit1 && $scope.subreddit1.length > 0 && $scope.subreddit2 && $scope.subreddit2.length > 0
                ? $scope.subreddit3
                : "",
            $scope.subreddit1 && $scope.subreddit1.length > 0 && $scope.subreddit2 && $scope.subreddit2.length > 0 && $scope.subreddit3 && $scope.subreddit3.length > 0
                ? $scope.subreddit4
                : "",
            $scope.subreddit1 && $scope.subreddit1.length > 0 && $scope.subreddit2 && $scope.subreddit2.length > 0 && $scope.subreddit3 && $scope.subreddit3.length > 0 && $scope.subreddit4 && $scope.subreddit4.length > 0
                ? $scope.subreddit5
                : ""
        ];

        krService.updateSubreddits($scope.krSet.id, subredditNames)
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