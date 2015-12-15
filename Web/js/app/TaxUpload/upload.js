// mark.lawrence 
// TaxDataUpload.controller.js


(function () {
    'use strict';

    angular.module('app.taxUpload').controller('upload', ['$http', 'datacontext', ctrl]);

    function ctrl($http, datacontext) {
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
            //var url = '/api/tax/posttaxdata';
            //var formData = new FormData();
            //formData.append('file', vm.file);

            //$http.post(url, formData, {
            //    transformRequest: angular.identity,
            //    headers: { 'Content-Type': undefined }
            //}).success(function (response) {
            //    vm.status = response;
            //}).error(function (response) {
            //    vm.status = response;
            //}).finally(function () {
            //    vm.isComplete = true;
            //    vm.isProcessing = false;
            //    vm.file = null; 
            //});

        }
    };

})();