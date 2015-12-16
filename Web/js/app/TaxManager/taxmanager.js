// mark.lawrence 
// taxManager.js


(function () {
    'use strict';

    angular.module('app.taxManager').controller('taxManager', ['$http', '$uibModal', 'datacontext', mainCtrl]);

    function mainCtrl($http, $modal, datacontext) {
        var vm = this;

        vm.title = 'Data Manager';
        vm.description = "Update Tax and Constituent Data";

        vm.result = {
            page: 1,
            pageSize: 5
        };

        vm.editModal = editModal;
        vm.showTaxItems = showTaxItems;
        vm.search = search;
        vm.showPager = showPager;

        init();

        function init() {
            getConstituents();
        }

        function getConstituents() {
            datacontext.getConstituents(vm).then(function (data) {
                vm.result = data.data;
            });
        };

        function search() {
            getConstituents();
        }

        function showPager() {
            return vm.result.totalPages > 1;
        }

        function editModal(item) {
            $modal.open({
                templateUrl: '/js/app/taxmanager/editConstituent.html',
                controller: ['$uibModalInstance', '$http', 'datacontext', 'item', EditModalCtrl],
                controllerAs: 'vm',
                resolve: {
                    item: function () { return item; }
                }
            });
        };

        function showTaxItems(id) {
            $modal.open({
                templateUrl: '/js/app/taxmanager/taxitems.html',
                controller: ['$uibModalInstance', 'datacontext', 'items', TaxItemsModalCtrl],
                controllerAs: 'vm',
                resolve: {
                    items: function (datacontext) {
                        return datacontext.getTaxItems(id).then(function (resp) {
                            return resp.data;
                        });
                    }
                }
            });
        };

    };

    function TaxItemsModalCtrl($modalInstance, datacontext, items) {
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

            datacontext.saveTaxItems(constituentId, vm.taxItems).then(function (r) {
                //NOTIFICATION HERE
            });
            
            datacontext.deleteTaxItems(vm.deletedItems);

            //$http.post('/api/constituent/' + constituentId + '/taxes', vm.taxItems).success(function (r) {
            //    //NOTIFICATION HERE

            //    vm.currentEdit[item.id] = false;
            //}).success(function () {

            //});
            //$http.delete('/api/taxitems', vm.deletedItems);

            $modalInstance.close();
        }
    }

    function EditModalCtrl($modalInstance, $http, datacontext, item) {
        var vm = this;
        vm.item = item;
        vm.save = save;

        function save() {
            datacontext.saveConstituent(vm.item).then(function (resp) {
                console.log(resp);
            }).finally(function () {
                $modalInstance.close();
            });

        }
    }

})();