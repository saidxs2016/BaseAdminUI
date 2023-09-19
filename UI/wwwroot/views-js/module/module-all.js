import { CloseInitLoader, Loader, EmptyGuid, BlockUI, UnBlockUI, AxiosPostRequest, Wait, CloseLoader } from './../shared/site.js';

(async ($) => {
    "use strict";


    // ============== Global Varaibles ==============
    let datatable = null, datatableIsReady = false;

    // ============== DataTable ==============
    const DataTable = () => {
        if (datatable)
            return datatable;

        datatable = $('#datatable').DataTable({
            "responsive": true,
            "processing": true,
            "serverSide": true,
            "paging": true,
            "filter": true,
            "ajax": async function (data, callback, settings) {
                try {
                    BlockUI("#datatable");
                    //custom data 
                    //data.uid = Uid;


                    // request ready 
                    const url = "/module/AllFeature";
                    const config = {
                        headers: {
                            'Content-Type': 'multipart/form-data',
                            'RequestVerificationToken': VerifyToken ?? null
                        }
                    };
                    const result = await AxiosPostRequest(url, data, config);
                    app.modules = result.data;
                    callback(result);
                } catch (err) {
                    console.log("err", err);
                    callback({ recordsFiltered: 0, recordsTotal: 0, data: null });
                } finally {
                    UnBlockUI("#datatable");
                }

            },
            "columns": [
                {
                    "data": "uid",
                    "name": "Uid",
                    "render": (data, type, row, meta) => {
                        return data;
                    },
                    responsivePriority: 3,
                    "orderable": false,
                },
                {
                    "data": "name",
                    "name": "Modül Adı",
                    "render": (data, type, row, meta) => {
                        return data;
                    },

                    "orderable": true,
                },
                {
                    "data": "controller",
                    "name": "Controller",
                    "render": (data, type, row, meta) => {
                        return data;
                    },
                    "orderable": true,
                },
                {
                    "data": "action",
                    "name": "Action",
                    "render": (data, type, row, meta) => {
                        return data;
                    },
                    "orderable": true,
                },
                {
                    "data": "address",
                    "name": "Adres",
                    "render": (data, type, row, meta) => {
                        return data;
                    },
                    responsivePriority: 2,
                    "orderable": true,
                },
                {
                    "data": "icon",
                    "name": "İkon",
                    "render": (data, type, row, meta) => {
                        let result = `<i class="${data}"></i>`
                        return result;
                    },
                    "orderable": true,
                },
                {
                    "data": null,
                    "name": "Alt Modüller",
                    "render": (data, type, row, meta) => {
                        let result = "";
                        if (!row.type || row.type.length < 1)
                            return result;
                        if (row.type != "Feature")
                            result = `<a class="btn badge badge-primary authorization-key" data-placement="bottom" data-popup="tooltip" title="Alt Modüller" href="javascript:void(0);"> ${row.subModuleList?.length ?? 0} </a>`
                        return result;
                    },
                    "orderable": true,
                },
                {
                    "data": "parentModule.name",
                    "name": "Parent Module",
                    "render": (data, type, row, meta) => {
                        let result = data ?? "";
                        return result;
                    },

                    "orderable": true,
                },
                {
                    "data": "isMenu",
                    "name": "Menu",
                    "render": (data, type, row, meta) => {
                        let result = data ? 'Evet' : 'Hayır';
                        return result;
                    },

                    "orderable": true,
                },
                {
                    "data": "type",
                    "name": "Tip",
                    "render": (data, type, row, meta) => {
                        return data ?? "";
                    },

                    "orderable": true,
                },
                {
                    "data": "addDate",
                    "name": "Ekleme Zamanı",
                    "render": (data, type, row, meta) => {
                        let date = data ? moment(data).format("DD-MM-YYYY HH:mm:ss") : "";
                        return date;
                    },
                    "orderable": true,
                    "visible": false,
                },
                {
                    "data": "updateDate",
                    "name": "Güncelleme Zamanı",
                    "render": (data, type, row, meta) => {
                        let date = data ? moment(data).format("DD-MM-YYYY HH:mm:ss") : "";
                        return date;
                    },
                    "orderable": true,
                    "visible": false,
                },
                {
                    "data": null,
                    "name": "İşlemler",
                    "render": (data, type, row, meta) => {
                        let opr = "";

                        opr += `<a href="javascript:void(0);" class="btn badge-danger openParentChangeModal" data-placement="bottom" data-uid="${row.uid}" data-popup="tooltip" title="Parent Değiştir"><i class='fas fa-exchange-alt'></i></a>`;


                        return opr;
                    },
                    responsivePriority: 1,
                    orderable: false,
                },
            ],
            "preDrawCallback": function (settings, json) {
                datatableIsReady = false;
            },
            "drawCallback": function () {
                datatableIsReady = true;
                InitListener();
                InitExternalCss();
            },
            "createdRow": function (row, data, index) {
                $(row).addClass('item');
                $(row).attr('id', 'tr-' + data.uid);
            },
            "language": {
                url: "/js/datatables/tr.json"
            },

            "lengthMenu": [[25, 50, 100, -1], [25, 50, 100, "Hepsi"]],
            "order": [[1, 'asc']],
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
                    text: 'Göster/Gizle'
                },

            ]
        });

        // ============== Add css to datatable ==============
        const InitExternalCss = async () => {
            await Wait(50);
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

        // ============== Init Listener ==============
        const InitListener = async () => {
            await Wait(50);
            $('.openParentChangeModal').on('click', function (e) {
                const uid = $(this).attr('data-uid');
                if (uid)
                    app.openParentChangeModal(uid);
            });






            $('#datatable .dtr-control').on('click', InitListener);// gizli elementlerein dinleyicilerini tekrar render etmek için

        }
    }

    // ============== Vue ==============
    const app = Vue.createApp({
        data() {
            return {
                modules: [],
                module: {
                    uid: null,
                    parentUid: null
                }
            }
        },
        async mounted() {
            try {
                DataTable();

            } catch (e) {
                toastr["warning"](e.message);
            }
            finally {
                CloseInitLoader();
            }

        },

        methods: {
            openParentChangeModal(uid) {
                if (!uid)
                    return;
                app.module.uid = uid;
                $('#parent-change-modal').modal('show');
            },
            dateFormatFull(date) {
                let str = "";
                if (date) {
                    str = moment(date).format('DD-MM-YYYY HH:mm:ss');
                }
                return str;
            },
            parentChange() {
                const swal = Swal.mixin({
                    customClass: {
                        confirmButton: 'btn btn-success',
                        cancelButton: 'btn btn-danger'
                    },
                    buttonsStyling: false
                })
                swal.fire({
                    title: 'Emin misiniz?',
                    text: "Parentini Değiştirmek istiyor musunuz?",
                    type: 'warning',
                    showCancelButton: true,
                    confirmButtonText: 'Evet',
                    cancelButtonText: 'Hayır',
                    reverseButtons: true
                }).then(async (result) => {
                    if (result.value) {

                        Loader();
                        debugger
                        const url = "/Module/ChangeParentFeature";
                        let data = {
                            uid: app.module.uid,
                            parentUid: app.module.parentUid
                        };


                        let res = null;
                        try {
                            res = await AxiosPostRequest(url, data);

                            let module = app.modules.find(w => w.uid == app.module.uid);
                            module.parentUid = app.module.parentUid;

                            toastr["success"](res.message);
                            $('#parent-change-modal').modal('hide');

                            datatable.draw();
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





