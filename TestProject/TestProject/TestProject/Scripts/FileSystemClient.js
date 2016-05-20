

// To upload the file, create an empty with POST
// then upload the contents will a PUT
function uploadFile (parentId, fileName, file, onComplete)
{
    createNewFile(parentId, fileName, function (data) {
        // the file has been created and the id returned
        $.ajax({
            url: 'api/Files/' + data.id,
            type: 'PUT',
            data: file,
            contentType: 'application/octet-stream',
            processData: false,


            success: function (data, status, req) {
                console.log(status);
                if (onComplete != null) {
                    onComplete(data);
                }
            },

            error: function (data, status, req) {
                console.log(data);
                alert(data);
            }

        });
    });


}



function makeRequest (requestUrl, requestType, requestData, onComplete)
{
    var msgdata;
    if (requestData != null)
    {
        msgdata = JSON.stringify(requestData);
    }
    $.ajax({
        url: requestUrl,
        type: requestType,
        contentType: 'application/json',
        dataType:'json',
        data: msgdata,
        
        success: function (data, status, req) {
            console.log(status);
            if (onComplete != null) {
                onComplete(data);
            }
        },

        error: function (data, status, req) {
            console.log(data);
            alert(data);
        }

    });
}
function getFileSystemRoot (onComplete)
{
    makeRequest('/api/Files', 'GET', null, onComplete);
}

function createNewFile (parentId, fileName, onComplete)
{
    var requestData = {
        parentId: parentId,
        name: fileName,
        isDirectory: false
    };

    makeRequest('/api/Files', 'POST', requestData, onComplete);
}

function createNewDirectory(parentId, fileName, onComplete) {
    var requestData = {
        parentId: parentId,
        name: fileName,
        isDirectory: true
    };

    makeRequest('/api/Files', 'POST', requestData, onComplete);
}

function deleteFile (fileId, onComplete)
{
    makeRequest('/api/Files/' + fileId, 'DELETE', null, onComplete)


}