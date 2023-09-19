import { CloseInitLoader, Loader, EmptyGuid, AxiosPostRequest, Wait, CloseLoader } from './../shared/site.js';

(async ($) => {

    "use strict";

    const app = Vue.createApp({
        data() {
            return {
                admin: {
                    username: "",
                    password: "",
                    email: "",
                    name: "",
                    surname: "",
                    title: "",
                    phone: "",
                    roleUid: EmptyGuid()
                },
                roles: []
            }
        },

        async mounted() {
            try {
                //    this.roles.push({ name: "Rol Seçiniz", uid: EmptyGuid() });

                //    let url = "/Admin/GetRoles";
                //    let data = {};

                //    let result = await AxiosPostRequest(url, data, VerifyToken)
                //    if (result.data?.roleList)
                //        this.roles.push(...result.data);

            } catch (err) {
                toastr["warning"](err.message);
                //location.href = `/ErrorPage/Error?details=${JSON.stringify(err) }`;
            }

            if (ParentUid) {
                CloseInitLoader();
            }
        },
        methods: {
            adminClear() {
                app.admin = {
                    username: "",
                    password: "",
                    email: "",
                    name: "",
                    surname: "",
                    title: "",
                    phone: "",
                    roleUid: EmptyGuid()
                };
            },
            async SubmitForm() {
                Loader();


                app.admin.roleUid = ParentUid != "" ? ParentUid : null;


                const url = "/Admin/AddFeature";
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
                    location.href = `/admin/sub/${ParentUid}`;
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

    const phoneInput = $("#phone");
    phoneInput.inputmask("0 (999) 999 99 99");
    phoneInput.on("keyup", () => {
        const event = new CustomEvent('input', { bubbles: true });
        phoneInput[0].dispatchEvent(event);
    });
})(jQuery).catch(err => {
    console.error("Err ::: ", err);
});






