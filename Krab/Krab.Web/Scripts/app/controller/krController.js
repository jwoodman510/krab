angular
    .module("myApp.controllers")
    .controller("krController", krController);

function krController($rootScope, $scope, $http, krService, $location) {
    $scope.krSets = [];
    $scope.redditUserName = "";
    $scope.isRedditUserNameLoading = true;
    $scope.isKrSetsLoading = true;
    $scope.needsRedditAccount = false;
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
    $scope.state = '';
    $scope.hasSelectedRow = false;
    $scope.hasError = true;
    $scope.errorMessage = "";
    $scope.isDeleting = false;
    $scope.goHome = goHome;

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
        enableRowSelection: true,
        enableRowHeaderSelection: true,
        multiSelect: false,
        columnDefs: [
            { field: "keyword", displayName: "Keyword" },
            { field: "response", displayName: "Response" },
            { field: "status", displayName: "Status" }
        ],
        onRegisterApi: function(gridApi) {
            $scope.gridApi = gridApi;
            gridApi.selection.on.rowSelectionChanged($scope, function (rows) {
                $scope.errorMessage = "";
                var selected = gridApi.selection.getSelectedRows();
                $scope.hasSelectedRow = selected && selected.length > 0;
            });
        }
    };

    $scope.$on('$locationChangeStart', function (next, last) {
        $scope.errorMessage = "";
        var path = $location.path();
        
        $scope.showAddEditDelete =
            path === '' ||
            path === '/#' ||
            path === '/';

        if (path === '/Add' && $scope.state !== 'ADD') {
            $scope.state = 'ADD';
            $location.path('/');
        }

        if (path === '/Edit' && $scope.state !== 'EDIT') {
            $scope.state = 'EDIT';
            $location.path('/');
        }
    });

    $scope.onAddClicked = function () {
        $scope.errorMessage = "";
        $scope.state = 'ADD';
        $location.path('/Add');
    }

    $scope.onEditClicked = function () {
        var selected = $scope.gridApi.selection.getSelectedRows()[0];
        $scope.selectedSet = selected;
        $scope.errorMessage = "";
        $scope.state = 'EDIT';
        $location.path('/Edit');
    }

    $scope.deleteKeywordResponseSet = function () {
        $scope.errorMessage = "";
        $scope.isDeleting = true;
        var selected = $scope.gridApi.selection.getSelectedRows()[0];

        krService.DeleteKrSet(selected.id)
           .success(function (response) {
               refreshGrid();
               $scope.isDeleting = false;
               $scope.hasSelectedRow = false;
            })
           .error(function (response) {
               $scope.errorMessage = "Failed to Delete Keyword-Response Set.";
               $scope.isDeleting = false;
            });
    }

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

    function goHome() {
        $location.path('/');
    }
}