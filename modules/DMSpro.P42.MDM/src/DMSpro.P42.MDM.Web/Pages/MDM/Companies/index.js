$(function () {
    var l = abp.localization.getResource("MDM");
	
	var companyService = window.dMSpro.p42.mDM.companies.company;
	
	
    var createModal = new abp.ModalManager({
        viewUrl: abp.appPath + "MDM/Companies/CreateModal",
        scriptUrl: "/Pages/MDM/Companies/createModal.js",
        modalClass: "companyCreate"
    });

	var editModal = new abp.ModalManager({
        viewUrl: abp.appPath + "MDM/Companies/EditModal",
        scriptUrl: "/Pages/MDM/Companies/editModal.js",
        modalClass: "companyEdit"
    });

	var getFilter = function() {
        return {
            filterText: $("#FilterText").val(),
            code: $("#CodeFilter").val(),
			name: $("#NameFilter").val(),
			address1: $("#Address1Filter").val(),
			identityUserId: $("#IdentityUserFilter").val()
        };
    };

    var dataTable = $("#CompaniesTable").DataTable(abp.libs.datatables.normalizeConfiguration({
        processing: true,
        serverSide: true,
        paging: true,
        searching: false,
        scrollX: true,
        autoWidth: false,
        scrollCollapse: true,
        order: [[1, "asc"],[2, "asc"]],
        ajax: abp.libs.datatables.createAjax(companyService.getList, getFilter),
        columnDefs: [
            {
                rowAction: {
                    items:
                        [
                            {
                                text: l("Edit"),
                                visible: abp.auth.isGranted('MDM.Companies.Edit'),
                                action: function (data) {
                                    editModal.open({
                                     id: data.record.company.id
                                     });
                                }
                            },
                            {
                                text: l("Delete"),
                                visible: abp.auth.isGranted('MDM.Companies.Delete'),
                                confirmMessage: function () {
                                    return l("DeleteConfirmationMessage");
                                },
                                action: function (data) {
                                    companyService.delete(data.record.company.id)
                                        .then(function () {
                                            abp.notify.info(l("SuccessfullyDeleted"));
                                            dataTable.ajax.reload();
                                        });
                                }
                            }
                        ]
                }
            },
			{ data: "company.code" },
			{ data: "company.name" },
			{ data: "company.address1" }
        ]
    }));

    createModal.onResult(function () {
        dataTable.ajax.reload();
    });

    editModal.onResult(function () {
        dataTable.ajax.reload();
    });

    $("#NewCompanyButton").click(function (e) {
        e.preventDefault();
        createModal.open();
    });

	$("#SearchForm").submit(function (e) {
        e.preventDefault();
        dataTable.ajax.reload();
    });

    $('#AdvancedFilterSectionToggler').on('click', function (e) {
        $('#AdvancedFilterSection').toggle();
    });

    $('#AdvancedFilterSection').on('keypress', function (e) {
        if (e.which === 13) {
            dataTable.ajax.reload();
        }
    });

    $('#AdvancedFilterSection select').change(function() {
        dataTable.ajax.reload();
    });
    
                $('#IdentityUserFilter').select2({
                ajax: {
                    url: abp.appPath + 'api/mdm/companies/identity-user-lookup',
                    type: 'GET',
                    data: function (params) {
                        return { filter: params.term, maxResultCount: 10 }
                    },
                    processResults: function (data) {
                        var mappedItems = _.map(data.items, function (item) {
                            return { id: item.id, text: item.displayName };
                        });
                        mappedItems.unshift({ id: "", text: ' - ' });

                        return { results: mappedItems };
                    }
                }
            });
        
});
