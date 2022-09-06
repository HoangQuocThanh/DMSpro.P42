$(() => {
    function sendRequest(url, method = 'GET', data) {
        const d = $.Deferred();

        //logRequest(method, url, data);

        $.ajax(url, {
            method,
            data,
            dataType: 'json',
            contentType: 'application/json;charset=UTF-8',
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

            const params = {};

            [
                "filter",
                "group",
                "groupSummary",
                "parentIds",
                "requireGroupCount",
                "requireTotalCount",
                "searchExpr",
                "searchOperation",
                "searchValue",
                "select",
                "sort",
                "skip",
                "take",
                "totalSummary",
                "userData"
            ].forEach(function (i) {
                if (i in loadOptions && isNotEmpty(loadOptions[i])) {
                    params[i] = JSON.stringify(loadOptions[i]);
                }
            });

            $.ajax({
                url: `${url}`,
                dataType: 'json',
                type: 'get',
                //contentType: 'application/json;charset=UTF-8',
                data: params, //JSON.stringify(loadOptions),
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

    var customDataSource = new DevExpress.data.CustomStore({
        key: "id",
        load: function (loadOptions) {
            var d = $.Deferred();
            var params = {};

            [
                "filter",
                "group",
                "groupSummary",
                "parentIds",
                "requireGroupCount",
                "requireTotalCount",
                //"searchExpr",
                //"searchOperation",
                //"searchValue",
                "select",
                "sort",
                "skip",
                "take",
                "totalSummary",
                //"userData"
            ].forEach(function (i) {
                if (i in loadOptions && isNotEmpty(loadOptions[i])) {
                    params[i] = JSON.stringify(loadOptions[i]);
                }
            });

            $.getJSON(url, params)
                .done(function (response) {
                    d.resolve(response.data, {
                        totalCount: response.totalCount,
                        summary: response.summary,
                        groupCount: response.groupCount
                    });
                })
                .fail(function () { throw "Data loading error" });
            return d.promise();
        },
        // Needed to process selected value(s) in the SelectBox, Lookup, Autocomplete, and DropDownBox
        // byKey: function(key) {
        //     var d = new $.Deferred();
        //     $.get('https://mydomain.com/MyDataService?id=' + key)
        //         .done(function(result) {
        //             d.resolve(result);
        //         });
        //     return d.promise();
        // }
    });

    const comsStore = new DevExpress.data.CustomStore({
        key: 'id',
        load(loadOptions) {
            console.log(loadOptions);
            return sendRequest(`${url}/get-all`, 'POST', JSON.stringify(loadOptions));
        },
        insert(values) {
            return sendRequest(`${url}`, 'POST',
                JSON.stringify(values));
        },
        update(key, values) {
            return sendRequest(`${url}/${key}`, 'PUT', JSON.stringify(values));
        },
        remove(key) {
            return sendRequest(`${url}/${key}`, 'DELETE', JSON.stringify(key));
        },
    });

    $('#gridContainer').dxDataGrid({
        dataSource: customDataSource,

        showBorders: true,

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
                visible: false
            },
        ],
        //editing: {
        //    mode: 'row',
        //    allowUpdating: true,
        //    allowDeleting: true,
        //    allowAdding: true,
        //},
        allowColumnReordering: true,
        allowColumnResizing: true,
        columnAutoWidth: true,
        columnChooser: {
            enabled: true,
        },
        columnFixing: {
            enabled: true,
        },
        filterRow: {
            visible: true,
        },
        headerFilter: {
            visible: true,
        },
        
        scrolling: {
            mode: 'virtual',
        },
        height: 600,

        groupPanel: {
            visible: true,
        },
        grouping: {
            autoExpandAll: false,
        },
        //paging: {
        //    pageSize: 2,
        //},
        //sorting: {
        //    mode: 'multiple',
        //},
        //filterRow: {
        //    visible: true,
        //    applyFilter: 'auto',
        //},
        //searchPanel: {
        //    visible: true,
        //    width: 240,
        //    placeholder: 'Search...',
        //},
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
