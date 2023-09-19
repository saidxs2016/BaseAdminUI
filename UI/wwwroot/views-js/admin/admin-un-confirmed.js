import { CloseInitLoader, Loader, EmptyGuid, AxiosPostRequest, Wait, CloseLoader } from './../shared/site.js';

(async ($) => {

    "use strict";

    const app = Vue.createApp({
        data() {
            return {
                admins: []
            }
        },
        async mounted() {
            try { 
                const url = `/Admin/UnConfirmedFeature`;
                const data = {};

                let result = await AxiosPostRequest(url, data);
                app.admins = result.data;

            } catch (e) {
                toastr["warning"](e.message);
            }
            finally {
                CloseInitLoader();
            }

        },

        methods: {
            dateFormatFull(date) {
                let str = "";
                if (date) {
                    str = moment(date).format('DD-MM-YYYY HH:mm:ss');
                }
                return str;
            },

        }
    }).mount("#app");

})(jQuery).catch(err => {
    console.error("Err ::: ", err);
});


