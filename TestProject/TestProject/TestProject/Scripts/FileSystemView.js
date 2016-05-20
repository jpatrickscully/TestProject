

var MY_FILE_APP = {};

$(document).ready(function () {
    refreshFileSystem();
});
function getFileById (here, id)
{
    if (id == null)
    {
        return null;
    }
    // TODO build an index 
    if (here.id == id)
    {
        return here;
    }
    else
    {
        if (here.isDirectory) {
            var i;
            for (i = 0; (i < here.children.length) ; i++) {
                child = getFileById(here.children[i], id);
                if (child != null) {
                    return child;
                }
            }
        }
    }
    return null;
}

function refreshFileSystem()
{
    getFileSystemRoot ( function (root)
    {
        MY_FILE_APP.root = root;
        if (window.history.state == null)
        {
            refershDisplayRoot(null);
        }
        else
        {
            refershDisplayRoot(window.history.state.topId);
        }
        

    })
}
//TODO be more clever on updating portions of the tree
function refershDisplayRoot (topId)
{
    var fileSystemView = $('#fileSystemView');
    newTop = getFileById(MY_FILE_APP.root, topId);
    if (newTop == null) // default to root
    {
        newTop = MY_FILE_APP.root;
    }
    var rootTree = $('<ul/>');
    makeFileRow(newTop).appendTo(rootTree);
    fileSystemView.empty();
    rootTree.appendTo(fileSystemView)
    
}

window.onpopstate = function (event) {
    if (event.state == null) {
        refershDisplayRoot(null);
    }
    else {
        refershDisplayRoot(event.state.topId);
    }
};

// TODO refresh folders that don't require a load of the whole directory tree

function makeFileRow (file)
{
    var row = $('<li/>');
    
    if (file.isDirectory)
    {
        row.text("Directory with " + file.children.length + " item(s) "  );

        var navLink = $('<a></a>').appendTo(row);
        
        navLink.attr('href', 'javascript:  var stateobj =  {topId:"' + file.id + '"}; history.pushState (stateobj,"p2","' + file.name + '.html"); refershDisplayRoot(' + file.id +');' + file.id);
        navLink.text(file.name);

        // Don't add a delete button if it is the top directory
        if (file.id != file.parentId)
        {
            if((window.history.state == null) || (file.id != window.history.state.topId))
            {
                var delDiv = $('<div/>').appendTo(row);
                var deleteFileBtn = $('<input type="button" value = "Delete" />').appendTo(delDiv);
                deleteFileBtn.click(function (event) {
                    deleteFile(file.id, function () { refreshFileSystem(); });
                });
            }
        }
        ///////// Upload File
        var upDiv = $('<div/>').appendTo(row);
        var filePicker = $('<input type = "file"/>').appendTo(upDiv);
        var uploadFileBtn = $('<input type="button" value = "Upload File" />').appendTo(upDiv);
        uploadFileBtn.click(function (event) {
            var pickedFile = filePicker.get(0).files[0];
            uploadFile(file.id, pickedFile.name, pickedFile, function () { refreshFileSystem(); });
        });

        ////////////////  New Directory
        var newDirectoryName = $('<input type="text"/>').appendTo(row);
        var newDirectoryBtn = $('<input type="button" value = "New Directory" />').appendTo(row);;
        newDirectoryBtn.click(function (event) {
            var dirName = newDirectoryName.val();
            createNewDirectory(file.id, dirName, function () { refreshFileSystem(); })
        });
   

        if (file.children.length > 0) {
            var dirTree = $('<ul/>').appendTo(row);
            var i;
            for (i = 0; (i < file.children.length) ; i++) {
                makeFileRow(file.children[i]).appendTo(dirTree);
            }
        }
        
    }
    else
    {   
        var downloadLink = $('<a></a>').appendTo(row);
        downloadLink.attr('href','/api/Files/' + file.id);
        downloadLink.text(file.name)
        var info = $('<div/>').appendTo(row);
        info.text(file.sizeInBytes + " bytes");
        var deleteFileBtn = $('<input type="button" value = "Delete" />').appendTo(row);
        deleteFileBtn.click(function (event) {
            deleteFile(file.id, function () { refreshFileSystem(); });
        });
    }

    return row;
}