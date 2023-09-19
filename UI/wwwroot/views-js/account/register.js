import { CloseInitLoader, Loader, Wait, AxiosPostRequest, CloseLoader } from './../shared/site.js';

(async ($) => {

    "use strict";

    const app = Vue.createApp({
        data() {
            return {
                termsAndConditions: false,
                password: "",
                rePassword: ""
            }
        },

        async mounted() {
            await Wait(500);
            CloseInitLoader();
        },
        methods: {

        }
    }).mount("#app");


    CloseInitLoader();
    $('#register-form').submit(async (e) => {
        e.preventDefault();
        Loader();
        const form = $('#register-form');
        const form_data = new FormData(form[0]);

        debugger;

        const request_model = {
            username: form_data.get('Username'),
            password: form_data.get('Password'),
            rePassword: form_data.get('RePassword'),
            email: form_data.get('Email'),
            name: form_data.get('Name'),
            surname: form_data.get('Surname'),
            phone: form_data.get('Phone'),
            termsAndConditions: form_data.get('TermsAndConditions') == "on" ? true : false,

        };

        const url = "/Register";
        const data = request_model;
        try {
            const result = await AxiosPostRequest(url, data);
            toastr["success"](result?.message);
        }
        catch (err) {
            //console.log("errr", err);
            toastr["warning"](err?.message);
            //location.href = `/ErrorPage/Error?details=${JSON.stringify(err)}`; 

        }
        finally { CloseLoader(); }
    });


    const phoneInput = $("#phone");
    phoneInput.inputmask("0 (999) 999 99 99");
    phoneInput.on("keyup", () => {
        const event = new CustomEvent('input', { bubbles: true });
        phoneInput[0].dispatchEvent(event);
    });


  
})(jQuery).catch(err => {
    console.error("err ::: ", err);
});
