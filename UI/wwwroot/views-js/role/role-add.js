import { CloseInitLoader, Loader, EmptyGuid, BlockUI, UnBlockUI, AxiosPostRequest, Wait, CloseLoader } from './../shared/site.js';

(async ($) => {

    "use strict";

    const app = Vue.createApp({
        data() {
            return {
                role: {
                    name: "",
                    route: ""
                },
                roleEmpty: {
                    name: "",
                    route: ""
                }
            }
        },
        async mounted() { 
            CloseInitLoader();
        },

        methods: {
            async AddForm() {
                Loader();


                const url = "/Role/AddFeature";
                let data = {
                    role: app.role
                };               
                let res = null;
                try {
                    res = await AxiosPostRequest(url, data);

                    toastr["success"](res.message);
                    await Wait(1500);
                    location.href = `/role/index`;
                }
                catch (err) {
                    toastr["warning"](err.message);

                    //location.href = `/ErrorPage/Error?details=${JSON.stringify(err) }`;
                }
                finally {
                    CloseLoader();
                }
            },
        }
    }).mount("#app");
})(jQuery).catch(err => {
    console.error("Err ::: ", err);
});





