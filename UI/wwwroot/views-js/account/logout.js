import { Loader, AxiosPostRequest, CloseLoader } from './../shared/site.js';

(async ($) => {

    "use strict";

    //CloseInitLoader();
    $('#logout-id').click(async (e) => {
        e.preventDefault();
        Loader();

        const request_model = {};

        const url = "/Account/Logout";
        const data = request_model;
        try {
            const result = await AxiosPostRequest(url, data);
            location.href = "/";
        }
        catch (err) {
            //console.log("errr", err);
            //toastr["warning"](err?.message);
            location.href = `/ErrorPage/Error?details=${JSON.stringify(err)}`;
        }
        finally { CloseLoader(); }
    });




})(jQuery).catch(err => {
    console.error("err ::: ", err);
});
