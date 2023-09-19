import { CloseInitLoader, Loader, AxiosPostRequest, Wait, CloseLoader } from './../shared/site.js';

(async ($) => {

    "use strict";
    const app = Vue.createApp({
        data() {
            return {
                newPassword: {
                    uid: null,
                    password: "",
                    rePassword: "",
                }
            }
        },
        async mounted() {


            CloseInitLoader();
        },

        methods: {
            async resetPassword() {
                if (app.newPassword.password != app.newPassword.rePassword) {
                    toastr["warning"]("Şifreler eşleşmiyor!");
                    return;
                }

                Loader();

                const url = "/ResetPasswordPage";
                let data = {
                    token: Token,
                    password: app.newPassword.password,
                    rePassword: app.newPassword.rePassword,
                };

                let res = null;
                try {
                    res = await AxiosPostRequest(url, data);

                    toastr["success"](res.message);
                    await Wait(2000);
                    location.href = "/";
                }
                catch (err) {
                    toastr["warning"](err.message);
                }
                finally {
                    CloseLoader();
                }
            },
        }
    }).mount("#app");

    
})(jQuery).catch(err => {
    console.error("err ::: ", err);
});
