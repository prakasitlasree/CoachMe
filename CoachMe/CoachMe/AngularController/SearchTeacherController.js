var app = angular.module('SearchTeacherApp', []);
app.controller('SearchTeacherController', function ($scope, $http, $compile) {

     

    $scope.provinceID;
    $scope.GetProvince = function () {
        debugger;
        $http({
            url: "/student/GetListProvince",
            method: "GET",
            //params: { geoID: $scope.geography }
        }).then(function (response) {
            console.log(response.data.OUTPUT_DATA)
            if (response.data.STATUS == true) {
                $scope.ListProvince = response.data.OUTPUT_DATA;
                $scope.RenderProvinceDrp()
            }
        });
    }

    $scope.RenderProvinceDrp = function () {
        $('#SeachProvince').empty();
        debugger;
        var html = "";

        var itemsLength = Object.keys($scope.ListProvince).length;
        html += "<option value = 0>-เลือกทั้งหมด-</option>"
        for (var i = 0; i < itemsLength; i++) {
            html += "<option value = " + $scope.ListProvince[i].PROVINCE_ID + ">" + $scope.ListProvince[i].PROVINCE_NAME + "</option>";
        }
        //var $OnCompile = $(html).appendTo('#SeachProvince');
        //$compile($OnCompile)($scope);
        $('#SeachProvince').append(html);
        $("#SeachProvince").prop('selectedIndex', 0);
        
    }

    $scope.GetAmphur = function () {
        $http({
            url: "/student/GetListAmphur",
            method: "GET",
            params: { provinceID: document.getElementById("SeachProvince").value }
        }).then(function (response) {
            console.log(response.data.OUTPUT_DATA)
            if (response.data.STATUS == true) {
                $scope.ListAmphur = response.data.OUTPUT_DATA;
                $scope.RenderAmphurDrp()
               
                $("#amphur").prop("disabled", false);
            }
        });
    }

    $scope.RenderAmphurDrp = function () {
        $('#amphur').empty();
        debugger;
        var html = "";

        var itemsLength = Object.keys($scope.ListAmphur).length;
        html += "<option value = 0>-เลือกทั้งหมด-</option>"
        for (var i = 0; i < itemsLength; i++) {
            html += "<option value = " + $scope.ListAmphur[i].AMPHUR_ID + ">" + $scope.ListAmphur[i].AMPHUR_NAME + "</option>";
        }
        var $OnCompile = $(html).appendTo('#amphur');
        $compile($OnCompile)($scope);
        $("#amphur").prop('selectedIndex', 0);
    }

    angular.element(document).ready(function () {
        $("#amphur").prop('selectedIndex', 0);
    });
});
