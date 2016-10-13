angular
    .module("myApp", ["ui.grid"])
    .controller("KRController", krController)
    .factory("krService", krService);

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

    function krService($http) {
        var svc = this;

        svc.getByUserId = function () {
            return $http.get("/api/keywordresponsesets");
        };

        return svc;
    }
