import { CloseInitLoader, Loader, AxiosPostRequest, CloseLoader } from './../shared/site.js';

(async ($) => {

    "use strict";

    CloseInitLoader();
    $('#login-form').submit(async (e) => {
        e.preventDefault();
        Loader();

        const form = $('#login-form');
        const form_data = new FormData(form[0]);

        const request_model = {
            username: form_data.get('Username'),
            password: form_data.get('Password'),
        };

        const url = "/Account/Login";
        const data = request_model;
        try {
            const result = await AxiosPostRequest(url, data);
            toastr["success"](result?.message);
            //await Wait(500);
            location.href = result.redirect;

            // eğer kod buraya kadar gelirse isSuccess true olur
        }
        catch (err) {
            //console.log("errr", err);
            toastr["warning"](err?.message);
            //location.href = `/ErrorPage/Error?details=${JSON.stringify(err)}`; 

        }
        finally { CloseLoader(); }



    });
})(jQuery).catch(err => {
    console.error("err ::: ", err);
});


