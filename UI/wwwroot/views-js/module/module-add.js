import { CloseInitLoader, Loader, EmptyGuid, AxiosPostRequest, Wait, CloseLoader } from './../shared/site.js';

(async ($) => {

    "use strict";

    const app = Vue.createApp({
        data() {
            return {
                module: {
                    name: "",
                    controller: "",
                    action: "",
                    address: "",
                    icon: "",
                    parentUid: EmptyGuid(),
                    isMenu: false,
                    sectionId: ""
                },
                moduleEmpty: {
                    name: "",
                    controller: "",
                    action: "",
                    address: "",
                    icon: "",
                    parentUid: EmptyGuid(),
                    isMenu: false,
                    sectionId: ""
                },
                parentModules: [],
                modulesKeys: []


            }
        },
        async mounted() {

            this.modulesKeys = ModulesKeys ?? [];

            // ========== Init Search Icon ==========
            InitIconCollection();

            CloseInitLoader();
        },

        methods: {
            async AddForm() {
                Loader();
                app.module.parentUid = ParentUid;

                const url = "/Module/AddFeature";
                let data = {
                    module: app.module
                };

                let copyData = { ...data };

                let res = null;
                try {
                    res = await AxiosPostRequest(url, copyData);

                    toastr["success"](res.message);
                    await Wait(1500);
                    if (!app.module.parentUid || app.module.parentUid == EmptyGuid())
                        location.href = `/module/index/`;
                    else
                        location.href = `/module/index/${app.module.parentUid}`;

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





