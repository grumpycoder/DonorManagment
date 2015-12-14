// mark.lawrence 
// DataManager.Controller.js


(function () {
    'use strict';

    window.TaxDataManager.controller('MainCtrl', ['$http', '$uibModal', mainCtrl]);

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
                controller: ['$uibModalInstance', '$http', 'item', EditModalCtrl],
                controllerAs: 'vm',
                resolve: {
                    item: function () { return item; }
                }
            });
        };

        function showTaxItems(id) {
            $modal.open({
                templateUrl: '/js/apps/taxdatamanager/templates/taxitems.tmpl.html',
                controller: ['$uibModalInstance', '$http', 'items', TaxItemsModalCtrl],
                controllerAs: 'vm',
                resolve: {
                    items: function ($http) {
                        return $http.get('/api/constituent/' + id + '/taxes').then(function (r) {
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
        vm.years = [];

        var constituentId = items[0].constituentId;
        
        var currentYear = parseInt(moment().get('Year') - 1);
        for (var i = 0; i < 5; i++) {
            vm.years.push(currentYear - i);
        }
        vm.selectedYear = {
            taxYear: currentYear
        };

        vm.deletedItems = []; 
        vm.editItem = editItem;
        vm.saveChanges = saveChanges;
        vm.saveItem = saveItem;
        vm.cancelItem = cancelItem;
        vm.deleteItem = deleteItem;

        vm.currentEdit = {};
        vm.hasChanges = false;
      


        function deleteItem(item) {
            var idx = vm.taxItems.indexOf(item);
            vm.deletedItems.push(item);
            vm.taxItems.splice(idx, 1);
            vm.hasChanges = true; 
        }

        function editItem(item) {
            vm.currentEdit = {};
            vm.currentEdit[item.id] = true;
            vm.itemToEdit = angular.copy(item);
        }

        function cancelItem(item) {
            vm.currentEdit[item.id] = false;
        }

        function saveItem() {
            vm.currentEdit = {};
            vm.hasChanges = true;
        }

        function saveChanges() {
            $http.post('/api/constituent/' + constituentId + '/taxes', vm.taxItems).success(function (r) {
                //NOTIFICATION HERE

                vm.currentEdit[item.id] = false;
            }).success(function () {

            });
            $http.delete('/api/taxitems', vm.deletedItems);
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