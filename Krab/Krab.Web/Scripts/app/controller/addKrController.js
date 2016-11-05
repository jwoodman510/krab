angular
    .module("myApp.controllers")
    .controller("addKrController", addKrController);

function addKrController($scope, krService, locationService, gridService) {
    $scope.keyword = "";
    $scope.response = "";
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
    $scope.hasAddError = false;
    $scope.isAdding = false;

    $scope.addKrSet = function () {
        $scope.isAdding = true;
        $scope.hasAddError = false;
        krService.AddKrSet({
            'Keyword': $scope.keyword,
            'Response': $scope.response,
            'StatusId': $scope.selectedStatus.id
        }).success(function () {
            gridService.refreshGrid();
            locationService.goHome();
            $scope.isAdding = false;
        }).error(function () {
            $scope.hasAddError = true;
            $scope.isAdding = false;
        });
    }
}