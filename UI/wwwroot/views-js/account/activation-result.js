import { Wait, CloseInitLoader } from './../shared/site.js';

(async ($) => {
    "use strict";
    const app = Vue.createApp({
        data() {
            return {
                result: {
                    isSuccess: false,
                    message: "",

                },
                typeObj: {
                    url: "",
                    text: ""
                }
            }
        },
        async mounted() {
            await Wait(500);
            this.result.isSuccess = Result.isSuccess == 'true' ? true : false;
            this.result.message = Result.message ?? '';
            if (!this.result.isSuccess) {
                this.typeObj.url = "/Register";
                this.typeObj.text = "Tekrar Kayıt Olunuz!";
            } else {
                this.typeObj.url = "/Account/Login";
                this.typeObj.text = "Giriş Yapınız!";
            }

            CloseInitLoader();
        },

        methods: {

        }
    }).mount("#app");
})(jQuery).catch(err => {
    console.error("err ::: ", err);
});