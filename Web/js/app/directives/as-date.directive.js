// mark.lawrence 
// as-date.directive.js

angular.module('CommonDirectives', [])
    .directive('asDate', [function () {
    return {
        require: '^ngModel',
        restrict: 'A',
        link: function (scope, element, attrs, ctrl) {
            ctrl.$formatters.splice(0, ctrl.$formatters.length);
            ctrl.$parsers.splice(0, ctrl.$parsers.length);
            ctrl.$formatters.push(function (modelValue) {
                if (!modelValue) {
                    return;
                }
                console.log(new Date(modelValue));
                return new Date(modelValue);
            });
            ctrl.$parsers.push(function (modelValue) {
                console.log(modelValue);
                return modelValue;
            });
        }
    };
}]); 