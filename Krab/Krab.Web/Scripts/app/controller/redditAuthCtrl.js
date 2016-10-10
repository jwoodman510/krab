angular
    .module("myApp")
    .controller("RedditAuthController", redditAuthController);

function redditAuthController($scope, $http) {

    $scope.test = test;

    function test() {
        var abc = 123;
    }
}