$(() => {
    function sendRequest(url, method = 'GET', data) {
        const d = $.Deferred();

        //logRequest(method, url, data);

        $.ajax(url, {
            method,
            data,
            dataType: 'json',
            //contentType: 'application/json;charset=UTF-8',
            cache: false,
            xhrFields: { withCredentials: true },
        }).done((result) => {
            if (method === 'GET')
                d.resolve(result, {
                    totalCount: result.totalCount,
                    summary: result.summary,
                    groupCount: result.groupCount,
                });
            else
                d.resolve( result);
        }).fail((xhr) => {
            d.reject(xhr.responseJSON ? xhr.responseJSON.Message : xhr.statusText);
        });

        return d.promise();
    }

    function logRequest(method, url, data) {
        const args = Object.keys(data || {}).map((key) => `${key}=${data[key]}`).join(' ');

        const logList = $('#requests ul');
        const time = DevExpress.localization.formatDate(new Date(), 'HH:mm:ss');
        const newItem = $('<li>').text([time, method, url.slice(URL.length), args].join(' '));

        logList.prepend(newItem);
    }

    function isNotEmpty(value) {
        return value !== undefined && value !== null && value !== '';
    }

    var l = abp.localization.getResource("MDM");
    var companyService = window.dMSpro.p42.mDM.companies.company;
    var getFilter = function () {
        return {
            filterText: '',
            code: '',
            name: '',
            address1: '',
            identityUserId: ''
        };
    };

    const url = '/api/mdm/companies';

    const store = new DevExpress.data.CustomStore({
        key: 'code',
        load(loadOptions) {

            const deferred = $.Deferred();
            
            console.log(JSON.stringify({ loadOptions: loadOptions }));

            $.ajax({
                url: url,
                dataType: 'json',
                type: 'GET',
                contentType: 'application/json;charset=UTF-8',
                data: loadOptions,
                success(result) {
                    console.log(result);
                    deferred.resolve(result, {
                        totalCount: result.totalCount,
                        summary: result.summary,
                        groupCount: result.groupCount,
                    });
                },
                error(e) {
                    deferred.reject('Data Loading Error');
                },
                timeout: 5000,
            });

            return deferred.promise();
        },
    });

    const comsStore = new DevExpress.data.CustomStore({
        key: 'id',
        load(loadOptions) {
            console.log(loadOptions);
            return sendRequest(`${url}`, 'GET', loadOptions);
        },
        insert(values) {
            return sendRequest(`${url}`, 'POST',
                JSON.stringify(values));
        },
        update(key, values) {
            return sendRequest(`${url}/${key}`, 'PUT',
                JSON.stringify(values));
        },
        remove(key) {
            return sendRequest(`${url}/${key}`, 'DELETE', JSON.stringify({
                id: key
            }));
        },
    });

    $('#gridContainer').dxDataGrid({
        //dataSource: DevExpress.data.AspNet.createStore({
        //    key: 'id',
        //    loadUrl: `${url}`,
        //    insertUrl: `${url}`,
        //    updateUrl: `${url}`,
        //    deleteUrl: `${url}`,
        //    onBeforeSend(method, ajaxOptions) {
        //        ajaxOptions.xhrFields = { withCredentials: true };
        //    },
        //}),
        dataSource: comsStore,
        showBorders: true,
        paging: {
            pageSize: 2,
        },
        //sorting: {
        //    mode: 'multiple',
        //},
        remoteOperations: true,

        columns: [
            {
                dataField: 'code',
                caption: l('Code'),
            },
            {
                dataField: 'name',
                caption: l('Name'),
            },
            {
                dataField: 'address1',
                caption: l('Address'),
            },
        ],
        //editing: {
        //    mode: 'row',
        //    allowUpdating: true,
        //    allowDeleting: true,
        //    allowAdding: true,
        //},
        //        allowColumnReordering: true,
//        allowColumnResizing: true,
//        columnAutoWidth: true,
//        columnChooser: {
//            enabled: true,
//        },
//        columnFixing: {
//            enabled: true,
//        },
//        filterRow: {
//            visible: true,
//            applyFilter: 'auto',
//        },
//        searchPanel: {
//            visible: true,
//            width: 240,
//            placeholder: 'Search...',
//        },
//        headerFilter: {
//            visible: true,
//        },
        //summary: {
        //    totalItems: [{
        //        column: 'code',
        //        summaryType: 'count',
        //    }]
        //},
    }).dxDataGrid('instance');
});


//$(() => {
//    const dataGrid = $('#gridContainer').dxDataGrid({
//        dataSource: ds,
//        keyExpr: 'Type',
//        columns: [
//            'Type',
//            {
//                caption: 'Value Bands',
//                columns: [{
//                    caption: 'Value',
//                    dataField: 'Value',
//                    format: 'fixedPoint',
//                    width: 500,
//                }]
//            },
//            {
//                caption: 'Subject',
//                columns: [{
//                    headerCellTemplate(container) {
//                        container.append($('<div>Area, km<sup>2</sup></div>'));
//                    },
//                    caption: 'Name',
//                    dataField: 'Name',
//                    format: 'fixedPoint'
//                }]
//            },

//        ],
//        showBorders: true,
//        allowColumnReordering: true,
//        allowColumnResizing: true,
//        columnAutoWidth: true,
//        columnChooser: {
//            enabled: true,
//        },
//        columnFixing: {
//            enabled: true,
//        },
//        filterRow: {
//            visible: true,
//            applyFilter: 'auto',
//        },
//        searchPanel: {
//            visible: true,
//            width: 240,
//            placeholder: 'Search...',
//        },
//        headerFilter: {
//            visible: true,
//        },
//        paging: {
//            pageSize: 10,
//        },
//        //scrolling: {
//        //    mode: 'virtual',
//        //    //rowRenderingMode: 'virtual',
//        //},
//        pager: {
//            visible: true,
//            allowedPageSizes: [5, 10, 'all'],
//            showPageSizeSelector: true,
//            showInfo: true,
//            showNavigationButtons: true,
//        },
//        onContentReady(e) {
//            e.component.option('loadPanel.enabled', false);
//        },
//    }).dxDataGrid('instance');
//});
