﻿var myApp = angular.module("myApp", ["ui.grid"]);

myApp.controller("KRController", function ($scope, krService) {

    getKeyWordResponseSet();

    function getKeyWordResponseSet() {
        krService.getKeyWordResponseSet()
            .success(function(response) {
                $scope.krSets = response;
                console.log($scope.krSets);
            })
            .error(function(error) {
                $scope.krSets = "Unable to load data: " + error.message;
                console.log($scope.krSets);
            });
    }

});



myApp.factory("krService",
[
    "$http", function ($http) {

        var krService = {};
        var urlBase = "http://localhost:44497/api";
        krService.getKeyWordResponseSet = function () {
            return $http.get(urlBase + "/keywordresponsesets");
        };
        return krService;
    }
]);

































////
////angular.module("myApp", [])
////    .controller("KeywordResponseSetsController",
////        function ($scope, $http) {
////            $http.get('http://localhost:38572/api/keywordresponsesets')
////            then(function (response) {
////                console.log(response);
////                $scope.keywordResponseSets = response.data;
////            });
////
////
////        });
//
//        var myApp = angular.module("myApp", []);
////
////
////        myApp.controller("KeywordResponseSetsController",['$scope',
////            function($scope) {
////
////                $(function() {
////                    getKeywordResponseSets();
////                });
////
////
////                function getKeywordResponseSets() {
////                    $http.get('http://localhost:38572/api/keywordresponsesets');
////                    then(function(response) {
////                        $scope.keywordResponseSets = response.data;
////                    });
////                });
////
////            }]);
//
//
//        myApp.controller('KeywordResponseSetsController',
//        [
//            '$scope','$http',
//            function($scope,$http) {
//                var keywordResponseSets = [];
//
//                $http.get('http://localhost:44497/api/keywordresponsesets').
//                                   then(function(response) {
//                                       $scope.keywordResponseSets = response.data;
//                                   });
//            }
//        ]);