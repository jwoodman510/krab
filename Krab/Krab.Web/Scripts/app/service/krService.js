angular
    .module('myApp.services', [])
    .factory("krService", krService);

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