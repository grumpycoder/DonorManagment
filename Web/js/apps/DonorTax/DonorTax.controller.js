// mark.lawrence 
// DonorTax.controller.js


(function() {
    'use strict';

    window.DonorTax.controller('DonorTaxCtrl', ['$scope', '$http', mainCtrl]);


    function mainCtrl($scope, $http) {
        
        $scope.vm = {};

        init();

        function init() {
                        getViewModel();
//            postViewModel();
        }

        function getViewModel() {
            $http.get('/api/DonorTaxApi').then(function(response) {

                $scope.vm = response.data;
                console.log($scope.vm);
            }, function(response) {
                console.log('error occurred');
            }).then(function(response) {
                console.log('success');
                postViewModel();
                $scope.vm.isDetailsVisible = true;
            });
        }


        function postViewModel() {
//            $scope.vm.searchEntity.lookupId = '8-13239064';
//            $scope.vm.searchEntity.zipcode = '80203';

            $http.post('/api/DonorTaxApi', $scope.vm).then(function (response) {

                $scope.vm = response.data;
                console.log($scope.vm);
            });
        }

    }

})(); 