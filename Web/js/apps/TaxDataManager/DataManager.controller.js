// mark.lawrence 
// DataManager.Controller.js


(function() {
    'use strict';

    window.TaxDataManager.controller('MainCtrl', ['$scope', '$http', mainCtrl]);

    function mainCtrl($scope, $http) {
        $scope.title = 'Data Manager'; 
    }; 

})();