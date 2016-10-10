angular
    .module("myApp")
    .controller("RedditAuthController", redditAuthController);

function redditAuthController($scope, $http, $window) {

    $scope.linkRedditAccount = linkRedditAccount;
    $scope.unlinkRedditAccount = unlinkRedditAccount;

    $scope.isLoading = false;
    $scope.isUnlinkComplete = false;
    $scope.linkLabel = "Link";

    function linkRedditAccount() {

        if ($scope.isLoading) {
            return;
        }

        $scope.error = "";
        $scope.isLoading = true;

        $http.get("/api/RedditAuthorization").success(function (data) {
            $window.location.href = data.result;
            $scope.isLoading = false;
        }).error(function () {
            $scope.error = "Sorry, something when wrong while linking your account...";
            $scope.isLoading = false;
        });
    }

    function unlinkRedditAccount() {

        if ($scope.isLoading) {
            return;
        }

        $scope.error = "";
        $scope.isLoading = true;

        $http.delete("/api/RedditAuthorization/UnlinkRedditAccount").success(function (data) {
            $scope.isLoading = false;
            $scope.isUnlinkComplete = true;
        }).error(function () {
            $scope.error = "Sorry, something when wrong while unlinking your account...";
            $scope.isLoading = false;
            $scope.isUnlinkComplete = true;
        });
    }
}