import { CloseInitLoader, Loader, EmptyGuid, BlockUI, UnBlockUI, AxiosPostRequest, Wait, CloseLoader } from './../shared/site.js';

(async ($) => {

    "use strict";

    const app = Vue.createApp({
        data() {
            return {
                roles: [],
                modules: [],
                modulesAsArray: [],
                checkedArr: []
            }
        },
        async mounted() {
            try {  
                const url = "/role/getAuthenticateFeature";
                const data = { uid: Uid };

                let result = await AxiosPostRequest(url, data);
                app.modules = result.data.moduleList;
                app.checkedArr = result.data.checkedModules;

                if (app.modules) {
                    app.modules.forEach(cat => {
                        app.modulesAsArray.push(cat);
                        if (cat.subModuleList) {
                            cat.subModuleList.forEach(page => {
                                app.modulesAsArray.push(page);
                                if (page.subModuleList) {
                                    page.subModuleList.forEach(feature => {
                                        app.modulesAsArray.push(feature);
                                    });
                                }
                            });
                        }
                    });
                }

                //this.$nextTick(function () {
                //    this.initParentTagListener();
                //});

                this.initParentTagListener();


            } catch (e) {
                toastr["warning"](e.message);
            }
            finally {
                CloseInitLoader();
            }

        },

        methods: { 
            async SubmitForm() {
                try {


                    let pages = app.modulesAsArray;

                    const url = "/role/postAuthenticateFeature";
                    const data = { uid: Uid, checkedArr: app.checkedArr, pages };

                    let result = await AxiosPostRequest(url, data);
                    //app.modules = result.data.moduleList;
                    //app.checkedArr = result.data.checkedModules;

                    toastr["success"](result.message);

                    await Wait(500);
                    location.href = `/role/index/${Uid}`

                } catch (e) {
                    toastr["warning"](e.message);
                }
                finally {
                    CloseInitLoader();
                }
            },
            subToggle(uid) {
                let noneis = $('.sub' + uid + ':eq(0)').css('display');
                if (noneis == "none") {
                    $('.sub' + uid).css('display', 'block');
                    let state = app.modulesAsArray.find(i => i.uid == uid);
                    if (state && !state.tagifyIsReady)
                        app.initTagListener(uid);
                }
                else
                    $('.sub' + uid).css('display', 'none');
            },
            selectAllPage(uid) {
                const selectedParent = app.checkedArr.find(w => w == uid);

                const module = app.modules.find(i => i.uid == uid);
                if (module.subModuleList)
                    module.subModuleList.forEach(item => {
                        const exist = app.checkedArr.some(i => i == item.uid);
                        if (exist)
                            app.checkedArr.splice(app.checkedArr.findIndex(i => i == item.uid), 1);

                    });

                if (selectedParent && module.subModuleList)
                    module.subModuleList.forEach(item => app.checkedArr.push(item.uid));

                let pages = module.subModuleList;
                if (pages)
                    pages.forEach(i => {
                        app.selectAllFeature(i.uid);
                    });
            },
            selectAllFeature(uid) {
                const selectedParent = app.checkedArr.find(w => w == uid);
                let modules = app.modules.map(i => [...i.subModuleList]).flat();
                const module = modules.find(i => i.uid == uid);
                if (module.subModuleList)
                    module.subModuleList.forEach(item => {
                        const exist = app.checkedArr.some(i => i == item.uid);
                        if (exist)
                            app.checkedArr.splice(app.checkedArr.findIndex(i => i == item.uid), 1);

                    });

                if (selectedParent && module.subModuleList)
                    module.subModuleList.forEach(item => app.checkedArr.push(item.uid));
            },
            initTagListener(uid) {
                if (!app.modulesAsArray.find(i => i.uid == uid))
                    return;
                app.modulesAsArray.find(i => i.uid == uid).tagifyIsReady = true;
                let inputs = document.querySelectorAll(`.tag-${uid}`);
                if (inputs) {
                    inputs.forEach(input => {
                        let pages = app.modulesAsArray;


                        let tagify = new Tagify(input, {
                            editTags: 1,
                            maxTags: 10,
                            delimiters: "-&&-",
                            duplicates: false,
                            hooks: {
                                beforePaste: function (content) {
                                    return new Promise((resolve, reject) => {
                                        confirm("Yapıştırmaya izin ver?")
                                            ? resolve()
                                            : reject()
                                    })
                                }
                            }
                        });

                        tagify.addTags(pages.find(i => i.uid == $(input).attr('data-uid'))?.ignoredSectionList ?? []);
                        // Chainable event listeners
                        tagify.on('add', function (e) {
                            let page = pages.find(i => i.uid == $(input).attr('data-uid'));
                            if (!page?.ignoredSectionList)
                                page.ignoredSectionList = [];
                            page.ignoredSectionList.push(e.detail.data.value);
                        });
                        tagify.on('remove', function (e) {
                            let page = pages.find(i => i.uid == $(input).attr('data-uid'));
                            if (!page.ignoredSectionList)
                                page.ignoredSectionList = [];

                            let index = page.ignoredSectionList.findIndex(i => i == e.detail.data.value);
                            if (index >= 0)
                                page.ignoredSectionList.splice(index, 1);
                        });
                        let old_tag = "";
                        tagify.on('edit:start', function (e) {
                            old_tag = e.detail.data.value;
                        });
                        tagify.on('edit:updated', function (e) {
                            let page = pages.find(i => i.uid == $(input).attr('data-uid'));
                            if (!page.ignoredSectionList)
                                page.ignoredSectionList = [];

                            let index = page.ignoredSectionList.findIndex(i => i == old_tag);
                            page.ignoredSectionList[index] = e.detail.data.value;
                        });
                    });
                }
            },
            initParentTagListener() {
                let inputs = document.querySelectorAll('.tag-category');
                //inputs = $(inp);

                if (inputs) {
                    inputs.forEach(input => {

                        let tagify = new Tagify(input, {
                            editTags: 1,
                            maxTags: 10,
                            delimiters: "-&&-",
                            duplicates: false,
                            hooks: {
                                beforePaste: function (content) {
                                    return new Promise((resolve, reject) => {
                                        confirm("Yapıştırmaya izin ver?")
                                            ? resolve()
                                            : reject()
                                    })
                                }
                            }
                        });

                        tagify.addTags(app.modulesAsArray.find(i => i.uid == $(input).attr('data-uid'))?.ignoredSectionList ?? []);
                        // Chainable event listeners
                        tagify.on('add', function (e) {
                            let page = app.modulesAsArray.find(i => i.uid == $(input).attr('data-uid'));
                            if (!page?.ignoredSectionList)
                                page.ignoredSectionList = [];
                            page.ignoredSectionList.push(e.detail.data.value);
                        });
                        tagify.on('remove', function (e) {
                            let page = app.modulesAsArray.find(i => i.uid == $(input).attr('data-uid'));
                            if (!page.ignoredSectionList)
                                page.ignoredSectionList = [];

                            let index = page.ignoredSectionList.findIndex(i => i == e.detail.data.value);
                            if (index >= 0)
                                page.ignoredSectionList.splice(index, 1);
                        });
                        let old_tag = "";
                        tagify.on('edit:start', function (e) {
                            old_tag = e.detail.data.value;
                        });
                        tagify.on('edit:updated', function (e) {
                            let page = app.modulesAsArray.find(i => i.uid == $(input).attr('data-uid'));
                            if (!page.ignoredSectionList)
                                page.ignoredSectionList = [];

                            let index = page.ignoredSectionList.findIndex(i => i == old_tag);
                            page.ignoredSectionList[index] = e.detail.data.value;
                        });
                    });
                }
            }

        }
    }).mount("#app");
      
    
 
 
}) (jQuery).catch(err => {
    console.error("Err ::: ", err);
});




