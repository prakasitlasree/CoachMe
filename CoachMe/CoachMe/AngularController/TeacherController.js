﻿/// <reference path="D:\PXProject\CoachMe\CoachMe\CoachMe\Scripts/loadingoverlay.min.js" />
var app = angular.module('TeacherApp', []);
app.controller('ListCategoryController', function ($scope, $http, $compile) {

    !function (e) { "function" == typeof define && define.amd ? define(["jquery"], e) : "object" == typeof module && module.exports ? e(require("jquery")) : e(jQuery) }(function (g, a) { "use strict"; var o = { background: "rgba(255, 255, 255, 0.8)", backgroundClass: "", image: "<svg xmlns='http://www.w3.org/2000/svg' viewBox='0 0 1000 1000'><circle r='80' cx='500' cy='90'/><circle r='80' cx='500' cy='910'/><circle r='80' cx='90' cy='500'/><circle r='80' cx='910' cy='500'/><circle r='80' cx='212' cy='212'/><circle r='80' cx='788' cy='212'/><circle r='80' cx='212' cy='788'/><circle r='80' cx='788' cy='788'/></svg>", imageAnimation: "2000ms rotate_right", imageAutoResize: !0, imageResizeFactor: 1, imageColor: "#202020", imageClass: "", imageOrder: 1, fontawesome: "", fontawesomeAnimation: "", fontawesomeAutoResize: !0, fontawesomeResizeFactor: 1, fontawesomeColor: "#202020", fontawesomeOrder: 2, custom: "", customAnimation: "", customAutoResize: !0, customResizeFactor: 1, customOrder: 3, text: "", textAnimation: "", textAutoResize: !0, textResizeFactor: .5, textColor: "#202020", textClass: "", textOrder: 4, progress: !1, progressAutoResize: !0, progressResizeFactor: .25, progressColor: "#a0a0a0", progressClass: "", progressOrder: 5, progressFixedPosition: "", progressSpeed: 200, progressMin: 0, progressMax: 100, size: 50, maxSize: 120, minSize: 20, direction: "column", fade: !0, resizeInterval: 50, zIndex: 2147483647 }, c = { overlay: { "box-sizing": "border-box", position: "relative", display: "flex", "flex-wrap": "nowrap", "align-items": "center", "justify-content": "space-around" }, element: { "box-sizing": "border-box", overflow: "visible", flex: "0 0 auto", display: "flex", "justify-content": "center", "align-items": "center" }, element_svg: { width: "100%", height: "100%" }, progress_fixed: { position: "absolute", left: "0", width: "100%" }, progress_wrapper: { position: "absolute", top: "0", left: "0", width: "100%", height: "100%" }, progress_bar: { position: "absolute", left: "0" } }, n = { count: 0, container: a, settings: a, wholePage: a, resizeIntervalId: a, text: a, progress: a }, s = { animations: ["rotate_right", "rotate_left", "fadein", "pulse"], progressPosition: ["top", "bottom"] }, d = { animations: { name: "rotate_right", time: "2000ms" }, fade: [400, 200] }; function r(e, s) { e = g(e), s.size = b(s.size), s.maxSize = parseInt(s.maxSize, 10) || 0, s.minSize = parseInt(s.minSize, 10) || 0, s.resizeInterval = parseInt(s.resizeInterval, 10) || 0; var t = u(e), a = v(e); if (!1 === a) { if ((a = g.extend({}, n)).container = e, a.wholePage = e.is("body"), t = g("<div>", { class: "loadingoverlay" }).css(c.overlay).css("flex-direction", "row" === s.direction.toLowerCase() ? "row" : "column"), s.backgroundClass ? t.addClass(s.backgroundClass) : t.css("background", s.background), a.wholePage && t.css({ position: "fixed", top: 0, left: 0, width: "100%", height: "100%" }), void 0 !== s.zIndex && t.css("z-index", s.zIndex), s.image) { g.isArray(s.imageColor) ? 0 === s.imageColor.length ? s.imageColor = !1 : 1 === s.imageColor.length ? s.imageColor = { fill: s.imageColor[0] } : s.imageColor = { fill: s.imageColor[0], stroke: s.imageColor[1] } : s.imageColor && (s.imageColor = { fill: s.imageColor }); var o = x(t, s.imageOrder, s.imageAutoResize, s.imageResizeFactor, s.imageAnimation); "<svg" === s.image.slice(0, 4).toLowerCase() && "</svg>" === s.image.slice(-6).toLowerCase() ? (o.append(s.image), o.children().css(c.element_svg), !s.imageClass && s.imageColor && o.find("*").css(s.imageColor)) : ".svg" === s.image.slice(-4).toLowerCase() || "data:image/svg" === s.image.slice(0, 14).toLowerCase() ? g.ajax({ url: s.image, type: "GET", dataType: "html", global: !1 }).done(function (e) { o.html(e), o.children().css(c.element_svg), !s.imageClass && s.imageColor && o.find("*").css(s.imageColor) }) : o.css({ "background-image": "url(" + s.image + ")", "background-position": "center", "background-repeat": "no-repeat", "background-size": "cover" }), s.imageClass && o.addClass(s.imageClass) } if (s.fontawesome) { o = x(t, s.fontawesomeOrder, s.fontawesomeAutoResize, s.fontawesomeResizeFactor, s.fontawesomeAnimation).addClass("loadingoverlay_fa"); g("<div>", { class: s.fontawesome }).appendTo(o), s.fontawesomeColor && o.css("color", s.fontawesomeColor) } if (s.custom) o = x(t, s.customOrder, s.customAutoResize, s.customResizeFactor, s.customAnimation).append(s.custom); if (s.text && (a.text = x(t, s.textOrder, s.textAutoResize, s.textResizeFactor, s.textAnimation).addClass("loadingoverlay_text").text(s.text), s.textClass ? a.text.addClass(s.textClass) : s.textColor && a.text.css("color", s.textColor)), s.progress) { o = x(t, s.progressOrder, s.progressAutoResize, s.progressResizeFactor, !1).addClass("loadingoverlay_progress"); var r = g("<div>").css(c.progress_wrapper).appendTo(o); a.progress = { bar: g("<div>").css(c.progress_bar).appendTo(r), fixed: !1, margin: 0, min: parseFloat(s.progressMin), max: parseFloat(s.progressMax), speed: parseInt(s.progressSpeed, 10) }; var i = (s.progressFixedPosition + "").replace(/\s\s+/g, " ").toLowerCase().split(" "); 2 === i.length && w(i[0]) ? (a.progress.fixed = i[0], a.progress.margin = b(i[1])) : 2 === i.length && w(i[1]) ? (a.progress.fixed = i[1], a.progress.margin = b(i[0])) : 1 === i.length && w(i[0]) && (a.progress.fixed = i[0], a.progress.margin = 0), "top" === a.progress.fixed ? o.css(c.progress_fixed).css("top", a.progress.margin ? a.progress.margin.value + (a.progress.margin.fixed ? a.progress.margin.units : "%") : 0) : "bottom" === a.progress.fixed && o.css(c.progress_fixed).css("top", "auto"), s.progressClass ? a.progress.bar.addClass(s.progressClass) : s.progressColor && a.progress.bar.css("background", s.progressColor) } s.fade ? !0 === s.fade ? s.fade = d.fade : "string" == typeof s.fade || "number" == typeof s.fade ? s.fade = [s.fade, s.fade] : g.isArray(s.fade) && s.fade.length < 2 && (s.fade = [s.fade[0], s.fade[0]]) : s.fade = [0, 0], s.fade = [parseInt(s.fade[0], 10), parseInt(s.fade[1], 10)], a.settings = s, t.data("loadingoverlay_data", a), e.data("loadingoverlay", t), t.fadeTo(0, .01).appendTo("body"), p(e, !0), 0 < s.resizeInterval && (a.resizeIntervalId = setInterval(function () { p(e, !1) }, s.resizeInterval)), t.fadeTo(s.fade[0], 1) } a.count++ } function i(e, s) { var t = u(e = g(e)), a = v(e); !1 !== a && (a.count--, (s || a.count <= 0) && t.animate({ opacity: 0 }, a.settings.fade[1], function () { a.resizeIntervalId && clearInterval(a.resizeIntervalId), g(this).remove(), e.removeData("loadingoverlay") })) } function l(e) { p(g(e), !0) } function m(e, s) { var t = v(e = g(e)); !1 !== t && t.text && (!1 === s ? t.text.hide() : t.text.show().text(s)) } function f(e, s) { var t = v(e = g(e)); if (!1 !== t && t.progress) if (!1 === s) t.progress.bar.hide(); else { var a = 100 * ((parseFloat(s) || 0) - t.progress.min) / (t.progress.max - t.progress.min); a < 0 && (a = 0), 100 < a && (a = 100), t.progress.bar.show().animate({ width: a + "%" }, t.progress.speed) } } function p(e, t) { var s = u(e), a = v(e); if (!1 !== a) { if (!a.wholePage) { var o = "fixed" === e.css("position"), r = o ? e[0].getBoundingClientRect() : e.offset(); s.css({ position: o ? "fixed" : "absolute", top: r.top + parseInt(e.css("border-top-width"), 10), left: r.left + parseInt(e.css("border-left-width"), 10), width: e.innerWidth(), height: e.innerHeight() }) } if (a.settings.size) { var i = a.wholePage ? g(window) : e, n = a.settings.size.value; a.settings.size.fixed || (n = Math.min(i.innerWidth(), i.innerHeight()) * n / 100, a.settings.maxSize && n > a.settings.maxSize && (n = a.settings.maxSize), a.settings.minSize && n < a.settings.minSize && (n = a.settings.minSize)), s.children(".loadingoverlay_element").each(function () { var e = g(this); if (t || e.data("loadingoverlay_autoresize")) { var s = e.data("loadingoverlay_resizefactor"); e.hasClass("loadingoverlay_fa") || e.hasClass("loadingoverlay_text") ? e.css("font-size", n * s + a.settings.size.units) : e.hasClass("loadingoverlay_progress") ? (a.progress.bar.css("height", n * s + a.settings.size.units), a.progress.fixed ? "bottom" === a.progress.fixed && e.css("bottom", a.progress.margin ? a.progress.margin.value + (a.progress.margin.fixed ? a.progress.margin.units : "%") : 0).css("bottom", "+=" + n * s + a.settings.size.units) : a.progress.bar.css("top", e.position().top).css("top", "-=" + n * s * .5 + a.settings.size.units)) : e.css({ width: n * s + a.settings.size.units, height: n * s + a.settings.size.units }) } }) } } } function u(e) { return e.data("loadingoverlay") } function v(e) { var s = u(e), t = void 0 === s ? a : s.data("loadingoverlay_data"); return void 0 === t ? (g(".loadingoverlay").each(function () { var e = g(this), s = e.data("loadingoverlay_data"); document.body.contains(s.container[0]) || (s.resizeIntervalId && clearInterval(s.resizeIntervalId), e.remove()) }), !1) : (s.toggle(e.is(":visible")), t) } function x(e, s, t, a, o) { var r = g("<div>", { class: "loadingoverlay_element", css: { order: s } }).css(c.element).data({ loadingoverlay_autoresize: t, loadingoverlay_resizefactor: a }).appendTo(e); if (!0 === o && (o = d.animations.time + " " + d.animations.name), "string" == typeof o) { var i, n, l = o.replace(/\s\s+/g, " ").toLowerCase().split(" "); 2 === l.length && h(l[0]) && y(l[1]) ? (i = l[1], n = l[0]) : 2 === l.length && h(l[1]) && y(l[0]) ? (i = l[0], n = l[1]) : 1 === l.length && h(l[0]) ? (i = d.animations.name, n = l[0]) : 1 === l.length && y(l[0]) && (i = l[0], n = d.animations.time), r.css({ "animation-name": "loadingoverlay_animation__" + i, "animation-duration": n, "animation-timing-function": "linear", "animation-iteration-count": "infinite" }) } return r } function h(e) { return !isNaN(parseFloat(e)) && ("s" === e.slice(-1) || "ms" === e.slice(-2)) } function y(e) { return -1 < s.animations.indexOf(e) } function w(e) { return -1 < s.progressPosition.indexOf(e) } function b(e) { return !(!e || e < 0) && ("string" == typeof e && -1 < ["vmin", "vmax"].indexOf(e.slice(-4)) ? { fixed: !0, units: e.slice(-4), value: e.slice(0, -4) } : "string" == typeof e && -1 < ["rem"].indexOf(e.slice(-3)) ? { fixed: !0, units: e.slice(-3), value: e.slice(0, -3) } : "string" == typeof e && -1 < ["px", "em", "cm", "mm", "in", "pt", "pc", "vh", "vw"].indexOf(e.slice(-2)) ? { fixed: !0, units: e.slice(-2), value: e.slice(0, -2) } : { fixed: !1, units: "px", value: parseFloat(e) }) } g.LoadingOverlaySetup = function (e) { g.extend(!0, o, e) }, g.LoadingOverlay = function (e, s) { switch (e.toLowerCase()) { case "show": r("body", g.extend(!0, {}, o, s)); break; case "hide": i("body", s); break; case "resize": l("body"); break; case "text": m("body", s); break; case "progress": f("body", s) } }, g.fn.LoadingOverlay = function (e, s) { switch (e.toLowerCase()) { case "show": var t = g.extend(!0, {}, o, s); return this.each(function () { r(this, t) }); case "hide": return this.each(function () { i(this, s) }); case "resize": return this.each(function () { l(this) }); case "text": return this.each(function () { m(this, s) }); case "progress": return this.each(function () { f(this, s) }) } }, g(function () { g("head").append(["<style>", "@-webkit-keyframes loadingoverlay_animation__rotate_right {", "to {", "-webkit-transform : rotate(360deg);", "transform : rotate(360deg);", "}", "}", "@keyframes loadingoverlay_animation__rotate_right {", "to {", "-webkit-transform : rotate(360deg);", "transform : rotate(360deg);", "}", "}", "@-webkit-keyframes loadingoverlay_animation__rotate_left {", "to {", "-webkit-transform : rotate(-360deg);", "transform : rotate(-360deg);", "}", "}", "@keyframes loadingoverlay_animation__rotate_left {", "to {", "-webkit-transform : rotate(-360deg);", "transform : rotate(-360deg);", "}", "}", "@-webkit-keyframes loadingoverlay_animation__fadein {", "0% {", "opacity   : 0;", "-webkit-transform : scale(0.1, 0.1);", "transform : scale(0.1, 0.1);", "}", "50% {", "opacity   : 1;", "}", "100% {", "opacity   : 0;", "-webkit-transform : scale(1, 1);", "transform : scale(1, 1);", "}", "}", "@keyframes loadingoverlay_animation__fadein {", "0% {", "opacity   : 0;", "-webkit-transform : scale(0.1, 0.1);", "transform : scale(0.1, 0.1);", "}", "50% {", "opacity   : 1;", "}", "100% {", "opacity   : 0;", "-webkit-transform : scale(1, 1);", "transform : scale(1, 1);", "}", "}", "@-webkit-keyframes loadingoverlay_animation__pulse {", "0% {", "-webkit-transform : scale(0, 0);", "transform : scale(0, 0);", "}", "50% {", "-webkit-transform : scale(1, 1);", "transform : scale(1, 1);", "}", "100% {", "-webkit-transform : scale(0, 0);", "transform : scale(0, 0);", "}", "}", "@keyframes loadingoverlay_animation__pulse {", "0% {", "-webkit-transform : scale(0, 0);", "transform : scale(0, 0);", "}", "50% {", "-webkit-transform : scale(1, 1);", "transform : scale(1, 1);", "}", "100% {", "-webkit-transform : scale(0, 0);", "transform : scale(0, 0);", "}", "}", "</style>"].join(" ")) }) });

    $scope.ListCourse = [];
    $scope.TempListCourse = [];
    $scope.TeacherProfile = [];
    $scope.ListCategoryAvailable = [];
    $scope.selectedCategory;
    $scope.chkCategory = {};

    $scope.ListGeography = [];
    $scope.ListProvince = [];
    $scope.ListAmphur = [];
    $scope.address = [null];

    $scope.geography
    $scope.provinceID
    $scope.amphurID
    $scope.TEACHING_TYPE
    $scope.STUDENT_LEVEL
    $scope.SEX_RADIO

    $scope.TEACHING_TYPE;
    $scope.STUDENT_LEVEL;
    $scope.FULLNAME;
    $scope.FIRST_NAME;
    $scope.LAST_NAME;
    $scope.MOBILE;
    $scope.NICKNAME;
    $scope.DATE_OF_BIRTH;
    $scope.SEX;
    $scope.ABOUT;
    $scope.ABOUT_IMG_1 = "";
    $scope.ABOUT_IMG_2 = "";
    $scope.ABOUT_IMG_3 = "";
    $scope.ABOUT_IMG_4 = "";
    $scope.ABOUT_IMG = new Array(4);


    $scope.EnableControl = function () {
        $("#FULLNAME").prop("disabled", false);
        $("#FIRST_NAME").prop("disabled", false);
        $("#LAST_NAME").prop("disabled", false);
        $("#MOBILE").prop("disabled", false);
        $("#ABOUT").prop("disabled", false);
        $("#NICKNAME").prop("disabled", false);
        $("#DATE_OF_BIRTH").prop("disabled", false);
        $("#SEX").prop("disabled", false);
        $("#LOCATE").prop("disabled", false);
        $("#TEACHING_TYPE").prop("disabled", false);
        $("#STUDENT_LEVEL").prop("disabled", false);
        $("#editAddress").show();
        $("#ADDRESS").hide();

        $("#SEX_RADIO").show();
        $("#SEX").hide();

        $("#btnAboutImg_1").prop("disabled", false);
        $("#AboutImg_1").css('cursor', 'pointer');
        $("#btnAboutImg_2").prop("disabled", false);
        $("#AboutImg_2").css('cursor', 'pointer');
        $("#btnAboutImg_3").prop("disabled", false);
        $("#AboutImg_3").css('cursor', 'pointer');
        $("#btnAboutImg_4").prop("disabled", false);
        $("#AboutImg_4").css('cursor', 'pointer');
    }
    $scope.HideButton = function () {
        $('#btnUpdateSubmit').hide()
        $('#btnUpdateCancel').hide()

        $("#SEX_RADIO").hide();
        $("#SEX_RADIO_BTN").prop("disabled", true);
        $("#SEX").show();

        $("#FULLNAME").prop("disabled", true);
        $("#FIRST_NAME").prop("disabled", true);
        $("#LAST_NAME").prop("disabled", true);
        $("#MOBILE").prop("disabled", true);
        $("#ABOUT").prop("disabled", true);
        $("#NICKNAME").prop("disabled", true);
        $("#DATE_OF_BIRTH").prop("disabled", true);
        $("#SEX").prop("disabled", true);
        $("#ID_CARD").prop("disabled", true);
        $("#ADDRESS").show();
        $('#editAddress').hide();
        $('#LOCATE').show();
        $("#LOCATE").prop("disabled", true);
        $("#TEACHING_TYPE").prop("disabled", true);
        $("#STUDENT_LEVEL").prop("disabled", true);

        $("#btnAboutImg_1").prop("disabled", true);
        $("#AboutImg_1").css('cursor', 'pointer');
        $("#btnAboutImg_2").prop("disabled", true);
        $("#AboutImg_2").css('cursor', 'pointer');
        $("#btnAboutImg_3").prop("disabled", true);
        $("#AboutImg_3").css('cursor', 'pointer');
        $("#btnAboutImg_4").prop("disabled", true);
        $("#AboutImg_4").css('cursor', 'pointer');
        $("#CATEGORY").prop("disabled", true);
        $("#LOCATION").prop("disabled", true);
    }

    $scope.GetComponent = function () {
        $scope.EnableControl();
        $('#editAddress').show();

        $http({
            url: "/teacher/GetGeography",
            method: "GET",
            //params: { OrderID: $scope.orderId }
        }).then(function (response) {
            console.log(response.data.OUTPUT_DATA)
            if (response.data.STATUS == true) {
                $scope.ListGeography = response.data.OUTPUT_DATA;
                $scope.RenderGeographyDrp()
            }
        });
    }

    $scope.RenderGeographyDrp = function () {
        $('#geography').empty();
        debugger;
        var html = "";
        var itemsLength = Object.keys($scope.ListGeography).length;

        for (var i = 0; i < itemsLength; i++) {
            html += "<option value = " + $scope.ListGeography[i].GEO_ID + ">" + $scope.ListGeography[i].GEO_NAME + "</option>";
        }
        var $OnCompile = $(html).appendTo('#geography');
        $compile($OnCompile)($scope);
    }

    $scope.GetProvince = function () {
        $http({
            url: "/teacher/GetListProvince",
            method: "GET",
            params: { geoID: $scope.geography }
        }).then(function (response) {
            console.log(response.data.OUTPUT_DATA)
            if (response.data.STATUS == true) {
                $scope.ListProvince = response.data.OUTPUT_DATA;
                $scope.RenderProvinceDrp()
            }
        });
    }

    $scope.RenderProvinceDrp = function () {
        $('#province').empty();
        debugger;
        var html = "";

        var itemsLength = Object.keys($scope.ListProvince).length;
        for (var i = 0; i < itemsLength; i++) {
            html += "<option value = " + $scope.ListProvince[i].PROVINCE_ID + ">" + $scope.ListProvince[i].PROVINCE_NAME + "</option>";
        }
        var $OnCompile = $(html).appendTo('#province');
        $compile($OnCompile)($scope);
    }

    $scope.GetAmphur = function () {
        $http({
            url: "/teacher/GetListAmphur",
            method: "GET",
            params: { provinceID: $scope.provinceID }
        }).then(function (response) {
            console.log(response.data.OUTPUT_DATA)
            if (response.data.STATUS == true) {
                $scope.ListAmphur = response.data.OUTPUT_DATA;
                $scope.RenderAmphurDrp()
            }
        });
    }

    $scope.RenderAmphurDrp = function () {
        $('#amphur').empty();
        debugger;
        var html = "";

        var itemsLength = Object.keys($scope.ListAmphur).length;
        for (var i = 0; i < itemsLength; i++) {
            html += "<option value = " + $scope.ListAmphur[i].AMPHUR_ID + ">" + $scope.ListAmphur[i].AMPHUR_NAME + "</option>";
        }
        var $OnCompile = $(html).appendTo('#amphur');
        $compile($OnCompile)($scope);
    }

    $scope.AddSelectedCategory = function () {
        debugger;
        $('#SelectedCategory').append(" #" + $scope.selectedCategory);
    }

    $scope.init = function () {
        debugger;
        $.LoadingOverlay("show");
       

        $http({
            url: "/teacher/GetTeacherProfile",
            method: "GET",
            //params: { OrderID: $scope.orderId }
        }).then(function (response) {
            console.log(response.data.OUTPUT_DATA)
            if (response.data.STATUS == true) {
                $scope.TeacherProfile = response.data.OUTPUT_DATA;
                $scope.BindingProfile();
                $http({
                    url: "/teacher/GetAdress",
                    method: "GET",
                    //params: { OrderID: $scope.orderId }
                }).then(function (response) {
                    console.log(response.data.OUTPUT_DATA)
                    if (response.data.STATUS == true) {
                        $scope.address = response.data.OUTPUT_DATA;
                        if ($scope.address[0] != null) {
                            var address = "จังหวัด : " + $scope.address[1] + " อำเภอ : " + $scope.address[0]
                            $("#ADDRESS").val(address);
                        }
                        $http({
                            url: "/teacher/GetListTeacherCategory",
                            method: "GET",
                        }).then(function (response) {
                            console.log(response.data.OUTPUT_DATA)
                            if (response.data.STATUS == true) {
                                $scope.ListCourse = response.data.OUTPUT_DATA;
                                $scope.renderInnerHtml();
                            }
                        });
                    }
                });
            }
        });

        


        $.LoadingOverlay("hide");
    };



    $scope.BindingProfile = function () {

        debugger;





        $("#TEACHING_TYPE").val($scope.TeacherProfile[0].TEACHING_TYPE);
        $("#STUDENT_LEVEL").val($scope.TeacherProfile[0].STUDENT_LEVEL);
        $("#LOCATE").val($scope.TeacherProfile[0].LOCATION);

        $scope.FULLNAME = $scope.TeacherProfile[0].FULLNAME
        $scope.FIRST_NAME = $scope.TeacherProfile[0].FIRST_NAME
        $scope.LAST_NAME = $scope.TeacherProfile[0].LAST_NAME
        $scope.MOBILE = $scope.TeacherProfile[0].MOBILE
        $scope.DATE_OF_BIRTH_TEXT = $scope.TeacherProfile[0].DATE_OF_BIRTH_TEXT
        $scope.NICKNAME = $scope.TeacherProfile[0].NICKNAME
        $scope.LOCATION = $scope.TeacherProfile[0].LOCATION
        $scope.TEACHING_TYPE = $scope.TeacherProfile[0].TEACHING_TYPE
        $scope.STUDENT_LEVEL = $scope.TeacherProfile[0].STUDENT_LEVEL
        if ($scope.TeacherProfile[0].AMPHUR_ID != null) {
            $scope.amphurID = $scope.TeacherProfile[0].AMPHUR_ID
        }
        else {
            $scope.amphurID = 0;
        }
        if ($scope.TeacherProfile[0].SEX == 1) {
            $scope.SEX = "ชาย"
          
            $("#SEX_RADIO_BTN_1").prop("checked", true);
        }
        else {
            $scope.SEX = "หญิง"
            
            $("#SEX_RADIO_BTN_2").prop("checked", true);
        }

        $scope.ABOUT = $scope.TeacherProfile[0].ABOUT
    }

    $scope.UpdateMemberProfile = function () {
        $.LoadingOverlay("show");

        debugger;
        $http({
            url: "/teacher/UpdateMemberProfile",
            method: "GET",
            params: {
                FULLNAME: $scope.FULLNAME,
                FIRST_NAME: $scope.FIRST_NAME,
                LAST_NAME: $scope.LAST_NAME,
                MOBILE: $scope.MOBILE,
                NICKNAME: $scope.NICKNAME,
                ABOUT: $scope.ABOUT,
                LOCATION: $scope.LOCATION,
                AMPHUR_ID: $scope.amphurID,
                TEACHING_TYPE: $('#TEACHING_TYPE option:selected').val(),
                STUDENT_LEVEL: $('#STUDENT_LEVEL option:selected').val(),
                SEX_RADIO: $scope.SEX_RADIO,
                DATE_OF_BIRTH_TEXT: $('#DATE_OF_BIRTH_TEXT').val(),
                
            }
        }).then(function (response) {
            console.log(response.data.OUTPUT_DATA)
            if (response.data.STATUS == true) {
                if ($scope.ABOUT_IMG_1 != "" || $scope.ABOUT_IMG_2 != "" || $scope.ABOUT_IMG_3 != "" || $scope.ABOUT_IMG_4 != "") {
                    $('#btnUpdateSubmitImg').trigger('click')
                }
                else {
                    $scope.init();
                    $scope.HideButton();
                }


            }

        });
        $.LoadingOverlay("hide");
    }

    $scope.renderInnerHtml = function () {
        debugger;
        $('#ListCategoryArea').val("");
        $('#BadgeCategoryArea').empty();

        var html = "";
        var string = "";
        var itemsLength = Object.keys($scope.ListCourse).length;
        debugger;
        for (var i = 0; i < itemsLength; i++) {
            html += "<p>#" + $scope.ListCourse[i].NAME + "</p>";

            if (i == itemsLength - 1) {
                string += $scope.ListCourse[i].NAME
            }
            else {
                string += $scope.ListCourse[i].NAME + ",";
            }
        }
        debugger;
        $('#BadgeCategoryArea').append(html);
        $('#ListCategoryArea').val(string);


    }

    $scope.UpdateProfileCategory = function () {
        $("#ListCategoryArea").hide();
        $("#DropdownCategory").show();
        $('#modalCategory').modal('toggle');
        $scope.GetCategoryAvailable();
    }

    $scope.GetCategoryAvailable = function () {
        debugger;

        $http({
            url: "/teacher/GetListAvailableCategory",
            method: "GET",
            //params: { OrderID: $scope.orderId }
        }).then(function (response) {
            console.log(response.data.OUTPUT_DATA)
            if (response.data.STATUS == true) {
                $scope.ListCategoryAvailable = response.data.OUTPUT_DATA;
                $scope.RenderCategoryAvailable()
            }
        });
    }

    $scope.RenderCategoryAvailable = function () {


        $('#categoryCheckbox').empty();
        var html = "";
        var itemsLength = Object.keys($scope.ListCategoryAvailable).length;
        for (var i = 0; i < itemsLength; i++) {
            html += "<label class='m-checkbox'>"
            html += "<input type='checkbox' name = 'categoryList[]' id = 'categoryList[]' ng-model='chkCategory[" + i + "]' value=" + $scope.ListCategoryAvailable[i] + " > " + $scope.ListCategoryAvailable[i]
            html += "<span></span>"
            html += "</label>"
        }
        var $OnCompile = $(html).appendTo('#categoryCheckbox');
        $compile($OnCompile)($scope);
    }

    $scope.SaveMemberCategory = function () {

        $.LoadingOverlay("show");
        var cboxes = document.getElementsByName('categoryList[]');
        var len = cboxes.length;
        $scope.categoryList = []

        debugger;
        for (var i = 0; i < len; i++) {
            if (cboxes[i].checked) {
                $scope.categoryList.push(cboxes[i].value);
            }
        }

        debugger;
        $http({
            url: "/teacher/UpdateMemberCategory",
            method: "GET",
            params: { categoryList: $scope.categoryList }
        }).then(function (response) {
            console.log(response.data.OUTPUT_DATA)
            if (response.data.STATUS == true) {
                $scope.init();
                $('#modalCategory').modal('hide');
            }
            else {
                $('#modalCategoryBody').empty()
                $('#modalCategoryBody').append("<p>Ops Something wrong !! please Refresh Website</p>")
            }
        });
        $.LoadingOverlay("hide");
    }


    $scope.aboutImg1_file = function () {
        debugger;
        var f = document.getElementById('btnAboutImg_1').files[0],
            r = new FileReader();
        r.onloadend = function (e) {
            $scope.ABOUT_IMG_1 = e.target.result.toString().split(",")[1];
        }
        r.readAsDataURL(f);
        debugger;
    };

    $scope.aboutImg2_file = function () {
        debugger;
        var f = document.getElementById('btnAboutImg_2').files[0],
            r = new FileReader();
        r.onloadend = function (e) {
            $scope.ABOUT_IMG_2 = e.target.result.toString().split(",")[1];
        }
        r.readAsDataURL(f);
        debugger;
    };

    $scope.aboutImg3_file = function () {
        debugger;
        var f = document.getElementById('btnAboutImg_3').files[0],
            r = new FileReader();
        r.onloadend = function (e) {
            $scope.ABOUT_IMG_3 = e.target.result.toString().split(",")[1];
        }
        r.readAsDataURL(f);
        debugger;
    };

    $scope.aboutImg4_file = function () {
        debugger;
        var f = document.getElementById('btnAboutImg_4').files[0],
            r = new FileReader();
        r.onloadend = function (e) {
            $scope.ABOUT_IMG_4 = e.target.result.toString().split(",")[1];
        }
        r.readAsDataURL(f);
        debugger;
    };



});




