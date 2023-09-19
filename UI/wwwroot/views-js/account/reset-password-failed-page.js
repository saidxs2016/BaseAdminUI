import { CloseInitLoader } from './../shared/site.js';

(async ($) => {

    "use strict";

    const app = Vue.createApp({
        data() {
            return {
                result: {
                    isSuccess: false,
                    message: "",

                }
            }
        },
        async mounted() {
            this.result.isSuccess = Result.isSuccess == "true" ? true : false;
            this.result.message = Result.message ?? '';
            CloseInitLoader();
        },

        methods: {

        }
    }).mount("#app");
})(jQuery).catch(err => {
    console.error("err ::: ", err);
});
