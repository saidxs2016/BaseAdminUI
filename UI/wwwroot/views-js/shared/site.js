"use strict";

let llocation = null, error_msgs = [];
// ============ Wait == Delay in js ============
export const Wait = (offset) => new Promise((res, rej) => setTimeout(() => { res(true); }, offset));

// ============ Axios Kullanarak Get istegi atma ============
export const AxiosGetRequest = (url, config) => new Promise(async (resolve, reject) => {


    try {
        if (!url || url.length == 0)
            throw new Error("url is empty.");

        if (!config)
            config = {
                headers: {
                    'Content-Type': 'application/json; charset=UTF-8',
                    'RequestVerificationToken': VerifyToken
                }
            };
        if (config && !config.headers)
            config.headers = {
                'Content-Type': 'application/json; charset=UTF-8',
                'RequestVerificationToken': VerifyToken
            };
        config.headers.location = llocation;
        const result = await axios.get(url, config);
        if (result.status == 200 && result.data && result.data.isSuccess)
            resolve(result.data);

        else {
            let msg = "";
            if (result.data && typeof result.data == "string")
                msg = result.data;
            if (result.data && typeof result.data == "object")
                msg = result.data?.message;
            throw new Error(msg);
        }

    }
    catch (err) {
        let message = err?.response?.data?.message && err.response.data.message.length > 0 ? err.response.data.message : err?.message;
        error_msgs.push(message);
        let error_details = {
            message: message,
            status: err?.response?.status ?? err?.request?.status,
        };
        if (error_details.status == 401)
            location.href = "/errorpage/error?status=401";
        reject(error_details);

    }
    finally { }
});

// ============ Axios Kullanarak Post istegi atma ============
export const AxiosPostRequest = (url, data, config) => new Promise(async (resolve, reject) => {
    try {
        if (!url || url.length == 0)
            throw new Error("url is empty.");
        if (!config)
            config = {
                headers: {
                    'Content-Type': 'application/json; charset=UTF-8',
                    'RequestVerificationToken': VerifyToken
                }
            };
        if (config && !config.headers)
            config.headers = {
                'Content-Type': 'application/json; charset=UTF-8',
                'RequestVerificationToken': VerifyToken
            };
        config.headers.location = llocation;

        const result = await axios.post(url, data, config);
        if (result.status == 200 && result.data && result.data.isSuccess)
            resolve(result.data);

        else {
            let msg = "";
            if (result.data && typeof result.data == "string")
                msg = result.data;
            if (result.data && typeof result.data == "object")
                msg = result.data?.message;
            throw new Error(msg);
        }

    }
    catch (err) {
        let message = err?.response?.data?.message && err.response.data.message.length > 0 ? err.response.data.message : err?.message;
        error_msgs.push(message);
        let error_details = {
            message: message,
            status: err?.response?.status ?? err?.request?.status,
        };
        if (error_details.status == 401)
            location.href = "/errorpage/error?status=401";
        reject(error_details);

    }
    finally { }
});

// ============ Axios Kullanarak Delete istegi atma ============
export const AxiosDeleteRequest = (url, data, config) => new Promise(async (resolve, reject) => {
    try {
        if (!url || url.length == 0)
            throw new Error("url is empty.");

        if (!config)
            config = {
                headers: {
                    'Content-Type': 'application/json; charset=UTF-8',
                    'RequestVerificationToken': VerifyToken
                }
            };
        if (config && !config.headers)
            config.headers = {
                'Content-Type': 'application/json; charset=UTF-8',
                'RequestVerificationToken': VerifyToken
            }
        config.headers.location = llocation;
        const result = await axios.delete(url, data, config);
        if (result.status == 200 && result.data && result.data.isSuccess)
            resolve(result.data);

        else {
            let msg = "";
            if (result.data && typeof result.data == "string")
                msg = result.data;
            if (result.data && typeof result.data == "object")
                msg = result.data?.message;
            throw new Error(msg);
        }

    }
    catch (err) {
        let message = err?.response?.data?.message && err.response.data.message.length > 0 ? err.response.data.message : err?.message;
        error_msgs.push(message);
        let error_details = {
            message: message,
            status: err?.response?.status ?? err?.request?.status,
        };
        if (error_details.status == 401)
            location.href = "/errorpage/error?status=401";
        reject(error_details);

    }
    finally { }
});

// ============ Axios Kullanarak istegi atma ============
export const AxiosRequest = (config) => new Promise(async (resolve, reject) => {


    try {

        if (!config)
            throw new Error("config is empty.");
        if (!config.url)
            throw new Error("url is empty.");
        if (!config.type)
            throw new Error("type is empty.");
        config.headers.location = llocation;
        const result = await axios(config);
        if (result.status == 200 && result.data && result.data.isSuccess)
            resolve(result.data);

        else {
            let msg = "";
            if (result.data && typeof result.data == "string")
                msg = result.data;
            if (result.data && typeof result.data == "object")
                msg = result.data?.message;
            throw new Error(msg);
        }

    }
    catch (err) {
        let message = err?.response?.data?.message && err.response.data.message.length > 0 ? err.response.data.message : err?.message;
        error_msgs.push(message);
        let error_details = {
            message: message,
            status: err?.response?.status ?? err?.request?.status,
        };
        if (error_details.status == 401)
            location.href = "/errorpage/error?status=401";
        reject(error_details);

    }
    finally { }
});

// ============ Ip Bilgilerini almak için ============
export const GetIpInfo = () => new Promise(async (resolve, reject) => {
    try {
        const LocResponse = await fetch("https://ipinfo.io/json?token=b7fab699aef627");
        const info = await Response.json();
        resolve(info);
    }
    catch (err) {
        reject(err);
    }
    finally { }
});

// ============ Konum Bilgilerini almak için ============
(() => {
    try {
        const GetPositionCoords = item => {
            llocation = `${item?.coords.latitude},${item?.coords.longitude}`;
        };
        const ErrorCoords = err => { };
        const Options = { enableHighAccuracy: true, timeout: 10000 };
        navigator.geolocation.getCurrentPosition(GetPositionCoords, ErrorCoords, Options);
    }
    catch (err) { } finally { }
})();


// ============ NewGuid ============
export const NewGuid = () => { // generete guid code == e7()
    let lut = []; for (var i = 0; i < 256; i++) { lut[i] = (i < 16 ? '0' : '') + (i).toString(16); }
    let d0 = Math.random() * 0xffffffff | 0;
    let d1 = Math.random() * 0xffffffff | 0;
    let d2 = Math.random() * 0xffffffff | 0;
    let d3 = Math.random() * 0xffffffff | 0;
    return lut[d0 & 0xff] + lut[d0 >> 8 & 0xff] + lut[d0 >> 16 & 0xff] + lut[d0 >> 24 & 0xff] + '-' +
        lut[d1 & 0xff] + lut[d1 >> 8 & 0xff] + '-' + lut[d1 >> 16 & 0x0f | 0x40] + lut[d1 >> 24 & 0xff] + '-' +
        lut[d2 & 0x3f | 0x80] + lut[d2 >> 8 & 0xff] + '-' + lut[d2 >> 16 & 0xff] + lut[d2 >> 24 & 0xff] +
        lut[d3 & 0xff] + lut[d3 >> 8 & 0xff] + lut[d3 >> 16 & 0xff] + lut[d3 >> 24 & 0xff];
}
export const EmptyGuid = () => "00000000-0000-0000-0000-000000000000";

// ============ Block Any UI Component ============
export const BlockUI = (selector) => {
    if (selector)
        $(selector).block({
            message: '<i class="icon-spinner2 spinner"></i>',
            overlayCSS: {
                backgroundColor: '#fff',
                opacity: 0.8,
                cursor: 'wait',
                'box-shadow': '0 0 0 1px #ddd'
            },
            css: {
                border: 0,
                padding: 0,
                backgroundColor: 'none'
            }
        });
}
export const UnBlockUI = (selector) => {
    if (selector)
        $(selector).unblock();
}

// ============ Loader ============
export const Loader = () => {
    // ============ <Start> Loading  ===========
    Swal.fire({
        title: 'Lütfen Bekleyiniz...',
        allowOutsideClick: false,
        showConfirmButton: false,
        didOpen: () => {
            Swal.showLoading()
        }
    });
    // ============ </End> Loading  ==========
}
export const CloseLoader = () => Swal.close();

// ============ Init Loader ============
export const InitLoader = () => document.querySelector('#load_screen').style.display = 'block';
export const CloseInitLoader = () => {
    let tmp_loader = document.querySelector('#load_screen');
    if (tmp_loader) tmp_loader.style.display = 'none';
}


// ============ BreadCrumb ============
export const BreadCrumb = async () => { /// sayfada istemediğimiz alanları role yetkilendirmesinden, veritabanından taglare göre görünmemesini sağlayabiliyoruz
    try {
        const url = "/Shared/BreadCrumbRequest";
        const data = {};
        const result = await AxiosPostRequest(url, data);
        result.data ??= [];

        let bread_crumb_html = `<a href="/" class="breadcrumb-item"><i class="icon-home2 me-2"></i></a>`;
        if (result.data.length > 0)
            result.data[result.data.length - 1].slug = null;

        result.data.forEach(b => {

            if (b.slug && b.slug.length > 0)
                bread_crumb_html += `<a href="${b.slug}" class="breadcrumb-item">${b.text}</a>`;
            else
                bread_crumb_html += ` <span class="breadcrumb-item breadcrumb-item-text">${b.text}</span>`;

        });
        $(".breadcrumb").html(bread_crumb_html);
    } catch (err) { } finally { }


}

// ============ Set li element as Active ============
export const ActivedLiElement = async () => {
    let page_path = "/admin/index";
    if (location.pathname == "/" || location.pathname.toLowerCase() == "/admin/index") {
        let parents_for_set_active = $('#sidebar a[data-url="/profile/index"][data-category="true"]').parentsUntil('ul.menu-categories');
        parents_for_set_active.addClass("active");
        localStorage.setItem("actived_ele", "/profile/index");
    }
    else {
        let page_path_arr = location.pathname.split("/");
        page_path_arr = page_path_arr.slice(1, page_path_arr.length);
        if (page_path_arr)
            page_path = "/" + page_path_arr[0] + "/" + page_path_arr[1];

        const targets_a = document.querySelectorAll(`#sidebar a[data-url^="${page_path}"]`);
        if (targets_a && targets_a.length > 0) {
            localStorage.setItem("actived_ele", page_path);
            targets_a.forEach(item => {
                let absoluteParent = $(item).closest("li.menu");

                let firschild = absoluteParent.children().first();
                firschild.attr('aria-expanded', true);
                let ul = absoluteParent.children().last();
                ul.addClass("show");

                let parents_for_set_active = $(item).parentsUntil('li.menu');
                parents_for_set_active.addClass("active");
                let parents_for_set_active_serialize = [...parents_for_set_active];
                parents_for_set_active_serialize.forEach(w => {

                    let firschild1 = $(w).children().first();
                    if (firschild1[0].localName == "a") {
                        firschild1.attr('aria-expanded', true);
                    }
                    if ($(w)[0].localName == "ul") {
                        $(w).addClass("show");
                    }

                });
            });
        }
        else {
            let url_util = new URL(document.referrer);
            page_path_arr = url_util.pathname.split("/");
            page_path_arr = page_path_arr.slice(1, page_path_arr.length);
            if (page_path_arr)
                page_path = "/" + page_path_arr[0] + "/" + page_path_arr[1];

            const old_targets_a = document.querySelectorAll(`#sidebar a[data-url^="${page_path}"]`);
            if (old_targets_a && old_targets_a.length > 0) {
                localStorage.setItem("actived_ele", page_path);
                old_targets_a.forEach(item => {
                    let absoluteParent = $(item).closest("li.menu");

                    let firschild = absoluteParent.children().first();
                    firschild.attr('aria-expanded', true);
                    let ul = absoluteParent.children().last();
                    ul.addClass("show");

                    let parents_for_set_active = $(item).parentsUntil('li.menu');
                    parents_for_set_active.addClass("active");
                    let parents_for_set_active_serialize = [...parents_for_set_active];
                    parents_for_set_active_serialize.forEach(w => {

                        let firschild1 = $(w).children().first();
                        if (firschild1[0].localName == "a") {
                            firschild1.attr('aria-expanded', true);
                        }
                        if ($(w)[0].localName == "ul") {
                            $(w).addClass("show");
                        }

                    });
                });
            }
            else {

                const storage_path = localStorage.getItem("actived_ele");
                const old_targets_a = document.querySelectorAll(`#sidebar a[data-url^="${storage_path}"]`);
                if (old_targets_a && old_targets_a.length > 0) {
                    old_targets_a.forEach(item => {
                        let absoluteParent = $(item).closest("li.menu");

                        let firschild = absoluteParent.children().first();
                        firschild.attr('aria-expanded', true);
                        let ul = absoluteParent.children().last();
                        ul.addClass("show");

                        let parents_for_set_active = $(item).parentsUntil('li.menu');
                        parents_for_set_active.addClass("active");
                        let parents_for_set_active_serialize = [...parents_for_set_active];
                        parents_for_set_active_serialize.forEach(w => {

                            let firschild1 = $(w).children().first();
                            if (firschild1[0].localName == "a") {
                                firschild1.attr('aria-expanded', true);
                            }
                            if ($(w)[0].localName == "ul") {
                                $(w).addClass("show");
                            }

                        });
                    });
                }


            }

        }
    }
}

// ============ HideIgnoredSections ============ 
export const HideIgnoredSections = async () => { /// sayfada istemediğimiz alanları role yetkilendirmesinden, veritabanından taglare göre görünmemesini sağlayabiliyoruz    
    try {
        const url = "/Shared/HideIgnoreSectionRequest";
        const data = {};
        const result = await AxiosPostRequest(url, data);
        result.data ??= [];

        result.data.forEach(i => { $(i)?.remove(); });
        result.data = null;

    } catch (err) { } finally { }
}



