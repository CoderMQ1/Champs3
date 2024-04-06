const MetaMaskPlugin = {
    GetPtrFromString : function(str){
        var bufferSize = lengthBytesUTF8(str) + 1;
        var buffer = _malloc(bufferSize);
        stringToUTF8(str, buffer, bufferSize);
        return buffer;
    },

    Hexer : function(input) {
        const utf8 = MetaMaskPlugin.ToUTF8Array(input);
        const hex = utf8.map(n => n.toString(16));
        return '0x' + hex.join('');
    },

    ToUTF8Array : function(str) {
        var utf8 = [];
        for (var i=0; i < str.length; i++) {
            var charcode = str.charCodeAt(i);
            if (charcode < 0x80) utf8.push(charcode);
            else if (charcode < 0x800) {
                utf8.push(0xc0 | (charcode >> 6),
                        0x80 | (charcode & 0x3f));
            }
            else if (charcode < 0xd800 || charcode >= 0xe000) {
                utf8.push(0xe0 | (charcode >> 12),
                        0x80 | ((charcode>>6) & 0x3f),
                        0x80 | (charcode & 0x3f));
            }
            // surrogate pair
            else {
                i++;
                // UTF-16 encodes 0x10000-0x10FFFF by
                // subtracting 0x10000 and splitting the
                // 20 bits of 0x0-0xFFFFF into two halves
                charcode = 0x10000 + (((charcode & 0x3ff)<<10)
                        | (str.charCodeAt(i) & 0x3ff));
                utf8.push(0xf0 | (charcode >>18),
                        0x80 | ((charcode>>12) & 0x3f),
                        0x80 | ((charcode>>6) & 0x3f),
                        0x80 | (charcode & 0x3f));
            }
        }
        return utf8;
    }

}


function ConnectToMetaMask(connectCallback, connectFailCallback){
    window.web3Adapter.ConnectToMetaMask();
}

function MetaMaskRequest(paramsJson){
    window.web3Adapter.MetaMaskRequest(UTF8ToString(paramsJson));
}

function StartMatch(matchInfo, account){
    console.log(UTF8ToString(account));
    window.web3Adapter.StartMatch(UTF8ToString(matchInfo), UTF8ToString(account));
}

function RotateSpin(spinId, account){
    window.web3Adapter.RotateSpin(spinId, UTF8ToString(account));
}

function ExchangeEnergy(orderInfo, account){
    window.web3Adapter.ExchangeEnergy(UTF8ToString(orderInfo), UTF8ToString(account));
}

function GetBalance(account){
    window.web3Adapter.GetBalance(UTF8ToString(account));
}

function BuyGoods(orderInfo, account){
    window.web3Adapter.BuyGoods(UTF8ToString(orderInfo), UTF8ToString(account));
}

function UpgradeRole(orderInfo, account){
    window.web3Adapter.UpgradeRole(UTF8ToString(orderInfo), UTF8ToString(account));
}

function GetRewardPool(account){
    window.web3Adapter.GetRewardPool(UTF8ToString(account));
}

function ApproveMatchGame(orderInfo, account){
    window.web3Adapter.ApproveMatchGame(UTF8ToString(orderInfo), UTF8ToString(account));
}

const Web3Lib = 
{
    $MetaMaskPlugin:MetaMaskPlugin,
    ConnectToMetaMask,
    MetaMaskRequest,
    StartMatch,
    RotateSpin,
    ExchangeEnergy,
    GetBalance,
    BuyGoods,
    UpgradeRole,
    ApproveMatchGame,
    GetRewardPool
}

autoAddDeps(Web3Lib, '$MetaMaskPlugin');
mergeInto(LibraryManager.library, Web3Lib);