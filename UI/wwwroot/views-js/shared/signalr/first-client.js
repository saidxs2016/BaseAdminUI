
import { InitConnection } from './signalr-base.js';
import { AxiosPostRequest, Wait } from './../site.js';

let connection = null;
// ============ Wait == Delay in js ============

// ============ Axios Kullanarak Get istegi atma ============
export const FirstClient = () => new Promise(async (resolve, reject) => {

    "use strict";
    if (connection && connection.state == signalR.HubConnectionState.Connected) {
        resolve(connection);
        return;
    }


    try {
        const url = "https://localhost:7060/hubs/notification_hub";
        const security = false;
        const getTokenAsync = async () => {


            //const res = await AxiosPostRequest(token_url, data);

            return null;
        };
        const maxReconnectCount = 10;
        const initInterval = 5000;
        const automaticReconnect = [1000, 2000, 2000, 3000, 3000, 5000];


        const CallAsyncConnect = async () => {
            connection = await InitConnection(url, security, getTokenAsync, maxReconnectCount, initInterval, automaticReconnect);
        }
        CallAsyncConnect();

        await Wait(1000);
        resolve(connection);
    }
    catch (err) {
        reject(err);
    }
    finally { }
});





