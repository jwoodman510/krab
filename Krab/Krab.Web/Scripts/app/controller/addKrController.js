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
    $scope.errorMsg = "";

    $scope.addKrSet = function () {
        $scope.isAdding = true;
        $scope.hasAddError = false;
        var validationResult = validateSet();

        if (validationResult && validationResult.length > 0) {
            $scope.errorMsg = validationResult;
            $scope.isAdding = false;
            $scope.hasAddError = true;
            return;
        }
        krService.AddKrSet({
            'Keyword': $scope.keyword,
            'Response': $scope.response,
            'StatusId': $scope.selectedStatus.id
        }).success(function () {
            gridService.refreshGrid();
            locationService.goHome();
            $scope.isAdding = false;
        }).error(function () {
            $scope.errorMsg = "Error adding keyword-response set.";
            $scope.hasAddError = true;
            $scope.isAdding = false;
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