// mark.lawrence 
// DataManager.Controller.js


(function () {
    'use strict';

    window.TaxDataManager.controller('MainCtrl', ['$scope', '$http', mainCtrl]);

    function mainCtrl($scope, $http) {
        var vm = this;

        $scope.title = 'Data Manager';

        vm.notesCollapsed = true;
        vm.isComplete = false;
        vm.isProcessing = false;
        vm.processFile = processFile;
        vm.status = null;

        function processFile() {
            vm.isProcessing = true;
            vm.isComplete = false; 
            var url = '/api/tax/posttaxdata';
            var formData = new FormData();
            formData.append('file', vm.file);

            $http.post(url, formData, {
                transformRequest: angular.identity,
                headers: { 'Content-Type': undefined }
            }).success(function (response) {
                vm.status = response;
            }).error(function (response) {
                vm.status = response;
            }).finally(function () {
                vm.isComplete = true;
                vm.isProcessing = false;
                vm.file = null; 
            });

        }
    };

})();