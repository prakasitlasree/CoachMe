var app = angular.module('MainHome', []);
app.controller('MainHomeController', function ($scope, $http, $compile) {

    $.LoadingOverlaySetup({
        background: "rgba(255,255, 255, 0.95)",
    });
    
    $scope.PAGE_NUMBER = null;
    $scope.PAGE_COUNT = null;

    $scope.ID_NAME_TEACHER
    $scope.ID_ABOUT_TEACHER

    $scope.LIST_SEARCH_TYPE = {
        availableOptions: [
          { id: '1', name: 'ครู', value: '1' },
          { id: '2', name: 'คอร์ส', value: '2' },
        ],
        selectedOption: { id: '1', name: 'ครู', value: '1' }
    };

    $scope.LIST_PROVINCE = {
        availableOptions: [],
        selectedOption: { PROVINCE_ID: 0, PROVINCE_CODE: 0, PROVINCE_NAME: "--เลือกทังหมด--", GEO_ID: 0 }
    }

    $scope.LIST_AMPHUR = {
        availableOptions: [],
        selectedOption: { AMPHUR_ID: 0, AMPHUR_CODE: "0", AMPHUR_NAME: "--เลือกทังหมด--" }
    }

    $scope.LIST_TEACHER_CATEGORY = {
        availableOptions: [],
        selectedOption: { AUTO_ID: 0, CODE: "0", NAME: "--เลือกทั้งหมด--" }
    }

    $scope.LIST_COURSE_CATEGORY = {
        availableOptions: [],
        selectedOption: { AUTO_ID: 0, NAME: "--เลือกทั้งหมด--" }
    }

    $scope.LIST_TEACHER = {}

    $scope.LIST_COURSE = {}

    $scope.GetComponent = function () {

        $http({
            url: "/home/GetListCategory    ",
            method: "GET",
        }).then(function (response) {
            console.log(response.data.OUTPUT_DATA)
            if (response.data.STATUS == true) {
                $scope.LIST_TEACHER_CATEGORY.availableOptions = response.data.OUTPUT_DATA
                $scope.LIST_TEACHER_CATEGORY.availableOptions.unshift({ AUTO_ID: 0, CODE: "0", NAME: "--เลือกทั้งหมด--" });


            }
        });

        $http({
            url: "/home/GetListCourseCategory    ",
            method: "GET",
        }).then(function (response) {
            console.log(response.data.OUTPUT_DATA)
            if (response.data.STATUS == true) {
                $scope.LIST_COURSE_CATEGORY.availableOptions = response.data.OUTPUT_DATA
                $scope.LIST_COURSE_CATEGORY.availableOptions.unshift({ AUTO_ID: 0, NAME: "--เลือกทั้งหมด--" });

            }
        });

        $http({
            url: "/student/GetListProvince",
            method: "GET",
        }).then(function (response) {
            console.log(response.data.OUTPUT_DATA)
            if (response.data.STATUS == true) {
                $scope.LIST_PROVINCE.availableOptions = response.data.OUTPUT_DATA;
                $scope.LIST_PROVINCE.availableOptions.unshift({ PROVINCE_ID: 0, PROVINCE_CODE: 0, PROVINCE_NAME: "--เลือกทั้งหมด--", GEO_ID: 0 });
            }
        });

        $scope.PAGE_NUMBER = 1;
        $scope.GetListTeacher();


    };

    $scope.ChangeSearchType = function () {
        $scope.Search()
        if ($scope.LIST_SEARCH_TYPE != null) {
            if ($scope.LIST_SEARCH_TYPE.selectedOption.value == 1) {
                $("#ID_TEACHER_CATE").show();
                $("#ID_COURSE_CATE").hide();
            }
            else if ($scope.LIST_SEARCH_TYPE.selectedOption.value == 2) {
                $("#ID_COURSE_CATE").show();
                $("#ID_TEACHER_CATE").hide();
            }
        }

    }

    $scope.GetAmphur = function () {

        $scope.PAGE_NUMBER = 1;
        $scope.Search();

        if ($scope.LIST_PROVINCE.selectedOption.PROVINCE_ID != 0) {
            $("#ID_AMPHUR").prop("disabled", false);
            $http({
                url: "/student/GetListAmphur",
                method: "GET",
                params: { provinceID: $scope.LIST_PROVINCE.selectedOption.PROVINCE_ID }
            }).then(function (response) {
                console.log(response.data.OUTPUT_DATA)
                if (response.data.STATUS == true) {
                    $scope.LIST_AMPHUR.availableOptions = response.data.OUTPUT_DATA;
                    $scope.LIST_AMPHUR.availableOptions.unshift({ AMPHUR_ID: 0, AMPHUR_CODE: "0", AMPHUR_NAME: "--เลือกทังหมด--" })
                }
            })
        }
        else {
            $scope.LIST_AMPHUR.selectedOption.AMPHUR_ID = 0;
            $("#ID_AMPHUR").prop("disabled", true);
        }

    }

    $scope.Search = function () {

        if ($scope.LIST_SEARCH_TYPE.selectedOption.value == 1) {
            $scope.PAGE_NUMBER = 1;
            $scope.GetListTeacher();
        }
        if ($scope.LIST_SEARCH_TYPE.selectedOption.value == 2) {
            $scope.PAGE_NUMBER = 1;
            $scope.GetListCourse();
        }
    }

    $scope.Next = function () {

        if ($scope.LIST_SEARCH_TYPE.selectedOption.value == 1) {

            $scope.PAGE_NUMBER = $scope.PAGE_NUMBER + 1;
            $scope.GetListTeacher();
        }
        if ($scope.LIST_SEARCH_TYPE.selectedOption.value == 2) {
            $scope.PAGE_NUMBER = $scope.PAGE_NUMBER + 1;
            $scope.GetListCourse();
        }
    }

    $scope.Previous = function () {

        if ($scope.LIST_SEARCH_TYPE.selectedOption.value == 1) {
            $scope.PAGE_NUMBER = $scope.PAGE_NUMBER - 1;
            $scope.GetListTeacher();
        }
        if ($scope.LIST_SEARCH_TYPE.selectedOption.value == 2) {
            $scope.PAGE_NUMBER = $scope.PAGE_NUMBER - 1;
            $scope.GetListCourse();
        }
    }

    $scope.GetListTeacher = function () {
        $.LoadingOverlay("show");
        $http({
            url: "/home/GetListTeacher",
            method: "GET",
            params: {
                SEARCH_TYPE: 1,
                CATEGORY_ID: $scope.LIST_TEACHER_CATEGORY.selectedOption.AUTO_ID,
                PROVINCE_ID: $scope.LIST_PROVINCE.selectedOption.PROVINCE_ID,
                AMPHUR_ID: $scope.LIST_AMPHUR.selectedOption.AMPHUR_ID,
                PAGE_NUMBER: $scope.PAGE_NUMBER,
                ID_ABOUT_TEACHER: $scope.ID_ABOUT_TEACHER,
                ID_NAME_TEACHER: $scope.ID_NAME_TEACHER,
            }
        }).then(function (response) {
            console.log(response.data.OUTPUT_DATA)
            if (response.data.STATUS == true) {

                $scope.PAGE_NUMBER = response.data.OUTPUT_DATA.PAGE_NUMBER;
                $scope.PAGE_COUNT = response.data.OUTPUT_DATA.PAGE_COUNT;
                $scope.LIST_TEACHER = response.data.OUTPUT_DATA;

                if ($scope.PAGE_NUMBER >= $scope.PAGE_COUNT) {
                    $('#BTN_NEXT').prop('disabled', true);
                }
                else {
                    $('#BTN_NEXT').prop('disabled', false);

                }
                if ($scope.PAGE_NUMBER > 1) {
                    $('#BTN_PREVIOUS').prop('disabled', false);
                }
                else {
                    $('#BTN_PREVIOUS').prop('disabled', true);
                }
            }
        }).then(function () {

            $scope.renderTeacherContent()
        }).then(function () {
            $.LoadingOverlay("hide");
        });

    }

    $scope.GetListCourse = function () {

        $.LoadingOverlay("show");
        $http({
            url: "/home/GetListCourse",
            method: "GET",
            params: {
                SEARCH_TYPE: 2,
                CATEGORY_ID: $scope.LIST_COURSE_CATEGORY.selectedOption.AUTO_ID,
                PROVINCE_ID: $scope.LIST_PROVINCE.selectedOption.PROVINCE_ID,
                AMPHUR_ID: $scope.LIST_AMPHUR.selectedOption.AMPHUR_ID,
                PAGE_NUMBER: $scope.PAGE_NUMBER
            }
        }).then(function (response) {
            console.log(response.data.OUTPUT_DATA)
            if (response.data.STATUS == true) {

                $scope.PAGE_NUMBER = response.data.OUTPUT_DATA.PAGE_NUMBER;
                $scope.PAGE_COUNT = response.data.OUTPUT_DATA.PAGE_COUNT;
                $scope.LIST_COURSE = response.data.OUTPUT_DATA;

                if ($scope.PAGE_NUMBER >= $scope.PAGE_COUNT) {
                    $('#BTN_NEXT').prop('disabled', true);
                }
                else {
                    $('#BTN_NEXT').prop('disabled', false);

                }
                if ($scope.PAGE_NUMBER > 1) {
                    $('#BTN_PREVIOUS').prop('disabled', false);
                }
                else {
                    $('#BTN_PREVIOUS').prop('disabled', true);
                }
            }
        }).then(function () {

            $scope.renderCourseContent()
        }).then(function () {
            $.LoadingOverlay("hide");
        });;
    }

    $scope.renderTeacherContent = function () {


        if ($scope.LIST_TEACHER.MEMBER_AUTO_ID > 0) {
            var keys = Object.keys($scope.LIST_TEACHER.LIST_MEMBERS);
            var len = keys.length;
            $('#listTeacher').empty()
            $('#listCourse').empty()
            for (var i = 0; i < len; i++) {

                var html = "";
                html += "<div class='m-portlet m-portlet--rounded'>"
                html += "<div class='m-portlet__body'>"
                html += "<div class='row'>"
                html += "   <div class='col-md-3'>"//md3
                html += "       <div class='m-card-profile'><div class='m-card-profile__pic'><div class='m-card-profile__pic-wrapper'>"
                if ($scope.LIST_TEACHER.LIST_MEMBERS[i].PROFILE_IMG_URL == null) {
                    debugger;
                    var pic = "..////Content//images//Blank-Profile.jpg";
                    html += "<img src= '" + pic + "' class='img-thumbnail' onclick = window.open('/content/images/blank-profile.jpg') style='height:130px;width:130px'  >"
                }
                else {
                    debugger;
                    var pic = "..////" + $scope.LIST_TEACHER.LIST_MEMBERS[i].PROFILE_IMG_URL;
                    if ($scope.LIST_TEACHER.LIST_MEMBERS[i].PROFILE_IMG_URL_FULL != null) {
                        var fullPic = $scope.LIST_TEACHER.LIST_MEMBERS[i].PROFILE_IMG_URL_FULL.replace("\\", "//")
                        html += "<img src= '" + pic + "' class='img-thumbnail' style='height:130px;width:130px' onclick=window.open('https://www.coachme.asia///" + fullPic + "')>"
                    }
                    else {
                        var fullPic = $scope.LIST_TEACHER.LIST_MEMBERS[i].PROFILE_IMG_URL.replace("\\", "//")
                        html += "<img src= '" + pic + "' class='img-thumbnail' style='height:130px;width:130px' onclick=window.open('https://www.coachme.asia///" + fullPic + "')>"
                    }
                }
                html += ""
                html += "       </div></div></div> "
                html += "   </div>"//ปิด md3
                html += "   <div class='col-md-8'>"//md8
                ////======================================================================== 
                if ($scope.LIST_TEACHER.LIST_MEMBERS[i].VERIFY == false) {
                    html += `<div class='m-list__content'>
                                <div class='m-list-badge m--margin-bottom-20'>
                                <div class ='m-list-badge__label ' style='font-size:12px;font-weight:400;'>
                                <i class ='fa fa-user-graduate'></i>&nbsp ชื่อ :
                                </div>
                                <div class='m-list-badge__items'>`
                    html += $scope.LIST_TEACHER.LIST_MEMBERS[i].FULLNAME
                    html += `    </div>
                                </div>
                                </div>`

                }
                else if ($scope.LIST_TEACHER.LIST_MEMBERS[i].VERIFY == true) {
                    html += `<div class='m-list__content'>
                                <div class='m-list-badge m--margin-bottom-20'>
                                <div class='m-list-badge__label ' style='font-size:12px;font-weight:400;'><i class='fa fa-user-graduate'></i>&nbsp ชื่อ :</div>
                                <div class='m-list-badge__items'>`
                    html += $scope.LIST_TEACHER.LIST_MEMBERS[i].FULLNAME
                    html += `
                                <i class ='fa fa-check-circle text-success'></i>
                                </div>
                                </div>
                                </div>`

                }

                ////======================================================================== 
                html += `<div class='m-list__content'>`
                html += `<div class='m-list-badge m--margin-bottom-20'>`
                html += `<div class='m-list-badge__label' style='font-size:12px;font-weight:400;'><i class='fa fa-book-open'></i>&nbsp หมวดหมู่ :</div>`
                html += `<div class='m-list-badge__items'>`
                var keysCategory = Object.keys($scope.LIST_TEACHER.LIST_MEMBERS[i].LIST_MEMBER_CETEGORY);
                var lenCategory = keysCategory.length;
                for (var j = 0; j < lenCategory; j++) {
                    if ($scope.LIST_TEACHER.LIST_MEMBERS[i].LIST_MEMBER_CETEGORY[j].CATEGORY_ID == 1) {
                        html += "<a class='m-list-badge__item m-list-badge__item--brand'>" + $scope.LIST_TEACHER.LIST_MEMBERS[i].LIST_MEMBER_CETEGORY[j].NAME + "</a>"
                    }
                    if ($scope.LIST_TEACHER.LIST_MEMBERS[i].LIST_MEMBER_CETEGORY[j].CATEGORY_ID == 2) {
                        html += "<a class='m-list-badge__item m-list-badge__item--focus'>" + $scope.LIST_TEACHER.LIST_MEMBERS[i].LIST_MEMBER_CETEGORY[j].NAME + "</a>"
                    }
                    if ($scope.LIST_TEACHER.LIST_MEMBERS[i].LIST_MEMBER_CETEGORY[j].CATEGORY_ID == 3) {
                        html += "<a class='m-list-badge__item m-list-badge__item--primary'>" + $scope.LIST_TEACHER.LIST_MEMBERS[i].LIST_MEMBER_CETEGORY[j].NAME + "</a>"
                    }
                    if ($scope.LIST_TEACHER.LIST_MEMBERS[i].LIST_MEMBER_CETEGORY[j].CATEGORY_ID == 4) {
                        html += "<a class='m-list-badge__item m-list-badge__item--success'>" + $scope.LIST_TEACHER.LIST_MEMBERS[i].LIST_MEMBER_CETEGORY[j].NAME + "</a>"
                    }
                    if ($scope.LIST_TEACHER.LIST_MEMBERS[i].LIST_MEMBER_CETEGORY[j].CATEGORY_ID == 5) {
                        html += "<a class='m-list-badge__item m-list-badge__item--danger'>" + $scope.LIST_TEACHER.LIST_MEMBERS[i].LIST_MEMBER_CETEGORY[j].NAME + "</a>"
                    }
                    if ($scope.LIST_TEACHER.LIST_MEMBERS[i].LIST_MEMBER_CETEGORY[j].CATEGORY_ID == 6) {
                        html += "<a class='m-list-badge__item m-list-badge__item--warning'>" + $scope.LIST_TEACHER.LIST_MEMBERS[i].LIST_MEMBER_CETEGORY[j].NAME + "</a>"
                    }
                    if ($scope.LIST_TEACHER.LIST_MEMBERS[i].LIST_MEMBER_CETEGORY[j].CATEGORY_ID == 7) {
                        html += "<a class='m-list-badge__item m-list-badge__item--success'>" + $scope.LIST_TEACHER.LIST_MEMBERS[i].LIST_MEMBER_CETEGORY[j].NAME + "</a>"
                    }
                }
                html += `</div>`
                html += `</div>`
                html += `</div>`
                ////======================================================================== 
                html += `<div class='m-list__content'>
                         <div class='m-list-badge m--margin-bottom-20'>
                         <div class ='m-list-badge__items'>
                         <i class ='fa fa-list-ul'></i> &nbsp`
                html += $scope.LIST_TEACHER.LIST_MEMBERS[i].ABOUT
                html += `</div>
                            </div>
                                </div>`
                html += "<button class='btn m-btn--pill m-btn--air btn-outline-success btn-sm m-btn m-btn--custom' ng-click='ShowModalTeacherProfile(" + $scope.LIST_TEACHER.LIST_MEMBERS[i].AUTO_ID + ")'>"
                html += `<span>
                         <i class='fa fa-plus'></i>
                         <span>ข้อมูลเพิ่มเติม</span>
                         </span>
                         </button>`
                if ($scope.LIST_TEACHER.LIST_MEMBERS[i].REGISTER_STATUS == false && $scope.LIST_TEACHER.LIST_MEMBERS[i].VERIFY == false) {
                    html += "<a href='#' class='btn m-btn--pill m-btn--air btn-outline-info btn-sm m-btn m-btn--custom' data-toggle='modal' data-target='#MODAL_ACCEPT_TEACHER" + $scope.LIST_TEACHER.LIST_MEMBERS[i].AUTO_ID + "'>"
                    html += `<span>
                             <i class='fa fa-check-square'></i>
                             <span>สมัครเรียน!!!</span>
                             </span>
                             </a>`
                }
                else if ($scope.LIST_TEACHER.LIST_MEMBERS[i].REGISTER_STATUS == false && $scope.LIST_TEACHER.LIST_MEMBERS[i].VERIFY == true) {
                    html += "<a href='#' class='btn m-btn--pill m-btn--air btn-outline-info btn-sm m-btn m-btn--custom' data-toggle='modal' data-target='#MODAL_ACCEPT_TEACHER" + $scope.LIST_TEACHER.LIST_MEMBERS[i].AUTO_ID + "'>"
                    html += `<span>
                             <i class='fa fa-check-hearth'></i>
                             <span>สมัครเรียน!!!</span>
                             </span>
                             </a>`
                }
                else {

                    html += "<button class='btn m-btn--pill m-btn--air btn-outline-info btn-sm m-btn m-btn--custom' data-toggle='modal' data-target='#MODAL_CANCEL_TEACHER" + $scope.LIST_TEACHER.LIST_MEMBERS[i].AUTO_ID + "'>\
                            <span>\
                            <i class='fa fa-star'></i>\
                            <span>สมัครเรียบร้อย</span>\
                            </span>\
                            </button>\
                    "
                }
                ////======================================================================== 

                ////======================================================================== 
                html += "   </div>"//ปิด md8
                html += "</div>"
                html += "</div>"
                html += "</div>"
                ////============================== MODAL ========================== 
                html += "<div class='modal fade' id='MODAL_ACCEPT_TEACHER" + $scope.LIST_TEACHER.LIST_MEMBERS[i].AUTO_ID + "' tabindex='-1' role='dialog' aria-labelledby='exampleModalCenterTitle' style='display: none' aria-hidden='true'>\
                        <div class='modal-dialog modal-lg' role='document'>\
                        <div class='modal-content'>\
                        <div class='modal-body'>\
                        <div class='m-portlet__body'>\
                        <div class='m-demo__preview m-demo__preview--badge'>\
                        <h4 class='m--font-bold m--font-bold'>\
                        <span>\
                        <i class='fa fa-address-card'></i>\
                        <input id='TEACHER_ROLE_ID' name='TEACHER_ROLE_ID' type='hidden' value=" + $scope.LIST_TEACHER.LIST_MEMBERS[i].AUTO_ID + " autocomplete='off'>\
                        <span> คุณจะเลือก " + $scope.LIST_TEACHER.LIST_MEMBERS[i].FULLNAME + " ใช่หรือไม่ ?</span>\
                        </span>\
                        </h4>\
                        </div>\
                        </div>\
                        </div>\
                        <div class='modal-footer'>\
                        <button class='btn m-btn--pill m-btn--air btn-primary' type='submit' ng-click='SelectTeacher(" + $scope.LIST_TEACHER.LIST_MEMBERS[i].AUTO_ID + ")' name='AcceptStudent' value='" + $scope.LIST_TEACHER.LIST_MEMBERS[i].AUTO_ID + "'>\
                        <span>\
                        <i class='fa fa-check-square'></i>\
                        <span>&nbsp; ตกลง</span>\
                        </span>\
                        </button>\
                        <button type='button' class='btn m-btn--pill m-btn--air btn-secondary m-btn m-btn--custom' data-dismiss='modal'>ปิด</button>\
                        </div>\
                        </div>\
                        </div>\
                        </div>\
                "
                ////============================== MODAL ==========================
                
                html += "<div class='modal fade' id='MODAL_CANCEL_TEACHER" + $scope.LIST_TEACHER.LIST_MEMBERS[i].AUTO_ID + "' tabindex='-1' role='dialog' aria-labelledby='exampleModalCenterTitle' style='display: none' aria-hidden='true'>\
                        <div class='modal-dialog modal-lg' role='document'>\
                        <div class='modal-content'>\
                        <div class='modal-body'>\
                        <div class='m-portlet__body'>\
                        <div class='m-demo__preview m-demo__preview--badge'>\
                        <h4 class='m--font-bold m--font-bold'>\
                        <span>\
                        <i class='fa fa-address-card'></i>\
                        <input id='TEACHER_ROLE_ID' name='TEACHER_ROLE_ID' type='hidden' value=" + $scope.LIST_TEACHER.LIST_MEMBERS[i].AUTO_ID + " autocomplete='off'>\
                        <span> คุณจะยกเลิกครู " + $scope.LIST_TEACHER.LIST_MEMBERS[i].FULLNAME + " ใช่หรือไม่ ?</span>\
                        </span>\
                        </h4>\
                        </div>\
                        </div>\
                        </div>\
                        <div class='modal-footer'>\
                        <button class='btn m-btn--pill m-btn--air btn-primary' type='submit' ng-click='CancelTeacher(" + $scope.LIST_TEACHER.LIST_MEMBERS[i].AUTO_ID + ")' name='AcceptStudent' value='" + $scope.LIST_TEACHER.LIST_MEMBERS[i].AUTO_ID + "'>\
                        <span>\
                        <i class='fa fa-check-square'></i>\
                        <span>&nbsp; ตกลง</span>\
                        </span>\
                        </button>\
                        <button type='button' class='btn m-btn--pill m-btn--air btn-secondary m-btn m-btn--custom' data-dismiss='modal'>ปิด</button>\
                        </div>\
                        </div>\
                        </div>\
                        </div>\
                "
                ////============================== MODAL ========================== 

                //console.log(html)
                var $OnCompile = $(html).appendTo('#listTeacher');
                $compile($OnCompile)($scope);


            }
        }
        else {
            var keys = Object.keys($scope.LIST_TEACHER.LIST_MEMBERS);
            var len = keys.length;
            $('#listTeacher').empty()
            $('#listCourse').empty()
            for (var i = 0; i < len; i++) {

                var html = "";
                html += "<div class='m-portlet m-portlet--rounded'>"
                html += "<div class='m-portlet__body'>"
                html += "<div class='row'>"
                html += "   <div class='col-md-3'>"//md3
                html += "       <div class='m-card-profile'><div class='m-card-profile__pic'><div class='m-card-profile__pic-wrapper'>"
                if ($scope.LIST_TEACHER.LIST_MEMBERS[i].PROFILE_IMG_URL == null) {
                    var pic = "..////Content//images//Blank-Profile.jpg";
                    html += "<img src= '" + pic + "' class='img-thumbnail' onclick = window.open('/content/images/blank-profile.jpg') style='height:130px;width:130px'  >"
                }
                else {
                    var pic = "..////" + $scope.LIST_TEACHER.LIST_MEMBERS[i].PROFILE_IMG_URL;
                    if ($scope.LIST_TEACHER.LIST_MEMBERS[i].PROFILE_IMG_URL_FULL != null) {
                        var fullPic = $scope.LIST_TEACHER.LIST_MEMBERS[i].PROFILE_IMG_URL_FULL.replace("\\", "//")
                        html += "<img src= '" + pic + "' class='img-thumbnail' style='height:130px;width:130px' onclick=window.open('https://www.coachme.asia///" + fullPic + "')>"
                    }
                    else {
                        var fullPic = $scope.LIST_TEACHER.LIST_MEMBERS[i].PROFILE_IMG_URL.replace("\\", "//")
                        html += "<img src= '" + pic + "' class='img-thumbnail' style='height:130px;width:130px' onclick=window.open('https://www.coachme.asia///" + fullPic + "')>"
                    }
                }
                html += ""
                html += "       </div></div></div> "
                html += "   </div>"//ปิด md3
                html += "   <div class='col-md-8'>"//md8
                ////======================================================================== 
                if ($scope.LIST_TEACHER.LIST_MEMBERS[i].VERIFY == false) {
                    html += `<div class='m-list__content'>
                                <div class='m-list-badge m--margin-bottom-20'>
                                <div class ='m-list-badge__label ' style='font-size:12px;font-weight:400;'>
                                <i class ='fa fa-user-graduate'></i>&nbsp ชื่อ :
                                </div>
                                <div class='m-list-badge__items'>`
                    html += $scope.LIST_TEACHER.LIST_MEMBERS[i].FULLNAME
                    html += `    </div>
                                </div>
                                </div>`

                }
                else if ($scope.LIST_TEACHER.LIST_MEMBERS[i].VERIFY == true) {
                    html += `<div class='m-list__content'>
                                <div class='m-list-badge m--margin-bottom-20'>
                                <div class='m-list-badge__label ' style='font-size:12px;font-weight:400;'><i class='fa fa-user-graduate'></i>&nbsp ชื่อ :</div>
                                <div class='m-list-badge__items'>`
                    html += $scope.LIST_TEACHER.LIST_MEMBERS[i].FULLNAME
                    html += `
                                <i class ='fa fa-check-circle text-success'></i>
                                </div>
                                </div>
                                </div>`

                }

                ////======================================================================== 
                html += `<div class='m-list__content'>`
                html += `<div class='m-list-badge m--margin-bottom-20'>`
                html += `<div class='m-list-badge__label' style='font-size:12px;font-weight:400;'><i class='fa fa-book-open'></i>&nbsp หมวดหมู่ :</div>`
                html += `<div class='m-list-badge__items'>`
                var keysCategory = Object.keys($scope.LIST_TEACHER.LIST_MEMBERS[i].LIST_MEMBER_CETEGORY);
                var lenCategory = keysCategory.length;
                for (var j = 0; j < lenCategory; j++) {
                    if ($scope.LIST_TEACHER.LIST_MEMBERS[i].LIST_MEMBER_CETEGORY[j].CATEGORY_ID == 1) {
                        html += "<a class='m-list-badge__item m-list-badge__item--brand'>" + $scope.LIST_TEACHER.LIST_MEMBERS[i].LIST_MEMBER_CETEGORY[j].NAME + "</a>"
                    }
                    if ($scope.LIST_TEACHER.LIST_MEMBERS[i].LIST_MEMBER_CETEGORY[j].CATEGORY_ID == 2) {
                        html += "<a class='m-list-badge__item m-list-badge__item--focus'>" + $scope.LIST_TEACHER.LIST_MEMBERS[i].LIST_MEMBER_CETEGORY[j].NAME + "</a>"
                    }
                    if ($scope.LIST_TEACHER.LIST_MEMBERS[i].LIST_MEMBER_CETEGORY[j].CATEGORY_ID == 3) {
                        html += "<a class='m-list-badge__item m-list-badge__item--primary'>" + $scope.LIST_TEACHER.LIST_MEMBERS[i].LIST_MEMBER_CETEGORY[j].NAME + "</a>"
                    }
                    if ($scope.LIST_TEACHER.LIST_MEMBERS[i].LIST_MEMBER_CETEGORY[j].CATEGORY_ID == 4) {
                        html += "<a class='m-list-badge__item m-list-badge__item--success'>" + $scope.LIST_TEACHER.LIST_MEMBERS[i].LIST_MEMBER_CETEGORY[j].NAME + "</a>"
                    }
                    if ($scope.LIST_TEACHER.LIST_MEMBERS[i].LIST_MEMBER_CETEGORY[j].CATEGORY_ID == 5) {
                        html += "<a class='m-list-badge__item m-list-badge__item--danger'>" + $scope.LIST_TEACHER.LIST_MEMBERS[i].LIST_MEMBER_CETEGORY[j].NAME + "</a>"
                    }
                    if ($scope.LIST_TEACHER.LIST_MEMBERS[i].LIST_MEMBER_CETEGORY[j].CATEGORY_ID == 6) {
                        html += "<a class='m-list-badge__item m-list-badge__item--warning'>" + $scope.LIST_TEACHER.LIST_MEMBERS[i].LIST_MEMBER_CETEGORY[j].NAME + "</a>"
                    }
                    if ($scope.LIST_TEACHER.LIST_MEMBERS[i].LIST_MEMBER_CETEGORY[j].CATEGORY_ID == 7) {
                        html += "<a class='m-list-badge__item m-list-badge__item--success'>" + $scope.LIST_TEACHER.LIST_MEMBERS[i].LIST_MEMBER_CETEGORY[j].NAME + "</a>"
                    }
                }
                html += `</div>`
                html += `</div>`
                html += `</div>`
                ////======================================================================== 
                html += `<div class='m-list__content'>
                         <div class='m-list-badge m--margin-bottom-20'>
                         <div class ='m-list-badge__items'>
                         <i class ='fa fa-list-ul'></i> &nbsp`
                html += $scope.LIST_TEACHER.LIST_MEMBERS[i].ABOUT
                html += `</div>
                            </div>
                                </div>`
                html += "<button class='btn m-btn--pill m-btn--air btn-outline-success btn-sm m-btn m-btn--custom' ng-click='ShowModalTeacherProfile(" + $scope.LIST_TEACHER.LIST_MEMBERS[i].AUTO_ID + ")'>"
                html += `<span>
                         <i class='fa fa-plus'></i>
                         <span>ข้อมูลเพิ่มเติม</span>
                         </span>
                         </button>`
                if ($scope.LIST_TEACHER.LIST_MEMBERS[i].VERIFY == false) {
                    html += `<a href='#' class='btn m-btn--pill m-btn--air btn-outline-info btn-sm m-btn m-btn--custom' data-toggle='modal' data-target='#m_modal_6'>
                             <span>
                             <i class='fa fa-check-square'></i>
                             <span>สมัครเรียน!!!</span>
                             </span>
                             </a>`
                }
                else {
                    html += `<a href='#' class='btn m-btn--pill m-btn--air btn-outline-info btn-sm m-btn m-btn--custom' data-toggle='modal' data-target='#m_modal_6'>
                             <span>
                             <i class='fa fa-check-heart'></i>
                             <span>สมัครเรียน!!!</span>
                             </span>
                             </a>`
                }
                ////======================================================================== 
                html += "   </div>"//ปิด md8
                html += "</div>"
                html += "</div>"
                html += "</div>"
                ////============================== MODAL ========================== 

                ////============================== MODAL ========================== 

                console.log(html)
                var $OnCompile = $(html).appendTo('#listTeacher');
                $compile($OnCompile)($scope);


            }
        }

    }

    $scope.renderCourseContent = function () {
        if ($scope.LIST_TEACHER.MEMBER_AUTO_ID > 0) {
            var keys = Object.keys($scope.LIST_COURSE.LIST_MEMBERS);
            var len = keys.length;
            $('#listCourse').empty()
            $('#listTeacher').empty()
            for (var i = 0; i < len; i++) {
                var html = "";

                html += "<div class='col-xl-4'>\
                   <div class='m-portlet m-portlet--bordered-semi m-portlet--full-height  m-portlet--rounded-force'>\
                    <div class='m-portlet__head m-portlet__head--fit'>\
                   <div class='m-portlet__head-caption'>\
                   <div class='m-portlet__head-action'>\
                   </div>\
                   </div>\
                   </div>\
            "
                //==================================================
                html += " <div class='m-portlet__body'>\
                      <div class='m-widget19'>\
                      <div class='m-widget19__pic m-portlet-fit--top m-portlet-fit--sides' style='min-height-: 286px'>\
            "
                if ($scope.LIST_COURSE.LIST_MEMBERS[i].COURSE_BANNER == null) {
                    var pic = "..////Content//images//Blank-Book.jpg";
                    html += "<img src= '" + pic + "' class='img-thumbnail' style='width:100%;height:230px'>"
                }
                else {
                    var pic = "..////" + $scope.LIST_COURSE.LIST_MEMBERS[i].COURSE_BANNER;
                    html += "<img src= '" + pic + "' class='img-thumbnail' style='width:100%;height:230px'>"
                }
                //==================================================
                html += " <h3 class='m-widget19__title m--font-light'>\
                      " + $scope.LIST_COURSE.LIST_MEMBERS[i].COURSE + "\
                    </h3>\
                    <div class='m-widget19__shadow'></div>\
                    </div>\
                    <div class='m-widget19__content'>\
                    <div class='m-widget19__header' style='margin-top:1.10rem;margin-bottom:1.10rem;'>\
                    <div class='m-widget19__info'>\
                    <i class='fa fa-user-graduate'></i>\
                    <span class='m-widget19__username' style='margin-bottom:0;'>\
                    " + $scope.LIST_COURSE.LIST_MEMBERS[i].FULLNAME + "\
                    </span><br>\
                    </div>\
                    </div>\
                    <div class='m-widget19__body'>\
                    " + $scope.LIST_COURSE.LIST_MEMBERS[i].ABOUT + "\
                    </div>\
                    </div>\
            "
                if ($scope.LIST_COURSE.LIST_MEMBERS[i].REGISTER_STATUS == false) {
                    html += "<div class='m-widget19__action'>\
                             <button type='button' class='btn m-btn--pill btn-brand m-btn--air m-btn--custom' data-toggle='modal' data-target='#MODAL_ACCEPT_COURSE" + $scope.LIST_COURSE.LIST_MEMBERS[i].COURSE_ID + "'><i class='fa fa-plus'></i>สมัครเรียน</button>\
                             </div>\
                            <div class='modal fade' id='MODAL_ACCEPT_COURSE" + $scope.LIST_COURSE.LIST_MEMBERS[i].COURSE_ID + "' tabindex='-1' role='dialog' aria-labelledby='exampleModalCenterTitle' style='display: none' aria-hidden='true'>\
                            <div class='modal-dialog modal-lg' role='document'>\
                            <div class='modal-content'>\
                            <div class='modal-body'>\
                            <div class='m-portlet__body'>\
                            <div class='m-demo__preview m-demo__preview--badge'>\
                            <h4 class='m--font-bold m--font-bold'>\
                            <span>\
                            <i class='fa fa-address-card'></i>\
                            <span> คุณจะเลือกคอร์ส " + $scope.LIST_COURSE.LIST_MEMBERS[i].COURSE + " ใช่หรือไม่ ?</span>\
                            </span>\
                            </h4>\
                            </div>\
                            </div>\
                            </div>\
                            <div class='modal-footer'>\
                            <button class='btn m-btn--pill m-btn--air btn-primary' ng-click='SelectCourse(" + $scope.LIST_COURSE.LIST_MEMBERS[i].COURSE_ID + ")'>\
                            <span>\
                            <i class='fa fa-check-square'></i>\
                            <span>ตกลง</span>\
                            </span>\
                            </button>\
                            <button type='button' class='btn m-btn--pill m-btn--air btn-secondary m-btn m-btn--custom' data-dismiss='modal'>ปิด</button>\
                            </div>\
                            </div>\
                            </div>\
                            </div>\
                    "
                }
                else {
                    html += "<div class='m-widget19__action'>\
                             <button type='button' class='btn m-btn--pill btn-success m-btn--air m-btn--custom' disabled='disabled'>\
                             <i class='fa fa-star'></i>สมัครเรียบร้อย\
                             </button>\
                             </div>\
                    "
                }


                console.log(html)
                var $OnCompile = $(html).appendTo('#listCourse');
                $compile($OnCompile)($scope);
            }
        }
        else {
            var keys = Object.keys($scope.LIST_COURSE.LIST_MEMBERS);
            var len = keys.length;
            $('#listCourse').empty()
            $('#listTeacher').empty()
            for (var i = 0; i < len; i++) {
                var html = "";

                html += "<div class='col-xl-4'>\
                   <div class='m-portlet m-portlet--bordered-semi m-portlet--full-height  m-portlet--rounded-force'>\
                    <div class='m-portlet__head m-portlet__head--fit'>\
                   <div class='m-portlet__head-caption'>\
                   <div class='m-portlet__head-action'>\
                   </div>\
                   </div>\
                   </div>\
            "
                //==================================================
                html += " <div class='m-portlet__body'>\
                      <div class='m-widget19'>\
                      <div class='m-widget19__pic m-portlet-fit--top m-portlet-fit--sides' style='min-height-: 286px'>\
            "
                if ($scope.LIST_COURSE.LIST_MEMBERS[i].COURSE_BANNER == null) {
                    var pic = "..////Content//images//Blank-Book.jpg";
                    html += "<img src= '" + pic + "' class='img-thumbnail' style='width:100%;height:230px'>"
                }
                else {
                    var pic = "..////" + $scope.LIST_COURSE.LIST_MEMBERS[i].COURSE_BANNER;
                    html += "<img src= '" + pic + "' class='img-thumbnail' style='width:100%;height:230px'>"
                }
                //==================================================
                html += " <h3 class='m-widget19__title m--font-light'>\
                      " + $scope.LIST_COURSE.LIST_MEMBERS[i].COURSE + "\
                    </h3>\
                    <div class='m-widget19__shadow'></div>\
                    </div>\
                    <div class='m-widget19__content'>\
                    <div class='m-widget19__header' style='margin-top:1.10rem;margin-bottom:1.10rem;'>\
                    <div class='m-widget19__info'>\
                    <i class='fa fa-user-graduate'></i>\
                    <span class='m-widget19__username' style='margin-bottom:0;'>\
                    " + $scope.LIST_COURSE.LIST_MEMBERS[i].FULLNAME + "\
                    </span><br>\
                    </div>\
                    </div>\
                    <div class='m-widget19__body'>\
                    " + $scope.LIST_COURSE.LIST_MEMBERS[i].ABOUT + "\
                    </div>\
                    </div>\
            "
                if ($scope.LIST_COURSE.LIST_MEMBERS[i].VERIFY == false) {
                    html += " <a href='#' class='btn m-btn--pill m-btn--air btn-outline-info btn-sm m-btn m-btn--custom' data-toggle='modal' data-target='#m_modal_6'>\
                          <span>\
                          <i class='fa fa-check-square'></i>\
                          <span>&nbsp สมัครเรียนคอร์สนี้!!!</span>\
                          </span>\
                          </a>\
                "
                }
                else {
                    html += " <a href='#' class='btn m-btn--pill m-btn--air btn-outline-info btn-sm m-btn m-btn--custom' data-toggle='modal' data-target='#m_modal_6'>\
                          <span>\
                           <i class='fa fa-heart'></i>\
                          <span>&nbsp สมัครเรียนคอร์สนี้!!!</span>\
                          </span>\
                          </a>\
                "
                }
                html += " </div>\
                      </div>\
                      </div>\
                      </div>\
                    "

                console.log(html)
                var $OnCompile = $(html).appendTo('#listCourse');
                $compile($OnCompile)($scope);
            }
        }
    }

    $scope.ShowModalTeacherProfile = function (AUTO_ID) {

        var keys = Object.keys($scope.LIST_TEACHER.LIST_MEMBERS);
        var len = keys.length;
        var html = "";
        $('#modalTeacherProfile').empty()
        for (var i = 0; i < len; i++) {
            if ($scope.LIST_TEACHER.LIST_MEMBERS[i].AUTO_ID == AUTO_ID) {
                html += "<div class='modal fade' id='modalProfile' tabindex='-1' role='dialog' aria-labelledby='exampleModalCenterTitle' style='display: none' aria-hidden='true'> \
                         <div class ='modal-dialog modal-lg' role='document'>\
                         <div class ='modal-content'>\
                         <div class ='m-portlet m-portlet--full-height   m-portlet--unair'>\
                         <div class ='m-portlet__body'>\
                         <div class ='m-card-profile'>\
                         <div class ='m-card-profile__title m--hide'>\
                         </div>\
                         <div class ='m-card-profile__pic'>\
                         <div class ='m-card-profile__pic-wrapper'>\
                    "
                if ($scope.LIST_TEACHER.LIST_MEMBERS[i].PROFILE_IMG_URL == null) {
                    var pic = "..////Content//images//Blank-Profile.jpg";
                    html += "<img src= '" + pic + "' class='img-thumbnail' onclick = window.open('/content/images/blank-profile.jpg') style='height:130px;width:130px' >"
                }
                else {
                    var pic = "..////" + $scope.LIST_TEACHER.LIST_MEMBERS[i].PROFILE_IMG_URL;
                    if ($scope.LIST_TEACHER.LIST_MEMBERS[i].PROFILE_IMG_URL_FULL != null) {
                        var fullPic = $scope.LIST_TEACHER.LIST_MEMBERS[i].PROFILE_IMG_URL_FULL.replace("\\", "//")
                        html += "<img src= '" + pic + "' class='img-thumbnail' style='height:130px;width:130px' onclick=window.open('https://www.coachme.asia///" + fullPic + "')>"
                    }
                    else {
                        var fullPic = $scope.LIST_TEACHER.LIST_MEMBERS[i].PROFILE_IMG_URL.replace("\\", "//")
                        html += "<img src= '" + pic + "' class='img-thumbnail' style='height:130px;width:130px' onclick=window.open('https://www.coachme.asia///" + fullPic + "')>"
                    }
                }
                html += "</div>\
                        <div class='m-card-profile__details'>\
                        <span class='m-card-profile__name'>\
                         " + $scope.LIST_TEACHER.LIST_MEMBERS[i].FULLNAME + "\
                        <br>\
                         สถานที่ : " + $scope.LIST_TEACHER.LIST_MEMBERS[i].LOCATION + "\
                        <br>\
                        เกี่ยวกับครู : " + $scope.LIST_TEACHER.LIST_MEMBERS[i].ABOUT + "\
                        <br>\
                         ประเภทการเรียน : " + $scope.LIST_TEACHER.LIST_MEMBERS[i].TEACHING_TYPE_NAME + "\
                        <br>\
                         ระดับผู้เรียน : " + $scope.LIST_TEACHER.LIST_MEMBERS[i].STUDENT_LEVEL_NAME + "\
                        </span>\
                        <br>\
                        <br>\
                "
                if ($scope.LIST_TEACHER.LIST_MEMBERS[i].ABOUT_IMG_1 == null) {

                    var pic = "..////Content//images//Blank-Profile.jpg";
                    html += "<a><img src= '" + pic + "' class='img-thumbnail' style='height:130px;width:130px'></a>"
                }
                else {
                    var pic = "..////" + $scope.LIST_TEACHER.LIST_MEMBERS[i].ABOUT_IMG_1;
                    html += "<a><img src= '" + pic + "' class='img-thumbnail' style='height:130px;width:130px'></a>"
                }
                if ($scope.LIST_TEACHER.LIST_MEMBERS[i].ABOUT_IMG_2 == null) {

                    var pic = "..////Content//images//Blank-Profile.jpg";
                    html += "<a><img src= '" + pic + "' class='img-thumbnail' style='height:130px;width:130px'></a>"
                }
                else {
                    var pic = "..////" + $scope.LIST_TEACHER.LIST_MEMBERS[i].ABOUT_IMG_2;
                    html += "<a><img src= '" + pic + "' class='img-thumbnail' style='height:130px;width:130px'></a>"
                }
                if ($scope.LIST_TEACHER.LIST_MEMBERS[i].ABOUT_IMG_3 == null) {

                    var pic = "..////Content//images//Blank-Profile.jpg";
                    html += "<a><img src= '" + pic + "' class='img-thumbnail' style='height:130px;width:130px'></a>"
                }
                else {
                    var pic = "..////" + $scope.LIST_TEACHER.LIST_MEMBERS[i].ABOUT_IMG_3;
                    html += "<a><img src= '" + pic + "' class='img-thumbnail' style='height:130px;width:130px'></a>"
                }
                if ($scope.LIST_TEACHER.LIST_MEMBERS[i].ABOUT_IMG_4 == null) {

                    var pic = "..////Content//images//Blank-Profile.jpg";
                    html += "<a><img src= '" + pic + "' class='img-thumbnail' style='height:130px;width:130px'></a>"
                }
                else {
                    var pic = "..////" + $scope.LIST_TEACHER.LIST_MEMBERS[i].ABOUT_IMG_4;
                    html += "<a><img src= '" + pic + "' class='img-thumbnail' style='height:130px;width:130px'></a>"
                }
                html += "</div></div></div></div>\
                        <div class='modal-footer'>\
                        <button type='button' class='btn m-btn--pill m-btn--air         btn-outline-brand btn-sm' data-dismiss='modal'>Close</button>\
                        </div>\
                        </div>\
                        </div>\
                        </div>\
                       "
            }
        }
        $('#modalTeacherProfile').append(html)
        $('#modalProfile').modal('show');


    }

    $scope.SelectTeacher = function (TEACHER_AUTO_ID) {


        $("#MODAL_ACCEPT_TEACHER" + TEACHER_AUTO_ID + "").modal('hide');
        $http({
            url: "/home/SelectTeacher",
            method: "GET",
            params: { MEMBER_AUTO_ID: $scope.LIST_TEACHER.MEMBER_AUTO_ID, TEACHER_AUTO_ID: TEACHER_AUTO_ID },
        }).then(function (response) {
            console.log(response.data.OUTPUT_DATA)
            if (response.data.STATUS == true) {
                $("#m_modal_1").modal('show');
                $scope.Search()
            }
            else {
                $("#m_modal_2").modal('show');
                $scope.Search()
            }
        });
    }

    $scope.CancelTeacher = function (TEACHER_AUTO_ID) {


        $("#MODAL_CANCEL_TEACHER" + TEACHER_AUTO_ID + "").modal('hide');
        $http({
            url: "/home/CancelTeacher",
            method: "GET",
            params: { MEMBER_AUTO_ID: $scope.LIST_TEACHER.MEMBER_AUTO_ID, TEACHER_AUTO_ID: TEACHER_AUTO_ID },
        }).then(function (response) {
            console.log(response.data.OUTPUT_DATA)
            if (response.data.STATUS == true) {
                $("#m_modal_1").modal('show');
                $scope.Search()
            }
            else {
                $("#m_modal_2").modal('show');
                $scope.Search()
            }
        });
    }

    $scope.SelectCourse = function (COURSE_ID) {


        $("#MODAL_ACCEPT_COURSE" + COURSE_ID + "").modal('hide');
        $http({
            url: "/home/SelectCourse",
            method: "GET",
            params: { MEMBER_AUTO_ID: $scope.LIST_TEACHER.MEMBER_AUTO_ID, COURSE_ID: COURSE_ID },
        }).then(function (response) {
            console.log(response.data.OUTPUT_DATA)
            if (response.data.STATUS == true) {
                $("#m_modal_1").modal('show');
                $scope.Search()
            }
            else {
                $("#m_modal_2").modal('show');
                $scope.Search()
            }
        });
    }



});