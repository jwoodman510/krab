angular
    .module("myApp.controllers", [])
    .controller("krController", krController);

function krController($rootScope, $scope, $http, krService, $location) {
    $scope.krSets = [];
    $scope.redditUserName = "";
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
    $scope.krDataChanged = 'KR_DATA_CHANGED';
    $scope.hasAddError = false;
    $scope.isAdding = false;

    init();

    function init() {
        getRedditUserName();
        getKeywordResponseSets();
    }

    function refreshGrid() {
        $rootScope.$broadcast($scope.krDataChanged);
    }

    $scope.gridOptions = {
        data: 'krSets',
        enableSorting: true,
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

    $scope.$on($scope.krDataChanged, function (event, args) {
        getKeywordResponseSets();
    });

    function getKeywordResponseSets() {
        krService.getByUserId()
            .success(function (response) {
                $scope.krSets = response.result;
                $scope.isKrSetsLoading = false;
            })
            .error(function (error) {
                $scope.errorMessage = "Unable to load data: " + error.message;
                $scope.isKrSetsLoading = false;
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
                $scope.isRedditUserNameLoading = false;
            })
            .error(function (error) {
                $scope.isRedditUserNameLoading = false;
            });
    }

    $scope.addKrSet = function () {
        $scope.isAdding = true;
        $scope.hasAddError = false;
        krService.AddKrSet({
            'Keyword': $scope.keyword,
            'Response': $scope.response,
            'StatusId': $scope.selectedStatus.id
        }).success(function (response) {
            refreshGrid();
            goHome();
            $scope.isAdding = false;
        }).error(function (response) {
            $scope.hasAddError = true;
            $scope.isAdding = false;
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