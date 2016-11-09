angular
    .module("myApp.controllers")
    .controller("krController", krController);

function krController($rootScope, $scope, $http, krService, locationService, $location, gridService, modalService) {
    $scope.krSets = [];
    $scope.redditUserName = "";
    $scope.isRedditUserNameLoading = true;
    $scope.isKrSetsLoading = true;
    $scope.needsRedditAccount = false;
    $scope.isDeletedSetVisible = false;
    $scope.errorMessage = "";
    $scope.state = "";
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

    $scope.gridOptions = {
        data: "krSets",
        enableSorting: true,
        enableRowSelection: true,
        enableRowHeaderSelection: true,
        multiSelect: false,
        enableColumnResizing: true,
        enableFiltering: true,
        columnDefs: [
            {
                field: "keyword",
                displayName: "Keyword"
            },
            {
                field: "response",
                displayName: "Response"
            },
            {
                field: "status",
                displayName: "Status"
            },
            {
                field: "subreddits",
                displayName: "Subreddits",
                cellTemplate: "<div style=\"text-align: center;\"><a class=\"clickable\" ng-click=\"grid.appScope.onManageSubredditsClicked(row)\">Manage Subreddits</a></div>"
            }
        ],
        onRegisterApi: function(gridApi) {
            $scope.gridApi = gridApi;
            gridApi.selection.on.rowSelectionChanged($scope, function () {
                $scope.errorMessage = "";
                var selected = gridApi.selection.getSelectedRows();
                $scope.hasSelectedRow = selected && selected.length > 0;
            });
        }
    };

    $scope.$on("$locationChangeStart", function () {
        $scope.errorMessage = "";
        var path = $location.path();
        
        $scope.showAddEditDelete =
            path === "" ||
            path === "/#" ||
            path === "/";

        if (path === "/Add" && $scope.state !== "ADD") {
            $scope.state = "ADD";
            $location.path("/");
        }

        if (path === "/Edit" && $scope.state !== "EDIT") {
            $scope.state = "EDIT";
            $location.path("/");
        }
    });

    $scope.onAddClicked = function () {
        $scope.errorMessage = "";
        $scope.state = "ADD";
        $location.path("/Add");
    }

    $scope.onEditClicked = function () {
        var selected = $scope.gridApi.selection.getSelectedRows()[0];
        gridService.setSelectedRow(selected);
        $scope.errorMessage = "";
        $scope.state = "EDIT";
        $location.path("/Edit");
    }

    $scope.deleteKeywordResponseSet = function () {
        $scope.errorMessage = "";
        $scope.isDeleting = true;
        var selected = $scope.gridApi.selection.getSelectedRows()[0];

        krService.DeleteKrSet(selected.id)
           .success(function () {
               gridService.refreshGrid();
               $scope.isDeleting = false;
               $scope.hasSelectedRow = false;
            })
           .error(function () {
               $scope.errorMessage = "Failed to Delete Keyword-Response Set.";
               $scope.isDeleting = false;
            });
    }

    $scope.$on(gridService.krDataChanged, function () {
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
                    $scope.redditUserName = "Welcome, Reddit User: " + response.result.userName;
                } else {
                    $scope.needsRedditAccount = true;
                }
                $scope.isRedditUserNameLoading = false;
            })
            .error(function () {
                $scope.isRedditUserNameLoading = false;
            });
    }

    function goHome() {
        locationService.goHome();
    }

    $scope.onManageSubredditsClicked = function(row) {
        modalService.setData(row.entity);
        modalService.open({
            controller: "manageSubredditsController",
            templateUrl: "Function_Views/manageSubredditsModal.html",
            controllerAs: "msCtrl"
        });
    }

    $scope.onGenerateReportClicked = function() {
        modalService.open({
            controller: "generateReportController",
            templateUrl: "Function_Views/generateReportModal.html",
            controllerAs: "gCtrl"
        });
    }
}