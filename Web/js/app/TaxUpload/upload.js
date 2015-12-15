// mark.lawrence 
// TaxDataUpload.controller.js


(function () {
    'use strict';

    angular.module('app.taxUpload').controller('upload', ['datacontext', ctrl]);

    function ctrl(datacontext) {
        var vm = this;

        vm.title = 'Tax';
        vm.description = "Upload Data";

        vm.notesCollapsed = true;
        vm.isComplete = false;
        vm.isProcessing = false;
        vm.processFile = processFile;
        vm.status = null;

        function processFile() {
            vm.isProcessing = true;
            vm.isComplete = false;
            datacontext.uploadTaxData(vm.file)
                       .success(function (response) {
                           vm.status = response;
                       }).error(function (response) {
                           vm.status = response;
                       }).finally(function () {
                           vm.isComplete = true;
                           vm.isProcessing = false;
                           vm.file = null;
                       });;

        }

    };

})();