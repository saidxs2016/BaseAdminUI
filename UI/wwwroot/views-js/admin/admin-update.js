import { CloseInitLoader, Loader, EmptyGuid, AxiosPostRequest, Wait, CloseLoader } from './../shared/site.js';

(async ($) => {

    "use strict";

    const app = Vue.createApp({
        data() {
            return {
                admin: {
                    username: "",
                    email: "",
                    name: "",
                    surname: "",
                    title: "",
                    phone: ""
                }
            }
        },

        async mounted() {
            try { 

                let url = "/Admin/GetAdminFeature";
                let data = { Uid };

                let result = await AxiosPostRequest(url, data)
                this.admin = result.data.admin;

            } catch (err) {
                toastr["warning"](err.message);
            }


            if (Uid) {
                CloseInitLoader();
            }
        },
        methods: {
            adminClear() {
                app.admin = {
                    username: "",
                    email: "",
                    name: "",
                    surname: "",
                    title: "",
                    phone: ""
                };
            },
            async SubmitForm() {
                Loader();


                const url = "/Admin/UpdateFeature";
                let data = {
                    admin: this.admin
                };
                let res = null;
                try {
                    res = await AxiosPostRequest(url, data);
                    toastr["success"](res.message);
                    //$('form')[0].reset();
                    app.adminClear();
                    await Wait(1500);
                    location.href = `/admin/sub/${res.data.role.uid}`;
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

    const phoneInput = $("#phone");
    phoneInput.inputmask("0 (999) 999 99 99");
    phoneInput.on("keyup", () => {
        const event = new CustomEvent('input', { bubbles: true });
        phoneInput[0].dispatchEvent(event);
    });
})(jQuery).catch(err => {
    console.error("Err ::: ", err);
});






