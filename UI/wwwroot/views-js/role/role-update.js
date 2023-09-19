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
            try { 
                let url = "/Role/GetRoleFeature";
                let data = { uid: RoleUid };

                let result = await AxiosPostRequest(url, data)
                app.role = result.data?.role;
            } catch (err) {
                toastr["warning"](err.message);
                //location.href = `/ErrorPage/Error?details=${JSON.stringify(err) }`;
            }

            CloseInitLoader();
        },

        methods: {
            async UpdateForm() {
                Loader();


                const url = "/Role/UpdateFeature";
                let data = {
                    role: app.role
                };
                let res;
                try {
                    res = await AxiosPostRequest(url, data);
                    app.role = app.roleEmpty;

                    toastr["success"](res.message);
                    await Wait(1500);
                    location.href = `/Role/Index`;
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






