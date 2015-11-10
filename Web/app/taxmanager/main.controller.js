// mark.lawrence 
// main.controller.js


(function () {
    'use strict';

    var controllerId = "MainCtrl";

    function mainCtrl($scope, $http) {

        $scope.title = 'Tax Manager';
        $scope.changed = false;
        $scope.undo = undo;
        $scope.save = save;

        activate();

        function activate() {
            console.log('activate');

            $http.get('/api/taxmanager/template').success(function (response) {
                console.log(response);
                $scope.vm = response;
            }).error(function (response) {
                console.log(response);
            });
        }

        function undo() {
            console.log('undo');
            $scope.frm.$setPristine();
        }

        function save() {
            console.log('save');
            $scope.vm.eventCommand = 'save';
            $http.post('/api/taxmanager/template', $scope.vm).success(function (response) {
                console.log(response);
                $scope.vm = response;
                $scope.frm.$setPristine();
            }).error(function (err) {
                console.log(err);
            });
        }

    }

    window.app.controller(controllerId, ['$scope', '$http', mainCtrl]);


})();