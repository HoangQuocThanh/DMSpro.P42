var abp = abp || {};

abp.modals.companyCreate = function () {
    var initModal = function (publicApi, args) {
        var l = abp.localization.getResource("MDM");
        
        
        
        
        
        publicApi.onOpen(function () {
            $('#IdentityUserLookup').select2({
                dropdownParent: $('#CompanyCreateModal'),
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

                        return { results: mappedItems };
                    }
                }
            });
        });

        var getNewIdentityUserIndex = function () {
            var idTds = $($(document).find("#IdentityUserTableRows")).find('td[name="id"]');

            if (idTds.length === 0){
                return 0;
            }

            return parseInt($(idTds[idTds.length -1]).attr("index")) +1;
        };

        var getIdentityUserIds = function () {
            var ids = [];
            var idTds = $("#IdentityUserTableRows td[name='id']");

            for(var i = 0; i< idTds.length; i++){
                ids.push(idTds[i].innerHTML.trim())
            }

            return ids;
        };

        $('#AddIdentityUserButton').on('click', '', function(){
            var $select = $('#IdentityUserLookup');
            var id = $select.val();
            var existingIds = getIdentityUserIds();
            if (!id || id === '' || existingIds.indexOf(id) >= 0){
                return;
            }

            $("#IdentityUserTable").show();

            var displayName = $select.find('option').filter(':selected')[0].innerHTML;

            var newIndex = getNewIdentityUserIndex();

            $("#IdentityUserTableRows").append(
                '                                <tr style="text-align: center; vertical-align: middle;" index="'+newIndex+'">\n' +
                '                                    <td style="display: none" name="id" index="'+newIndex+'">'+id+'</td>\n' +
                '                                    <td style="display: none">' +
                '                                        <input value="'+id+'" id="SelectedIdentityUserIds['+newIndex+']" name="SelectedIdentityUserIds['+newIndex+']"/>\n' +
                '                                    </td>\n' +
                '                                    <td style="text-align: left">'+displayName+'</td>\n' +
                '                                    <td style="text-align: right">\n' +
                '                                        <button class="btn btn-danger btn-sm text-light identityUserDeleteButton" index="'+newIndex+'"> <i class="fa fa-trash"></i> </button>\n' +
                '                                    </td>\n' +
                '                                </tr>'
            );
        });

        $(document).on('click', '.identityUserDeleteButton', function (e) {
            e.preventDefault();
            var index = $(this).attr("index");
            $("#IdentityUserTableRows").find('tr[index='+index+']').remove();

            setTimeout(
                function()
                {
                    var rows = $(document).find("#IdentityUserTableRows").find("tr");

                    if (rows.length === 0){
                        $("#IdentityUserTable").hide();
                    }

                    for (var i=0; i<rows.length; i++){
                        $(rows[i]).attr('index', i);
                        $(rows[i]).find('th[scope="Row"]').empty();
                        $(rows[i]).find('th[scope="Row"]').append(i+1);
                        $($(rows[i]).find('td[name="id"]')).attr('index', i);
                        $($(rows[i]).find('input')).attr('id', 'SelectedIdentityUserIds['+i+']');
                        $($(rows[i]).find('input')).attr('name', 'SelectedIdentityUserIds['+i+']');
                        $($(rows[i]).find('button')).attr('index', i);
                    }
                }, 200);
        });
    };

    return {
        initModal: initModal
    };
};
