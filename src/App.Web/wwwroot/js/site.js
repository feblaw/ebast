var theDataTable = null;

function initPlUploadWidget(element, uploadUrl, uploadedItemTemplate, uploadedListElement, uploadedValueElement, postInit) {
    return $(element).pluploadQueue({
        runtimes: 'html5,html4',
        url: uploadUrl,
        max_file_size: '5mb',
        
        filters: [
            { title: "All files", extensions: "*" }
        ],
        rename: true,
        sortable: true,
        dragdrop: true,
        multiple_queues: true,
        views: {
            list: true,
            active: 'list'
        },
        init: {
            PostInit: function () {
                $(element).find(".plupload_header").hide();
                $(element).find(".plupload_content").css("top", 0);
                $(element).find(".plupload_content").css("bottom", 0);
                $(element).find(".plupload_filelist_footer").css("position", "static");
                $(element).find(".plupload_container").prop("title", "");
                if (postInit) postInit(element);
            },
            FileUploaded: function (up, file, info) {
                var dataObj = JSON.parse(info.response);
                var divContent = $(uploadedItemTemplate).html();
                divContent = divContent.replace(/{{name}}/ig, dataObj.data.name);
                divContent = divContent.replace(/{{size}}/ig, dataObj.data.size);
                divContent = divContent.replace(/{{path}}/ig, dataObj.data.path);
                if ($(uploadedListElement).find('.no-attachment').length === 0)
                    $(uploadedListElement).append(divContent);
                else $(uploadedListElement).html(divContent);
                var value = $(uploadedValueElement).val() + dataObj.data.path + "|";
                $(uploadedValueElement).val(value);
            }
        }
    });
}

function removeAttachment(element, uploadedListElement, uploadedValueElement, ajaxUrl) {
    var Deleted = $(element).data("to-delete");
    var value = $(uploadedValueElement).val();
    value = value.replace($(element).data("to-delete") + "|", "");
    $(uploadedValueElement).val(value);
    if (ajaxUrl !== undefined && ajaxUrl !== null) {
        $.ajax({ url: ajaxUrl, success: function (result) { afterRemoveAttachment(element, uploadedListElement, uploadedValueElement); } });
    } else afterRemoveAttachment(element, uploadedListElement, uploadedValueElement);
}

var noAttachment = '<p class="form-control-static"><a class="no-attachment" href="#">No Attachment</a></p>';
var noAttachmentDt = '<strong>No Attachment</strong>';

function afterRemoveAttachment(element, uploadedListElement, uploadedValueElement) {
    $(element).parent().remove();
    if ($(uploadedValueElement).val() === '') $(uploadedListElement).html(noAttachment);
}

function extractFilename(url) {
    var filename = '';
    var startWhere = url.lastIndexOf('/');
    if (startWhere === -1) startWhere = 0;
    var splitWhere = url.indexOf('_', startWhere);
    if (splitWhere === -1) filename = url;
    else filename = url.slice(splitWhere + 1);
    return filename;
}

function renderExistingFiles(dataElement, targetElement, baseHref, editable, listItemElement, renderDt) {

    if (editable === true) {
        var list = $(dataElement).val();
        var row = list.split("|");
        var val = '';
        if (row[0] == '') {
            val = noAttachment;
        } else {
            for (i = 0; i < row.length; i++) {
                if ((typeof row[i]) !== 'string' || row[i] === '') continue;
                var filename = extractFilename(row[i]);
                var btn_group = '<div class="btn-group " style="margin-right:5px;">';
                btn_group += '<a target="_blank" class="btn btn-sm btn-success" href="' + baseHref + sanitizeHref(row[i]) + '"><i class="fa fa-paperclip"></i> ' + filename + '</a>';
                btn_group += '<a class="btn btn-sm btn-success delete-upload" data-to-delete="' + sanitizeHref(row[i]) + '" href="javascript:void(0)"><i class="fa fa-times"></i></a>';
                btn_group += '</div>';
                val += btn_group;
            }
        }

        $(targetElement).html(val);
    } else if (listItemElement !== undefined && listItemElement !== null && listItemElement !== '') {
        $(targetElement).each(function () {
            var row = $(this).find(listItemElement).text();
            $(this).find(listItemElement).text(extractFilename(row));
            $(this).find("a").prop("href", $(this).find("a").prop("href") + sanitizeHref(row));
        });
    } else {
        if (renderDt !== undefined && renderDt !== null && typeof renderDt === 'string') {
            var json = JSON.parse(renderDt);
            //var json = eval(_json);
            if (json.length === 0) return noAttachmentDt;
            var content = "<ul style='list-style-type:none; padding:0;'>";
            for (i = 0; i < json.length; i++)
                content += "<li><a href=" + baseHref + sanitizeHref(json[i]) + "><i class='fa fa-paperclip'></i> " + extractFilename(json[i]) + "</a></li>";
            content += "<ul>";
            return content;
        } else return noAttachmentDt;
    }
}

function sanitizeHref(dirty) {
    if (typeof dirty !== 'string' || dirty == null) return '#';

    var split = dirty.split('/');
    var res = [];
    for (var i = 0; i < split.length; i++) {
        res.push(encodeURIComponent(split[i]));
    }

    return res.join('/');
}

function disableImmediateSearchDt(e, settings, json, tableSelector) {
    var searchInput = $(tableSelector + '_wrapper .dataTables_filter input');
    searchInput.off();
    searchInput.on('keyup', function(e) {
        if(e.keyCode == 13)
            $(tableSelector).DataTable().search(this.value).draw();
    });
}
