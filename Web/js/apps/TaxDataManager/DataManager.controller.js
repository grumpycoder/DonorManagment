// mark.lawrence 
// DataManager.Controller.js


(function () {
    'use strict';

    window.TaxDataManager.controller('MainCtrl', ['$http', '$modal', mainCtrl]);

    function mainCtrl($http, $modal) {
        var vm = this;

        vm.title = 'Data Manager';
        vm.description = "Update Tax and Constituent Data";

        vm.pageSizes = ['10', '15', '25', '50'];

        vm.result = {
            page: 1,
            pageSize: 25
        };

        vm.editModal = editModal;

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

    };

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