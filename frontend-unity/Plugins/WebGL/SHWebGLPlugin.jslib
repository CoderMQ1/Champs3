const SHWebGLPlugin = {

    GetLocationURLFunction : function()
    {
        var url = window.location.href;
        
        var bufferSize = lengthBytesUTF8(url) + 1;
        var buffer = _malloc(bufferSize);
        stringToUTF8(url, buffer, bufferSize);
        return buffer;
    },
    
    CopyToClipboard : function copyToClipboard(text) {
      navigator.clipboard.writeText(UTF8ToString(text)).then(function() {
        console.log('Text copied to clipboard');
      }, function(err) {
        console.error('Failed to copy text to clipboard: ', err);
      });
    }
};

mergeInto(LibraryManager.library, SHWebGLPlugin);
