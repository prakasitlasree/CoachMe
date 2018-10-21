var app = angular.module('SearchTeacherApp', []);
app.controller('SearchTeacherController', function ($scope, $http, $compile) {



    $scope.provinceID;
    $scope.GetProvince = function () {
        debugger;
        $http({
            url: "http://localhost:1935/student/GetListProvince",
            method: "GET",
            //params: { geoID: $scope.geography }
        }).then(function (response) {
            console.log(response.data.OUTPUT_DATA)
            if (response.data.STATUS == true) {
                $scope.ListProvince = response.data.OUTPUT_DATA;
                $scope.RenderProvinceDrp()
            }
        }).then(function () {
            $('#SeachProvince').val('0').change()
        });
    }

    $scope.RenderProvinceDrp = function () {
        $('#SeachProvince').empty();
        debugger;
        var html = "";

        var itemsLength = Object.keys($scope.ListProvince).length;
        html += "<option value = 0 >-เลือกทั้งหมด-</option>"
        for (var i = 0; i < itemsLength; i++) {
            html += "<option value = " + $scope.ListProvince[i].PROVINCE_ID + ">" + $scope.ListProvince[i].PROVINCE_NAME + "</option>";
        }
        $('#SeachProvince').append(html)

    }


    $scope.GetAmphur = function () {
        $http({
            url: "http://localhost:1935/student/GetListAmphur",
            method: "GET",
            params: { provinceID: document.getElementById("SeachProvince").value }
        }).then(function (response) {
            console.log(response.data.OUTPUT_DATA)
            if (response.data.STATUS == true) {
                $scope.ListAmphur = response.data.OUTPUT_DATA;
                $scope.RenderAmphurDrp()
            }
        }).then(function () {
            $("#amphur").prop('disabled', false);
        });
    }

    $scope.RenderAmphurDrp = function () {
        $('#amphur').empty();
        debugger;
        var html = "";

        var itemsLength = Object.keys($scope.ListAmphur).length;
        html += "<option value = 0 selected = 'selected'>-เลือกทั้งหมด-</option>"
        for (var i = 0; i < itemsLength; i++) {
            html += "<option value = " + $scope.ListAmphur[i].AMPHUR_ID + ">" + $scope.ListAmphur[i].AMPHUR_NAME + "</option>";
        }
        $('#amphur').append(html)
        //var $OnCompile = $(html).appendTo('#amphur');
        //$compile($OnCompile)($scope);

    }

    angular.element(document).ready(function () {


    });
});
