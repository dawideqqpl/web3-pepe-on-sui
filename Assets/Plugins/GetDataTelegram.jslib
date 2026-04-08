mergeInto(LibraryManager.library, {

    GetData : function()
    {
        print("TEST1 : " + Telegram.WebApp.initData);
        window.unityInstance.SendMessage('GameMangerObject', 'getTelegramData', JSON.stringify(Telegram.WebApp.initData));
    }, CallJavaScript: function () {
        
        window.tryConnect();
    },
      getWalletData: function () {
        
        window.tryWallet();
    },

     getIsConnected: function () {
        
        window.tryConnected();
    },
});
