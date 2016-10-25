angular
    .module("myApp", ['ui.grid', 'ngRoute'])
    .controller("KRController", krController)
    .factory("krService", krService)
     .config(function ($routeProvider) {
         $routeProvider
             .when("/Add", {
                 templateUrl: "Function_Views/add.html",
                 controller: "KRController"
             })
             .when("/Edit", {
                 templateUrl: "Function_Views/edit.html",
                 controller: "KRController"
             })
             .when("/Delete", {
                 templateUrl: "Function_Views/delete.html",
                 controller: "KRController"
             })
             .otherwise({
                 redirectTo: "/"
             });
});

function krController($scope, krService, $http) {

    $scope.redditUserName = "";
    $scope.krSets = [];
    $scope.isRedditUserNameLoading = true;
    $scope.isKrSetsLoading = true;
    $scope.needsRedditAccount = false;

    $scope.selectedSet = "Select Keyword Response Set";
    $scope.isDeletedSetVisible = false;

    $scope.gridOptions = { enableFiltering: true, data: "krSets" };

    var refresh = function () {
        $scope.refresh = true;
        $timeout(function () {
            $scope.refresh = false;
        }, 0);
    };

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
                   { field: "status", displayName: "Status" }
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

    $scope.addKrSet = function () {
        var krToAdd = {
            'Keyword': $scope.keyword,
            'Response': $scope.response,
            'StatusId': $scope.statusId
        };
        krService.AddKrSet(krToAdd)
        .success(function (response) {
            alert("Keyword Response Set Added!");
            $scope.keyword = undefined;
            $scope.response = undefined;
            $scope.statusId = undefined;
            getKeywordResponseSets();
            console.log($scope.krSets);

            // we need to refresh the grid here with the new data -> Having a tough time figuring this out. 
        })
        .error(function (response) {
            alert("Error in Adding");
            // instead of an alert, we can just show/hide an error label to the user. Most people have AdBlock now.
        });
    }

    $scope.dropboxitemselected = function (data) {
        $scope.isDeletedSetVisible = true;
        $scope.selectedSet = data.id;
        $scope.keyword = data.keyword;
        $scope.response = data.response;
        $scope.statusId = data.statusId;
    };

    $scope.UpdateKrSet = function () {
        var setsToUpdate = {
            'Id': $scope.selectedSet,
            'Keyword': $scope.keyword,
            'Response': $scope.response,
            'StatusId': $scope.statusId
        };
        console.log(setsToUpdate);

        krService.EditKrSet(setsToUpdate)
        .success(function (response) {
            alert("KRSet Updated!");
            $scope.keyword = undefined;
            $scope.response = undefined;
            $scope.statusId = undefined;
            $scope.selectedSet = "Select Keyword Response Set"
            $scope.isDeletedSetVisible = false;
            getKeywordResponseSets();
        })
        .error(function (response) {
            alert("Error in Updating");
        });
    }

    $scope.DeleteSet = function () {
        console.log("Deleting: " + $scope.selectedSet.toString());
        
        krService.DeleteKrSet($scope.selectedSet)
           .success(function (response) {
               alert("KRSet Deleted!");
               $scope.keyword = undefined;
               $scope.response = undefined;
               $scope.statusId = undefined;
                $scope.selectedSet = "Select Keyword Response Set";
               $scope.isDeletedSetVisible = false;
               getKeywordResponseSets();
           })
           .error(function (response) {
               alert("Error in Deleting");
           });
    }
}

// this can go in its own file. we can make a "service" folder under the app folder in scripts -> Will move it soon. 
function krService($http) {
    var svc = this;

    svc.getByUserId = function () {
        return $http.get("/api/keywordresponsesets");
    };

    svc.AddKrSet = function (sets) {
        return $http.post("/api/keywordresponsesets", sets);
    };

    svc.EditKrSet = function (setsToUpdate) {
        return $http.put("/api/keywordresponsesets", setsToUpdate);
    };

    svc.DeleteKrSet = function (keywordResponseSetId) {
        return $http.delete("/api/keywordresponsesets/deleteKeywordResponseSets?keywordResponseSetId=" + keywordResponseSetId.toString());
    };
    return svc;
}
