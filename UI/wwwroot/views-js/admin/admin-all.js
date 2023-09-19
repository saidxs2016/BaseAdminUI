import { CloseInitLoader, Loader, EmptyGuid, AxiosPostRequest, Wait, CloseLoader } from './../shared/site.js';

(async ($) => {

    "use strict";

    const app = Vue.createApp({
        data() {
            return {
                admins: [],
                selectedAdmin: {
                    uid: null,
                    roleUid: EmptyGuid()
                },
                roles: [],
                connectionKey: {
                    uid: null,
                    key: null
                }
            }
        },
        async mounted() {
            try {
                const url = `/admin/allFeature`;
                const data = {
                    uid: Uid
                };

                let result = await AxiosPostRequest(url, data);
                app.admins = result.data;

            } catch (e) {
                toastr["warning"](e.message);
            }
            finally {
                CloseInitLoader();
            }

            try {
                const url = `/admin/GetRolesFeature`;
                const data = {
                    uid: Uid
                };

                let result = await AxiosPostRequest(url, data);
                app.roles = result.data;

            } catch (e) {
                toastr["warning"](e.message);
            }
            finally {
                CloseInitLoader();
            }

        },

        methods: {
            async SubmitForm() {

            },
            dateFormatFull(date) {
                let str = "";
                if (date) {
                    str = moment(date).format('DD-MM-YYYY HH:mm:ss');
                }
                return str;
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
            openRoleChangeModal(uid) {
                if (!uid || uid == EmptyGuid())
                    return;

                const admin = app.admins.find(i => i.uid == uid);
                app.selectedAdmin.uid = uid;
                app.selectedAdmin.roleUid = admin?.roleUid;
                $('#role-change-modal').modal('show');
            },
            async roleChange() {

                Loader();

                const url = "/Admin/RoleChangeFeature";
                let data = {
                    uid: app.selectedAdmin.uid,
                    roleUid: app.selectedAdmin.roleUid,
                };

                let res = null;
                try {
                    res = await AxiosPostRequest(url, data);
                    $('#role-change-modal').modal('hide');

                    toastr["success"](res.message);

                    await Wait(1000);
                    location.reload();
                }
                catch (err) {
                    toastr["warning"](err.message);
                }
                finally {
                    CloseLoader();
                }
            },
            openLoginConnectionKeyModal(uid) {
                app.connectionKey.uid = uid;
                app.connectionKey.key = null;
                $('#login-connection-key-modal').modal('show');
            },
            async loginByConnectionKey() {

                Loader();

                const url = "/Admin/LoginByConnectionKeyFeature";
                let data = {
                    uid: app.connectionKey.uid,
                    key: app.connectionKey.key
                };

                let res = null;
                try {
                    res = await AxiosPostRequest(url, data);
                    $('#login-connection-key-modal').modal('hide');

                    toastr["success"](res.message);

                    /// bu alanda yeni penceere açmalıyız
                    await Wait(1000);
                    location.href = "/shared/newsession?to=" + res.redirect;
                }
                catch (err) {
                    toastr["warning"](err.message);
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




