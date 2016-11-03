angular
    .module("myApp.controllers")
    .controller("editKrController", editKrController);

function editKrController($rootScope, $scope, $http, krService, $location) {
    $scope.keyword = "";
    $scope.response = "";
    $scope.statusId = "";
    $scope.hasError = false;
    $scope.isSaving = false;
    $scope.krDataChanged = 'KR_DATA_CHANGED';
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
        var krController = $scope.$parent.$parent;
        $scope.selectedSet = krController.selectedSet;
        $scope.keyword = $scope.selectedSet.keyword;
        $scope.response = $scope.selectedSet.response;

        if ($scope.selectedSet.statusId == 2) {
            $scope.selectedStatus = $scope.statuses[1];
        }
    }

    $scope.UpdateKrSet = function () {
        $scope.isSaving = true;
        $scope.hasError = false;
        var set = {
            'Id': $scope.selectedSet.id,
            'Keyword': $scope.keyword,
            'Response': $scope.response,
            'StatusId': $scope.selectedStatus.id
        };

        krService.EditKrSet(set)
            .success(function (response) {
                $scope.isSaving = false;
                $rootScope.$broadcast($scope.krDataChanged);
                $location.path('/');
            })
            .error(function (response) {
                $scope.isSaving = false;
                $scope.hasError = true;
            });
    }
}