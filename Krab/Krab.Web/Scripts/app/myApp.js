angular
    .module("myApp", ['ui.grid', 'ngRoute'])
    .controller("KRController", krController)
    .factory("krService", krService)
    .controller("AddController", addController)
     .config(function ($routeProvider) {
         $routeProvider
             .when("/Add", {
                 templateUrl: "Function_Views/add.html",
                 controller: "AddController"
             })
             .otherwise({
                 redirectTo: "/"
             });
     });

// i feel like this can be part of the same controller as the grid.. but either way works.
function addController($scope, krService) {
    $scope.addKrSet = function () {
        var krToAdd = {
            'Keyword': $scope.keyword,
            'Response': $scope.response,
            'StatusId': $scope.statusId
        };
        krService.AddKrSet(krToAdd)
        .success(function (response) {
            alert("Keyword Response Set Added!");
            // instead of an alert, we can just show/hide a label to the user. Most people have AdBlock now.
            $scope.keyword = undefined;
            $scope.response = undefined;
            $scope.statusId = undefined;
            // we need to refresh the grid here with the new data.
        })
        .error(function (response) {
            alert("Error in Adding");
            // instead of an alert, we can just show/hide an error label to the user. Most people have AdBlock now.
        });
    }
};


function krController($scope, krService, $http) {

    $scope.redditUserName = "";
    $scope.krSets = [];
    $scope.isRedditUserNameLoading = true;
    $scope.isKrSetsLoading = true;
    $scope.needsRedditAccount = false;

    $scope.gridOptions = { enableFiltering: true, data: "krSets" };

    init();

    function init() {
        getRedditUserName();
        getKeywordResponseSets();
    }

    function getKeywordResponseSets() {
        krService.getByUserId()
            .success(function (response) { 
                $scope.krSets = response.result;
                $scope.isRedditUserNameLoading = false;
            })
            .error(function (error) {
                $scope.krSets = "Unable to load data: " + error.message;
                console.log($scope.krSets);
                $scope.isRedditUserNameLoading = false;
            });
        }

    function getRedditUserName() {
        $http.get("/api/reddituser")
            .success(function (response) {
                if (response.result.userName && response.result.userName.length > 0) {
                    $scope.redditUserName = "Reddit Username: " + response.result.userName;
                    $scope.gridOptions.columnDefs = [
                   { field: "keyword", displayName: "Keyword" },
                   { field: "response", displayName: "Response" },
                   { field: "status", displayName: "Status"}
                    ];
                } else {
                    $scope.needsRedditAccount = true;
                }
                $scope.isKrSetsLoading = false;
            })
            .error(function (error) {
                console.log("Unable to load reddit user name: " + error.message);
                $scope.isKrSetsLoading = false;
            });
    }
}

// this can go in its own file. we can make a "service" folder under the app folder in scripts.
function krService($http) {
    var svc = this;

    svc.getByUserId = function () {
        return $http.get("/api/keywordresponsesets");
    };

    svc.AddKrSet = function (sets) {
        return $http.post("/api/keywordresponsesets", sets);
    };

    return svc;
}
