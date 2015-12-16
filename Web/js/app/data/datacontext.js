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
            getConstituent: getConstituent,
            getConstituents: getConstituents,
            saveConstituent: saveConstituent,
            getTaxItems: getTaxItems,
            deleteTaxItems: deleteTaxItems, 
            saveTaxItems: saveTaxItems
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

        function getConstituent(id) {
            console.log(id);
            return $http.get('/api/constituent/' + id + '/taxes');
        }

        function getConstituents(vm) {
            return $http.post('/api/constituents', vm.result);
        }

        function saveConstituent(constituent) {
            console.log(constituent);
            return $http.post('/api/constituent', constituent);
        }

        function getTaxItems(constituentId) {
            return $http.get('/api/constituent/' + constituentId + '/taxes'); 
        }

        function deleteTaxItems(items) {
            return $http.post('/api/deletetaxitems', items);
        }

        function saveTaxItems(constituentId, items) {
            return $http.post('/api/constituent/' + constituentId + '/taxes', items);
        }

    }
})(); 