// mark.lawrence 
// main.controller.js


(function () {
    'use strict';

    
    angular.module('app.taxTemplate').controller('template', ['$scope', 'datacontext', ctrl]);

    function ctrl($scope, datacontext) {
        var vm = this; 
        vm.title = 'Tax Template';
        vm.description = 'Update Donor Tax page template';
       
        vm.changed = false;
        vm.undo = undo;
        vm.save = save;
        vm.previous = {};

        activate();

        function activate() {
            datacontext.getTemplateByName('donortax').then(function(response) {
                vm.template = response.data;
                vm.previous = angular.copy(vm.template); 
            });
        }

        function undo() {
            vm.template = angular.copy(vm.previous); 
            $scope.frm.$setPristine();
        }

        function save() {
            datacontext.saveTemplate(vm.template).then(function(response) {
                vm.previous = angular.copy(vm.template);
                $scope.frm.$setPristine();
            });
        }

    }


})();