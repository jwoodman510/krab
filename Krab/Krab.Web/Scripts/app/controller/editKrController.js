angular
    .module("myApp.controllers")
    .controller("editKrController", editKrController);

function editKrController($rootScope, $scope, $http, krService, locationService, gridService) {
    $scope.keyword = "";
    $scope.response = "";
    $scope.statusId = "";
    $scope.hasError = false;
    $scope.errorMsg = "";
    $scope.isSaving = false;
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

    init();

    function init() {
        $scope.selectedSet = gridService.getSelectedRow();
        $scope.keyword = $scope.selectedSet.keyword;
        $scope.response = $scope.selectedSet.response;

        if ($scope.selectedSet.statusId === 2) {
            $scope.selectedStatus = $scope.statuses[1];
        }
    }

    $scope.UpdateKrSet = function () {
        $scope.isSaving = true;
        $scope.hasError = false;
        var validationResult = validateSet();

        if (validationResult && validationResult.length > 0) {
            $scope.errorMsg = validationResult;
            $scope.isSaving = false;
            $scope.hasError = true;
            return;
        }

        var set = {
            'Id': $scope.selectedSet.id,
            'Keyword': $scope.keyword,
            'Response': $scope.response,
            'StatusId': $scope.selectedStatus.id
        };

        krService.EditKrSet(set)
            .success(function () {
                $scope.isSaving = false;
                gridService.refreshGrid();
                locationService.goHome();
            })
            .error(function () {
                $scope.errorMsg = "Error udpating keyword-response set.";
                $scope.isSaving = false;
                $scope.hasError = true;
            });
    }

    function validateSet() {
        if (!$scope.keyword || $scope.keyword === null || $scope.keyword.length < 5 || $scope.keyword.length > 50) {
            return "Keyword must be 5 - 50 characters.";
        }
        else if (!$scope.response || $scope.response === null || $scope.response.length < 1 || $scope.response.length > 1000) {
            return "Response must be 1 - 1000 characters.";
        }

        return null;
    }
}