// mark.lawrence 
// main.controller.js


(function () {
    'use strict';

    window.TaxTemplate.controller('TemplateCtrl', ['$scope', '$http', mainCtrl]);

    function mainCtrl($scope, $http) {
        var vmCopy = {};
        $scope.title = 'Tax Template';
        $scope.description = 'Update Donor Tax page template';
        $scope.changed = false;
        $scope.undo = undo;
        $scope.save = save;

        activate();

        function activate() {
            $http.get('/api/tax/template').success(function (response) {
                $scope.vm = response;
                vmCopy = angular.copy($scope.vm);
            }).error(function (response) {
                console.log(response);
            });
        }

        function undo() {
            $scope.frm.$setPristine();
            $scope.vm = angular.copy(vmCopy);
        }

        function save() {
            $scope.vm.eventCommand = 'save';
            $http.post('/api/tax/template', $scope.vm).success(function (response) {
                $scope.vm = response;
                vmCopy = angular.copy($scope.vm);
                $scope.frm.$setPristine();
            }).error(function (err) {
                console.log(err);
            });
        }
    }



})();