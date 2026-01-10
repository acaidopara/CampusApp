window.blazorNetwork = {
    check: () => navigator.onLine,

    register: (dotNetHelper) => {
        window._onOnline = () => dotNetHelper.invokeMethodAsync("SetOfflineStatus", false);
        window._onOffline = () => dotNetHelper.invokeMethodAsync("SetOfflineStatus", true);

        window.addEventListener("online", window._onOnline);
        window.addEventListener("offline", window._onOffline);
    },

    unregister: () => {
        if (window._onOnline) {
            window.removeEventListener("online", window._onOnline);
            window.removeEventListener("offline", window._onOffline);
        }
    }
};
