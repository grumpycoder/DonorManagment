// mark.lawrence 
// datacontext.js


(function() {
    'use strict';
    angular.module('app.data').factory('datacontext', ['$http', datacontext]);


    function datacontext($http) {
        
        var service = {
            uploadTaxData: uploadTaxData,
            getTemplateById: getTemplateById,
            getTemplateByName: getTemplateByName,
            saveTemplate: saveTemplate,
            getContituents: getConstituents

        }

        return service;

        function uploadTaxData(datafile) {
            var url = '/api/tax/posttaxdata';
            var formData = new FormData();
            formData.append('file', datafile);

            return $http.post(url, formData, {
                transformRequest: angular.identity,
                headers: { 'Content-Type': undefined }
            });
        }

        function getTemplateById(id) {
            return $http.get('/api/template/' + id);
        }

        function getTemplateByName(name) {
            return $http.get('/api/template/' + name);
        }

        function saveTemplate(template) {
            return $http.post('/api/template', template);
        }

        function getConstituents() {
            
        }
    }
})(); 