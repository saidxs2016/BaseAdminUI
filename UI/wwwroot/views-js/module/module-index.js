import { CloseInitLoader, Loader, EmptyGuid, BlockUI, UnBlockUI, AxiosPostRequest, Wait, CloseLoader } from './../shared/site.js';

(async ($) => {
    "use strict";


    // ============== Global Varaibles ==============
    let datatable = null, datatableIsReady = false;

    // ============== Datatables ==============
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
                    data.uid = Uid;


                    // request ready 
                    const url = "/module/IndexFeature";
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
                    "data": "order",
                    "name": "#",
                    "render": (data, type, row, meta) => {
                        return data;
                    },
                    "orderable": true,
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
                            result = `<a class="btn badge badge-primary authorization-key" data-auth-key="/module/index/${row.uid}" data-placement="bottom" data-popup="tooltip" title="Alt Modüller" href="/module/index/${row.uid}"> ${row.subModuleList?.length ?? 0} </a>`
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

                        opr += `<a class="btn badge-warning me-1" data-placement="bottom" data-popup="tooltip" title="Düzenle" href="/module/update/${row.uid}"><i class="icon-pencil"></i> </a>`;
                        opr += `<a href="javascript:void(0);" class="btn badge-danger moduleDeleteFn" data-placement="bottom" data-uid="${row.uid}" data-popup="tooltip" title="Silme"><i class='fas fa-trash-alt'></i></a>`;

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
                InitSortableForRecords();
            },
            "createdRow": function (row, data, index) {
                $(row).addClass('item');
                $(row).attr('id', 'tr-' + data.uid);
            },
            "language": {
                url: "/js/datatables/tr.json"
            },

            "lengthMenu": [[25, 50, 100, -1], [25, 50, 100, "Hepsi"]],
            "order": [[0, 'asc']],
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
        // ============== Reorder records ==============
        const InitSortableForRecords = async () => {
            await Wait(50);
            $("#datatable").sortable({
                items: '.item',
                axis: "y",
                update: function () {
                    getOrder("table.table", "module", "order");
                }
            });
            async function getOrder(tableName, table, column) {
                let trclass = "odd";
                const uids = [];
                $(tableName + ' tbody tr.item').each(function (index, element) {
                    trclass = trclass == "odd" ? "even" : "odd";
                    const uid = $(this).attr("id").replace("tr-", "");
                    uids.push(uid);

                });
                try {
                    const url = "/module/orderRecordsFeature";
                    const data = { 'table': table, 'column': column, 'uids': uids };

                    const result = await AxiosPostRequest(url, data);

                    toastr["success"](result.message);
                    datatable.draw();
                    //await Wait(1500);
                    //location.reload(0);

                } catch (e) {
                    toastr["warning"](e.message);
                }
                finally {
                    CloseInitLoader();
                }

            }
        }
        // ============== Init Listener ==============
        const InitListener = async () => {
            await Wait(50);
            $('.moduleDeleteFn').on('click', function (e) {
                const uid = $(this).attr('data-uid');
                if (uid)
                    app.moduleDelete(uid);
            });






            $('#datatable .dtr-control').on('click', InitListener);// gizli elementlerein dinleyicilerini tekrar render etmek için

        }
    }

    // ============== Init VueMethod ==============
    const app = Vue.createApp({
        data() {
            return {
                modules: []
            }
        },
        async mounted() {
            //while (!datatableIsReady)
            //    await Wait(100);
            DataTable();
            CloseInitLoader();
        },
        methods: {
            moduleDelete(uid) {
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
                    type: 'info',
                    showCancelButton: true,
                    confirmButtonText: 'Evet',
                    cancelButtonText: 'Hayır',
                    reverseButtons: true
                }).then(async (result) => {
                    if (result.value) {

                        Loader();

                        const url = "/Module/DeleteFeature";
                        let data = { uid: uid };
                        try {
                            const res = await AxiosPostRequest(url, data);

                            let index = app.modules.findIndex(w => w.uid == uid);
                            app.modules.splice(index, 1);
                            datatable.draw();
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







