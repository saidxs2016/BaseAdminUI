import { CloseInitLoader, Loader, EmptyGuid, BlockUI, UnBlockUI, AxiosPostRequest, Wait, CloseLoader } from './../shared/site.js';


(async ($) => {

    "use strict";

    const app = Vue.createApp({
        data() {
            return {
                roles: [],
                selectedRole: {
                    uid: null,
                    loginCount: 1,
                    expiration: "1 d",
                    expirationValue: "1",
                    expirationInterval: "d"

                },
                intervalAsArr: []
            }
        },
        async mounted() {
            try {

                const url = "/role/indexFeature";
                const data = {};

                let result = await AxiosPostRequest(url, data);
                app.roles = result.data;


                app.intervalAsArr = IntervalsAsArr; 

                InitDatatable();
            } catch (e) {
                toastr["warning"](e.message);
            }
            finally {
                CloseInitLoader();
            }

        },

        methods: {
            openLoginConstraintModal(uid) {
                if (!uid || uid == EmptyGuid())
                    return;
                app.selectedRole.uid = uid;
                const role = app.roles.find(w => w.uid == uid);
                app.selectedRole.loginCount = role.loginCount;
                app.selectedRole.expiration = role.expiration;
                if (role.expiration && role.expiration.split(' ')) {
                    app.selectedRole.expirationValue = role.expiration.split(' ')[0];
                    app.selectedRole.expirationInterval = role.expiration.split(' ')[1];
                }

                $('#login-constraint-modal').modal('show');
            },
            async loginConstraintChange() {
                Loader();

                const url = "/Role/LoginConstraintFeature";
                let data = app.selectedRole;
                data.expiration = app.selectedRole.expirationValue + ' ' + app.selectedRole.expirationInterval;
                let res = null;
                try {
                    res = await AxiosPostRequest(url, data);

                    $('#login-constraint-modal').modal('hide');
                    toastr["success"](res.message);
                }
                catch (err) {
                    toastr["warning"](err.message);
                }
                finally {
                    CloseLoader();
                }
            }
        }
    }).mount("#app");




    let datatable = null;
    const InitDatatable = async () => {
        await Wait(250);
        datatable = $('#datatable').DataTable({
            "language": {
                url: "/js/datatables/tr.json"
            },
            "lengthMenu": [[25, 50, 100, -1], [25, 50, 100, "Hepsi"]],
            "order": [[0, 'desc']],
            "dom": 'lBftip',
            "buttons": [
                {
                    extend: 'excelHtml5',
                    exportOptions: {
                        columns: ':visible'
                    }
                },
                {
                    extend: 'pdfHtml5',
                    exportOptions: {
                        columns: ':visible'
                    },
                    orientation: 'landscape',
                    pageSize: 'TABLOID',
                    customize: function (doc) {
                        let tblBody = doc.content[1].table.body;
                        // ***
                        //This section creates a grid border layout
                        // ***
                        doc.content[1].layout = {
                            hLineWidth: function (i, node) {
                                return (i === 0 || i === node.table.body.length) ? 2 : 1;
                            },
                            vLineWidth: function (i, node) {
                                return (i === 0 || i === node.table.widths.length) ? 2 : 1;
                            },
                            hLineColor: function (i, node) {
                                return (i === 0 || i === node.table.body.length) ? 'black' : 'gray';
                            },
                            vLineColor: function (i, node) {
                                return (i === 0 || i === node.table.widths.length) ? 'black' : 'gray';
                            }
                        };
                    }
                },
                {
                    extend: 'colvis',
                    text: 'G�ster/Gizle'
                },

            ]
        });

        await Wait(100);
        //========================= table css ===================
        let datatable_wrapper = {
            display: "flex",
            flexWrap: "wrap",
            justifyContent: "space-between"
        };

        $('#datatable_wrapper').css(datatable_wrapper);
        $('#datatable_length').css("marginTop", "8px");
        $('.dt-buttons').css("marginTop", "8px");
        $('.dt-buttons button').css("marginRight", "5px");
        $('#datatable_filter').css("marginTop", "8px");
        $('#datatable').css("width", "100%");
        let datatable_paginate = {
            margin: "auto",
            marginTop: "4px",
            marginRight: "0px"
        };
        $('#datatable_paginate').css(datatable_paginate);
    }
})(jQuery).catch(err => {
    console.error("Err ::: ", err);
});


