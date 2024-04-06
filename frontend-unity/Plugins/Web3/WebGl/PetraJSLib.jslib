
function ConnectToPetra(){
    window.petraAdapter.ConnectToPetra();
}

function Mint(){
    window.petraAdapter.Mint();
}

function Match(){
    window.petraAdapter.Match();
}

const PetraLib = 
{
    ConnectToPetra,
    Mint,
    Match,
}

mergeInto(LibraryManager.library, PetraLib);