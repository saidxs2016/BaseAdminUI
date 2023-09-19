import { CloseInitLoader, Loader, EmptyGuid, BlockUI, UnBlockUI, AxiosPostRequest, Wait, CloseLoader } from './../shared/site.js';

(async ($) => {

    "use strict";

    const app = Vue.createApp({
        data() {
            return {
                profile: {},
                tempProfile: {},
                newPassword: {
                    oldPassword: "",
                    password: "",
                    rePassword: "",
                },
                connectionKeys: [],
                connectionKey: {
                    password: null,
                    description: ""
                }
            }
        },
        async mounted() {
            try { 
                const url = `/Profile/GetProfileFeature`;
                const data = {};

                let result = await AxiosPostRequest(url, data);
                this.profile = result.data.admin;
                this.connectionKeys = JSON.parse(result.data.admin?.connectionKeys ?? "[]");


              
            } catch (e) {
                toastr["warning"](e.message);
            }
            finally {
                CloseInitLoader();
            }


            

        },

        methods: {
            copyDoiToClipboard(doi) {
                navigator.clipboard.writeText(doi);

                toastr["success"]("Kopyalandı!");
            },
            dateFormatFull(date) {
                let str = "";
                if (date) {
                    str = moment(date).format('DD-MM-YYYY HH:mm:ss');
                }
                return str;
            },
            enableConnectionKeyDelete(dateStr) {
                const date = new Date(dateStr);
                const today = new Date();

                const oneDayInMilliseconds = 1000 * 60 * 60 * 24;
                const differenceInTime = today.getTime() - date.getTime();

                return differenceInTime <= oneDayInMilliseconds;
            },
            enableConnectionKeyDelete1(validDateStr) {
                let validDate = new Date(validDateStr);
                let now = new Date();
                if (validDate && (now <= validDate))
                    return false;
                return true;
            },

            openPasswordChangeModal() {
                app.newPassword = {
                    oldPassword: "",
                    password: "",
                    rePassword: ""
                };
                $('#password-change-modal').modal('show');
            },
            async passwordChange() {
                if (app.newPassword.password != app.newPassword.rePassword) {
                    toastr["warning"]("Şifreler eşleşmiyor!");
                    return;
                }

                Loader();

                const url = "/Profile/PasswordChangeFeature";
                let data = {
                    oldPassword: app.newPassword.oldPassword,
                    password: app.newPassword.password,
                    rePassword: app.newPassword.rePassword,
                };

                let res = null;
                try {
                    res = await AxiosPostRequest(url, data);

                    $('#password-change-modal').modal('hide');
                    toastr["success"](res.message);
                }
                catch (err) {
                    toastr["warning"](err.message);
                }
                finally {
                    CloseLoader();
                }
            },
            openUpdateProfileModal() {
                app.tempProfile = { ...app.profile };
                $('#profile-update-modal').modal('show');
            },
            async updateProfile() {
                Loader();


                const url = "/Profile/UpdateFeature";
                let data = {
                    admin: app.tempProfile
                };
                let res = null;
                try {
                    res = await AxiosPostRequest(url, data);
                    app.profile = res.data.admin;
                    toastr["success"](res.message);
                    $('#profile-update-modal').modal('hide');
                }
                catch (err) {
                    toastr["warning"](err.message);

                    //location.href = `/ErrorPage/Error?details=${JSON.stringify(err) }`;
                }
                finally {
                    CloseLoader();
                }
            },
            openConnectionKeyModal() {
                app.connectionKey.password = null;
                app.connectionKey.description = null;
                $('#connection-key-modal').modal('show');
            },
            async generateConnectionKey() {
                Loader();

                const url = "/Profile/GenerateConnectionKeyFeature";
                let data = {
                    password: app.connectionKey.password,
                    description: app.connectionKey.description
                };

                let res = null;
                try {
                    res = await AxiosPostRequest(url, data);
                    app.connectionKeys = JSON.parse(res.data.admin?.connectionKeys);
                    $('#connection-key-modal').modal('hide');
                    toastr["success"](res.message);
                }
                catch (err) {
                    toastr["warning"](err.message);
                }
                finally {
                    CloseLoader();
                }
            },
            connectionKeyDelete(key, validTo) {
                if (app.enableConnectionKeyDelete(validTo) == true) {
                    toastr["warning"]("Oluşturma Zamanından 1 Gün Sonra Silme İşlemi Yapabilirsiniz.");
                    return;
                }

                const swal = Swal.mixin({
                    customClass: {
                        confirmButton: 'btn btn-success',
                        cancelButton: 'btn btn-danger'
                    },
                    buttonsStyling: false
                })
                swal.fire({
                    title: 'Emin misiniz?',
                    text: "Dana önce oluşturduğunuz bağlantı anahtarını silmek istiyor musunuz?",
                    type: 'warning',
                    showCancelButton: true,
                    confirmButtonText: 'Evet',
                    cancelButtonText: 'Hayır',
                    reverseButtons: true
                }).then(async (result) => {
                    if (result.value) {


                        Loader();

                        const url = "/Profile/ConnectionKeyDeleteFeature";
                        let data = {
                            key: key
                        };


                        let res = null;
                        try {
                            res = await AxiosPostRequest(url, data);

                            app.connectionKeys = JSON.parse(res.data.admin?.connectionKeys ?? "[]");

                            toastr["success"](res.message);
                        }
                        catch (err) {
                            toastr["warning"](err.message);
                        }
                        finally {
                            CloseLoader();
                        }


                    } else if (result.dismiss === Swal.DismissReason.cancel) {
                        swal.close();
                    }
                })
            }
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