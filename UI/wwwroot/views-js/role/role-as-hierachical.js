import { CloseInitLoader, Loader, EmptyGuid, BlockUI, UnBlockUI, AxiosPostRequest, Wait, CloseLoader } from './../shared/site.js';

(async ($) => {

    "use strict";

    const app = Vue.createApp({
        data() {
            return {
                roles: [],
                tree: ""
            }
        },
        async mounted() {
            try { 
                const url = "/Role/RoleAsHierachicalFeature";
                const data = {};

                let result = await AxiosPostRequest(url, data);
                app.roles = result.data.roleList;
                await Wait(100);
                ShowTree();
            } catch (e) {
                toastr["warning"](e.message);
            }
            finally {
                CloseInitLoader();
            }

        },

        methods: {
            parentMethod(uid) {
                let role = app.roles.find(w => w.uid == uid);
                let parent = app.roles.find(w => w.uid == role.parentUid);
                if (!parent)
                    return "";
                return app.roles.findIndex(w => w.uid == parent.uid);
            },
            firstChild(uid) {
                let role = app.roles.find(w => w.uid == uid);
                let childs = app.roles.filter(w => w.parentUid == role.uid);
                if (!childs || childs.length == 0)
                    return "";
                return app.roles.findIndex(w => w.uid == childs[0]?.uid);
            },
            firstSibling(uid) {
                let role = app.roles.find(w => w.uid == uid);
                let siblings = app.roles.filter(w => w.parentUid == role.parentUid);
                if (!siblings)
                    return "";

                let index = siblings.findIndex(w => w.uid == uid);
                let last = siblings.length - 1;

                return index < last ? (app.roles.findIndex(w => w.uid == siblings[index + 1].uid)) : "";
            }
        }
    }).mount("#app");


    const ShowTree = () => {
        // -- init -- //
        jsPlumb.ready(function () {

            // connection lines style
            let connectorPaintStyle = {
                lineWidth: 3,
                strokeStyle: "#4F81BE",
                joinstyle: "round"
            };

            let pdef = {
                // disable dragging
                DragOptions: null,
                // the tree container
                Container: "treemain"
            };
            const plumb = jsPlumb.getInstance(pdef);

            // all sizes are in pixels
            const opts = {
                prefix: 'node_',
                // left margin of the root node
                baseLeft: 10,
                // top margin of the root node
                baseTop: 10,
                // node width
                nodeWidth: 100,
                // horizontal margin between nodes
                hSpace: 50,
                // vertial margin between nodes
                vSpace: 25,
                imgPlus: '/files/images/tree_expand.png',
                imgMinus: '/files/images/tree_collapse.png',
                // queste non sono tutte in pixel
                sourceAnchor: [1, 0.5, 1, 0, 10, 0],
                targetAnchor: "LeftMiddle",
                sourceEndpoint: {
                    endpoint: ["Image", { url: "/files/images/tree_collapse.png" }],
                    cssClass: "collapser",
                    isSource: true,
                    connector: ["Flowchart", { stub: [40, 60], gap: [10, 0], cornerRadius: 5, alwaysRespectStubs: false }],
                    connectorStyle: connectorPaintStyle,
                    enabled: false,
                    maxConnections: -1,
                    dragOptions: null
                },
                targetEndpoint: {
                    endpoint: "Blank",
                    maxConnections: -1,
                    dropOptions: null,
                    enabled: false,
                    isTarget: true
                },
                connectFunc: function (tree, node) {
                    var cid = node.data('id');
                    console.log('Connecting node ' + cid);
                }
            };
            const tree = jQuery.jsPlumbTree(plumb, opts);
            tree.init();
            //window.treemain = tree;
        });
    }
})(jQuery).catch(err => {
    console.error("Err ::: ", err);
});



