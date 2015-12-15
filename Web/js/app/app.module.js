// mark.lawrence 
// app.module.js


(function() {
    'use strict';
    angular.module('app', [
        //Angular modules
        'app.core',
        'app.data',
    
        //Feature areas
        'app.taxTemplate', 
        'app.taxUpload',
        'app.taxManager'
    ]);

})();

