var app = angular.module('SearchTeacherApp', []);
app.controller('SearchTeacherController', function ($scope, $http, $compile) {


    $scope.SEARCH_TYPE = "ครู";
    $scope.provinceID;
    $scope.ListCategory = [];
    $scope.ListCourse = [];

    $scope.GetType = function () {
        

        $('#SEARCH_TYPE').val($scope.SEARCH_TYPE)

        if (document.getElementById("SEARCH_TYPE").value == "ครู") {
            $http({
                url: "/home/GetListCategory",
                method: "GET",
            }).then(function (response) {
                console.log(response.data.OUTPUT_DATA)
                if (response.data.STATUS == true) {
                    $scope.ListCategory = response.data.OUTPUT_DATA;
                    $scope.RenderCategoryDrp()
                }
            }).then(function () {
                $('#SEARCH_TEACHER_MODEL.LIST_MEMBER_CETEGORY').prop('selectedIndex', 0);
               
            });
        }
        else {
            
            $http({
                url: "/home/GetListCourse",
                method: "GET",
                //params: { geoID: $scope.geography }
            }).then(function (response) {
                console.log(response.data.OUTPUT_DATA)
                if (response.data.STATUS == true) {
                    $scope.ListCourse = response.data.OUTPUT_DATA;
                    $scope.RenderCourseDrp()
                }
            }).then(function () {
                $('#SEARCH_TEACHER_MODEL.LIST_MEMBER_TEACH_COURSE').prop('selectedIndex', 0);
            });
        }
    }

    $scope.RenderCategoryDrp = function () {
        $('#TYPE').empty();
        var html = "";
        
        var itemsLength = Object.keys($scope.ListCategory).length;
        html += "<select onchange='changeListMemberCategory(this)' class='form-control m-input m-input--pill' id='SEARCH_TEACHER_MODEL.LIST_MEMBER_CETEGORY' name='LIST_CATE'>"
        html += "<option value = 0 >-เลือกทั้งหมด-</option>"
        for (var i = 0; i < itemsLength; i++) {
            html += "<option value = " + $scope.ListCategory[i].AUTO_ID + ">" + $scope.ListCategory[i].NAME + "</option>";
        }
        html += "</select>"
        $('#TYPE').append(html)
    }

    $scope.RenderCourseDrp = function () {
        $('#TYPE').empty();
        var html = "";
        
        var itemsLength = Object.keys($scope.ListCourse).length;
        html += "<select onchange='changeListMemberCategory(this)' class='form-control m-input m-input--pill' id='SEARCH_TEACHER_MODEL.LIST_MEMBER_TEACH_COURSE' name='LIST_COURSE'>"
        html += "<option value = 0 >-เลือกทั้งหมด-</option>"
        for (var i = 0; i < itemsLength; i++) {
            html += "<option value = " + $scope.ListCourse[i].AUTO_ID + ">" + $scope.ListCourse[i].NAME + "</option>";
        }
        html += "</select>"
        $('#TYPE').append(html)
    }

    $scope.GetProvince = function () {
        
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
        }).then(function () {
            $('#SeachProvince').val('0').change()
        });
    }

    $scope.RenderProvinceDrp = function () {
        $('#SeachProvince').empty();
        
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
            url: "/student/GetListAmphur",
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
