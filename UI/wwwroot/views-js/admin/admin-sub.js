import { CloseInitLoader, Loader, EmptyGuid, AxiosPostRequest, Wait, CloseLoader } from './../shared/site.js';

(async ($) => {

    "use strict";

    const app = Vue.createApp({
        data() {
            return {
                admins: [],
                newPassword: {
                    uid: null,
                    password: "",
                    rePassword: "",
                }

            }
        },
        async mounted() {
            try { 
                const url = `/admin/subFeature`;
                const data = { uid: Uid };

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
            openPasswordChangeModal(uid) {
                if (!uid || uid == EmptyGuid())
                    return;
                app.newPassword = {
                    uid: null,
                    password: "",
                    rePassword: ""
                };
                app.newPassword.uid = uid;
                $('#password-change-modal').modal('show');
            },
            async passwordChange() {
                if (app.newPassword.password != app.newPassword.rePassword) {
                    toastr["warning"]("Şifreler eşleşmiyor!");
                    return;
                }

                Loader();

                const url = "/Admin/PasswordChangeFeature";
                let data = {
                    uid: app.newPassword.uid,
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
            adminDelete(uid) {
                const swal = Swal.mixin({
                    customClass: {
                        confirmButton: 'btn btn-success',
                        cancelButton: 'btn btn-danger'
                    },
                    buttonsStyling: false
                })
                swal.fire({
                    title: 'Emin misiniz?',
                    text: "Bu işlemin geri dönüşü yoktur!",
                    type: 'warning',
                    showCancelButton: true,
                    confirmButtonText: 'Evet',
                    cancelButtonText: 'Hayır',
                    reverseButtons: true
                }).then(async (result) => {
                    if (result.value) {



                        Loader();

                        const url = "/Admin/DeleteFeature";
                        let data = {
                            uid: uid
                        };


                        let res = null;
                        try {
                            res = await AxiosPostRequest(url, data);

                            let index = app.modules.findIndex(w => w.uid == uid);
                            app.modules.splice(index, 1);
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
            },
            suspendToogle(uid) {
                if (!uid)
                    return;
                let admin = app.admins.find(w => w.uid == uid);
                const swal = Swal.mixin({
                    customClass: {
                        confirmButton: 'btn btn-success',
                        cancelButton: 'btn btn-danger'
                    },
                    buttonsStyling: false
                })
                swal.fire({
                    title: 'Emin misiniz?',
                    text: admin.isSuspend ? 'Askıdan Al' : 'Askıya Al',
                    type: 'warning',
                    showCancelButton: true,
                    confirmButtonText: 'Evet',
                    cancelButtonText: 'Hayır',
                    reverseButtons: true
                }).then(async (result) => {
                    if (result.value) {

                        Loader();

                        const url = "/Admin/SuspendToogleFeature";
                        let data = {
                            uid: uid
                        };

                        let res = null;
                        try {
                            res = await AxiosPostRequest(url, data);
                            admin.isSuspend = res.data.admin.isSuspend;

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
})(jQuery).catch(err => {
    console.error("Err ::: ", err);
});


