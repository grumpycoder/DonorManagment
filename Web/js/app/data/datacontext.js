// mark.lawrence 
// datacontext.js


(function() {
    'use strict';
    angular.module('app.data').factory('datacontext', ['$http', datacontext]);


    function datacontext($http) {
        
        var service = {
            uploadTaxData: uploadTaxData,
            getContituents: getConstituents,

        }

        return service;

        function uploadTaxData(datafile) {
            console.log('upload file');
            var url = '/api/tax/posttaxdata';
            var formData = new FormData();
            formData.append('file', datafile);

            return $http.post(url, formData, {
                transformRequest: angular.identity,
                headers: { 'Content-Type': undefined }
            });
        }

        function getConstituents() {
            
        }
    }
})(); 