// mark.lawrence 
// DataManager.Controller.js


(function () {
    'use strict';

    window.TaxDataManager.controller('MainCtrl', ['$http', '$modal', mainCtrl]);

    function mainCtrl($http, $modal) {
        var vm = this;

        vm.title = 'Data Manager';
        vm.description = "Update Tax and Constituent Data";

        vm.result = {
            page: 1,
            pageSize: 25
        };

        vm.editModal = editModal;
        vm.showTaxItems = showTaxItems;

        init();

        function init() {
            getConstituents();
        }

        function getConstituents() {
            $http.post('/api/constituents', vm.result)
                 .success(function (response) {
                     vm.result = response;
                 });
        };

        function editModal(item) {
            $modal.open({
                templateUrl: '/js/apps/taxdatamanager/templates/modal.html',
                controller: ['$modalInstance', '$http', 'item', EditModalCtrl],
                controllerAs: 'vm',
                resolve: {
                    item: function () { return item; }
                }
            });
        };

        function showTaxItems(id) {
            $modal.open({
                templateUrl: '/js/apps/taxdatamanager/templates/taxitems.tmpl.html',
                controller: ['$modalInstance', '$http', 'items', TaxItemsModalCtrl],
                controllerAs: 'vm',
                resolve: {
                    items: function($http) {
                        return  $http.get('/api/constituent/' + id + '/taxes').then(function(r) {
                            return r.data; 
                        });
                    }
                }
            });
        };

    };

    function TaxItemsModalCtrl($modalInstance, $http, items) {
        var vm = this;
        vm.taxItems = items;
        var constituentId = items[0].constituentId;
        console.log(constituentId);

        vm.save = save;

        function save() {
            console.log('save');
            $http.post('/api/constituent/' + constituentId +'/taxes', vm.taxItems).success(function (r) {
                //NOTIFICATION HERE
            });
            $modalInstance.close();
        }
    }

    function EditModalCtrl($modalInstance, $http, item) {
        var vm = this;
        vm.item = item;
        vm.save = save;

        function save() {
            $http.post('/api/constituent', vm.item).success(function (r) {
                //NOTIFICATION HERE
            });
            $modalInstance.close();
        }
    }

})();