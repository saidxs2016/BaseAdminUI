import { CloseInitLoader, Loader, CloseLoader, AxiosPostRequest } from './../shared/site.js';

(async ($) => {
    "use strict";


    CloseInitLoader();
    $('#forget-password-form').submit(async (e) => {
        e.preventDefault();
        Loader();
        const form = $('#forget-password-form');
        const form_data = new FormData(form[0]);

        const request_model = {
            email: form_data.get('Email'),
        };

        const url = "/ForgetPassword";
        const data = request_model;
        try {
            const result = await AxiosPostRequest(url, data);
            toastr["success"](result?.message);
            //await Wait(1500);
            //location.href = "/account/login"; 
        }
        catch (err) {
            toastr["warning"](err?.message);
        }
        finally { CloseLoader(); }



    });


})(jQuery).catch(err => {
    console.error("err ::: ", err);
});