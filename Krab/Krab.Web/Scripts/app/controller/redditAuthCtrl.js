angular
    .module("myApp")
    .controller("RedditAuthController", redditAuthController);

function redditAuthController($scope, $http, $window) {

    $scope.linkRedditAccount = linkRedditAccount;

    $scope.isRedirect = false;

    function linkRedditAccount() {

        if ($scope.isRedirect) {
            return;
        }

        $scope.error = "";
        $scope.isRedirect = true;

        $http.get("/api/RedditAuthorization").success(function (data) {
            $window.location.href = data.result;
            $scope.isRedirect = false;
        }).error(function () {
            $scope.error = "Sorry, something when wrong while adding your account...";
            $scope.isRedirect = false;
        });
    }
}