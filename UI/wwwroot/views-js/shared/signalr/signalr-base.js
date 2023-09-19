"use strict";
// ============ Init SignalR Hub Connection ============
export const InitConnection = (url, security = false, getTokenAsync, maxReconnectCount = 10, initInterval = 5000, automaticReconnect = [1000, 1000, 2000, 2000, 3000, 3000]) => new Promise(async (resolve, reject) => {
    let try_counter = 0;

    if (typeof url != "string" || !url || !url.startsWith('http'))
        throw new Error("url is required");

    if (typeof security != "boolean")
        security = false;

    if (typeof maxReconnectCount != "number" || maxReconnectCount < 1)
        maxReconnectCount = 10;

    if (typeof initInterval != "number" || initInterval < 1000)
        initInterval = 5000;

    if (typeof automaticReconnect != "object" || automaticReconnect.length < 1)
        automaticReconnect = [1000, 1000, 2000, 2000, 3000, 3000];

    try {
        const connection = new signalR.HubConnectionBuilder()
            .withUrl(url, {
                accessTokenFactory: async () => {
                    if (security)
                        return await getTokenAsync();
                    return null;
                }
            })
            .withAutomaticReconnect(automaticReconnect)
            //.withHubProtocol(new signalR.protocols.msgpack.MessagePackHubProtocol())
            .build();

        async function startHub() {
            try {
                await connection.start();
                resolve(connection);
            } catch (err) {
                if (try_counter <= maxReconnectCount) {
                    try_counter++;
                    setTimeout(startHub, initInterval);
                }
                else { reject(err); }
            }
        }
        //connection.onreconnecting(err => { });
        //connection.onreconnected(id => { });
        connection.onclose(id => startHub());
        startHub();
    }
    catch (err) {
        reject(err);
    }
    finally { }
});





