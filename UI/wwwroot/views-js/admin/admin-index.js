import { CloseInitLoader, Wait } from './../shared/site.js';
import { FirstClient } from './../shared/signalr/first-client.js';




(async ($) => {

    "use strict";


    const app = Vue.createApp({
        data() {
            return {
            }
        },
        async mounted() {


            CloseInitLoader();
            $('#signalr-btn').click(async e => {
                try {

                    const s1 = performance.now();

                    const connection = await FirstClient();
                    const x = await connection.invoke("TestData", { message: "Merhaba Dünya" });
                    const s2 = performance.now();

                    console.log("x ::: ", x, " ---- ", s2 - s1);


                } catch (err) {
                    alert("err ::: " + JSON.stringify(err));
                    console.log("admin-index err :::: ", err);
                }
            });

            $('#rate-limit-btn').click(async e => {
                try {

                    const result = await axios.get("/Home/Test");
                    console.log("x ::: ", result);
                } catch (err) {
                    alert("err ::: " + JSON.stringify(err));
                    console.log("admin-index err :::: ", err);
                }
            });


        },

        methods: {
            async SubmitForm() {

            },
        }
    }).mount("#app");

})(jQuery).catch(err => {
    console.error("Err ::: ", err);
});



