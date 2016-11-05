angular
    .module("myApp.services")
    .factory("krService", krService);

function krService($http) {
    var svc = this;

    svc.getByUserId = function () {
        return $http.get("/api/keywordresponsesets");
    };

    svc.AddKrSet = function (set) {
        return $http.post("/api/keywordresponsesets", set);
    };

    svc.EditKrSet = function (set) {
        return $http.put("/api/keywordresponsesets", set);
    };

    svc.DeleteKrSet = function (keywordResponseSetId) {
        return $http.delete("/api/keywordresponsesets/deleteKeywordResponseSet?keywordResponseSetId=" + keywordResponseSetId.toString());
    };

    svc.getSubreddits = function (keywordResponseSetId) {
        return $http.get("/api/subreddit/GetByKeywordResponseSet?keywordResponseSetId=" + keywordResponseSetId);
    }

    svc.updateSubreddits = function(keywordResponseSetId, subredditNames) {
        return $http.put("/api/keywordresponsesets/updateSubreddits?keywordResponseSetId=" + keywordResponseSetId, subredditNames);
    }

    return svc;
}