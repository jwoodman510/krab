angular
    .module("myApp", ['ui.grid', 'ngRoute', 'myApp.services'])
    .controller("KRController", krController)
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

function krController($scope, $http, krService, $location) {
    $scope.redditUserName = "";
    $scope.krSets = [];
    $scope.isRedditUserNameLoading = true;
    $scope.isKrSetsLoading = true;
    $scope.needsRedditAccount = false;
    $scope.selectedSet = "Select Keyword Response Set";
    $scope.isDeletedSetVisible = false;
    $scope.errorMessage = "";
    $scope.statuses = [
        {
            "id": 1,
            "label": "Active"
        },
        {
            "id": 2,
            "label": "Paused"
        }
    ];
    $scope.selectedStatus = $scope.statuses[0];

    $scope.gridOptions = {
        enableSorting: true,
        data: 'krSets',
        columnDefs: [
            { field: "keyword", displayName: "Keyword" },
            { field: "response", displayName: "Response" },
            { field: "status", displayName: "Status" }
        ]
    };

    $scope.$on('$locationChangeStart', function (next, last) {
        $scope.showAddEditDelete =
            $location.path() === '' ||
            $location.path() === '/#' ||
            $location.path() === '/';
    });

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
                $scope.errorMessage = "Unable to load data: " + error.message;
                $scope.isRedditUserNameLoading = false;
            });
    }

    function getRedditUserName() {
        $http.get("/api/reddituser")
            .success(function (response) {
                if (response.result.userName && response.result.userName.length > 0) {
                    $scope.redditUserName = "Reddit Username: " + response.result.userName;
                } else {
                    $scope.needsRedditAccount = true;
                }
                $scope.isKrSetsLoading = false;
            })
            .error(function (error) {
                $scope.isKrSetsLoading = false;
            });
    }

    $scope.addKrSet = function () {
        var krSet = {
            'Keyword': $scope.keyword,
            'Response': $scope.response,
            'StatusId': $scope.selectedStatus.id
        };

        krService.AddKrSet(krSet)
        .success(function (response) {
            $scope.goHome();
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

    $scope.goHome = function () {
        $location.path('/');
    }

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
