$(function () {


    gridSite = $('#client_site_settings').grid({
        dataSource: '/Admin/Settings?handler=ClientSites',
        uiLibrary: 'bootstrap4',
        iconsLibrary: 'fontawesome',
        primaryKey: 'id',
        inlineEditing: { mode: 'command' },
        columns: [
            { field: 'typeId', hidden: true },
            { field: 'name', title: 'Client Site', width: 180, editor: true },
            { field: 'emails', title: 'Emails', width: 200, editor: true },
            { field: 'address', title: 'Address', width: 200, editor: addressEditor },
            { field: 'state', title: 'State', width: 80, type: 'dropdown', editor: { dataSource: '/Admin/Settings?handler=ClientStates', valueField: 'name', textField: 'name' } },
            { field: 'billing', title: 'Billing', width: 100, editor: true },
            { field: 'status', title: 'Status', width: 150, renderer: statusTypeRenderer, editor: statusTypeEditor },
            { field: 'statusDate', hidden: true, editor: true }
        ],
        initialized: function (e) {
            $(e.target).find('thead tr th:last').html('<i class="fa fa-cogs" aria-hidden="true"></i>');
        }
    });

    if (gridSite) {
        gridSite.on('rowDataChanged', function (e, id, record) {
            const data = $.extend(true, {}, record);
            data.status = !Number.isInteger(data.status) ? clientSiteStatuses.getValue(data.status) : data.status;
            const token = $('input[name="__RequestVerificationToken"]').val();
            $.ajax({
                url: '/Admin/Settings?handler=ClientSites',
                data: { record: data },
                type: 'POST',
                headers: { 'RequestVerificationToken': token },
            }).done(function () {
                gridSite.clear();
                gridSite.reload({ typeId: $('#sel_client_type').val(), searchTerm: $('#search_kw_client_site').val() });
            }).fail(function () {
                console.log('error');
            }).always(function () {
                if (isClientSiteAdding)
                    isClientSiteAdding = false;
            });
        });

        gridSite.on('rowRemoving', function (e, id, record) {
            const isAdminLoggedIn = $('#hdnIsAdminLoggedIn').val();
            if (isAdminLoggedIn === 'False') {
                showModal('Insufficient permission to perform this operation');
                return;
            }

            if (confirm('Are you sure want to delete this client site?')) {
                const token = $('input[name="__RequestVerificationToken"]').val();
                $.ajax({
                    url: '/Admin/Settings?handler=DeleteClientSite',
                    data: { id: record },
                    type: 'POST',
                    headers: { 'RequestVerificationToken': token },
                }).done(function () {
                    gridSite.reload();
                }).fail(function () {
                    console.log('error');
                }).always(function () {
                    if (isClientSiteAdding)
                        isClientSiteAdding = false;
                });
            }
        });
    }

    let isClientSiteAdding = false;
    $('#add_client_site').on('click', function () {

        if (isClientSiteAdding) {
            alert('Unsaved changes in the grid. Refresh the page');
        } else {
            isClientSiteAdding = true;
            gridSite.addRow({
                'id': -1,
                'name': ''
            }).edit(-1);
        }
    });

    /** Client Site Status & Status Date **/
    const clientSiteStatuses = {
        data: [
            { id: 0, text: 'Ongoing', color: 'text-success' },
            { id: 1, text: 'Expiring', color: 'text-warning' },
            { id: 2, text: 'Expired', color: 'text-danger' }
        ],
        getText: function (value) {
            const item = this.data.find((e) => e.id === value);
            return item ? item.text : '';
        },
        getValue: function (text) {
            const item = this.data.find((e) => e.text === text);
            return item ? item.id : -1;
        },
        getColorByText: function (text) {
            const item = this.data.find((e) => e.text === text);
            return item ? item.color : '';
        },
        getColorByValue: function (value) {
            const item = this.data.find((e) => e.id === value);
            return item ? item.color : '';
        }
    }

    $('#client_site_settings').on('change', '.dd-client-status', function () {
        if (clientSiteStatuses.getValue($(this).find('option:selected').text()) > 0)
            $(this).next('input').show();
        else
            $(this).next('input').hide();
    });

    $('#client_site_settings').on('change', '.dt-client-statusdate', function () {
        $(this).closest('td').next('td').find('input').val($(this).val());

        const recordId = $(this).attr("data-id");
        var data = gridSite.getById(recordId);
        data.statusDate = $(this).val();
    });

    function statusTypeRenderer(value, record) {
        let rendering = false;
        if (Number.isInteger(value)) rendering = true;
        else if (value === undefined) value = 0;
        else {
            value = clientSiteStatuses.getValue(value);
            if (value === -1) value = 0;
        }

        if (value === 1 && record.formattedStatusDate && new Date(record.formattedStatusDate) < new Date())
            value = 2;

        const statusText = clientSiteStatuses.getText(value);
        const color = clientSiteStatuses.getColorByText(statusText);
        let statusDate = '';
        if (value !== 0) {
            if (rendering) {
                statusDate = record.formattedStatusDate;
            } else if (record.statusDate) {
                statusDate = new Intl.DateTimeFormat('en-Au', { year: 'numeric', month: 'short', day: 'numeric' }).format(new Date(record.statusDate));
            }
        }
        return '<div><i class="fa fa-circle ' + color + ' mr-2"></i>' + statusText + '</div><div>' + statusDate + '</div>';
    }

    function statusTypeEditor($editorContainer, value, record) {
        if (isClientSiteAdding && value === undefined)
            value = 0;
        let selectHtml = $('<select class="form-control dd-client-status"></select>');
        clientSiteStatuses.data.forEach(function (item) {
            const selected = item.id === value ? 'selected' : '';
            selectHtml.append('<option "value="' + item.id + '" ' + selected + '>' + item.text + '</option>')
        });
        $editorContainer.append(selectHtml);
        const showDate = value === 0 ? 'display:none' : 'display:block';
        const datePikcer = $('<input type="date" class="dt-client-statusdate form-control" data-id="' + record.id + '" style="' + showDate + '" value="' + record.statusDate.split('T')[0] + '">')
        $editorContainer.append(datePikcer);
    }


    function addressEditor($editorContainer, value, record) {
        const clientAddress = $('<input type="text" id="client_address_' + record.id + '" value="' + value + '">');
        $editorContainer.append(clientAddress);

        const autoComplete = new google.maps.places.Autocomplete(document.getElementById('client_address_' + record.id), {
            types: ['address'],
            componentRestrictions: { country: 'AU' },
            fields: ['place_id', 'geometry', 'name']
        });

        autoComplete.addListener('place_changed', function () {
            const place = this.getPlace();
            if (place.geometry) {
                var lat = place.geometry.location.lat();
                var lon = place.geometry.location.lng();
                document.getElementById('client_gps_' + record.id).value = lat + ',' + lon;
            }
        });
    }


    let gridUsers,
        gridClientSiteAccess,
        ucaTree;
    gridUsers = $('#user_settings').grid({
        dataSource: '/Admin/Settings?handler=Users',
        uiLibrary: 'bootstrap4',
        iconsLibrary: 'fontawesome',
        primaryKey: 'id',
        columns: [
            { field: 'userName', title: 'User Name', width: 200 },
            { title: 'Password', width: 200, renderer: passwordRenderer },
            { field: 'isDeleted', title: 'Deleted?', align: 'center', width: 50, renderer: function (value) { return value ? 'Yes' : '&nbsp;'; } },
            { width: 150, renderer: userButtonRenderer },
        ],
        initialized: function (e) {
            $(e.target).find('thead tr th:last').addClass('text-center').html('<i class="fa fa-cogs" aria-hidden="true"></i>');
        }
    });

    $('#user_settings').on('click', ".showPassword", function () {
        const btn = $(this);
        const userId = btn.attr('data-uid');
        const spanElem = btn.siblings().first();
        $.ajax({
            url: '/Admin/Settings?handler=ShowPassword',
            data: { id: userId },
            type: 'POST',
            headers: { 'RequestVerificationToken': $('input[name="__RequestVerificationToken"]').val() },
        }).done(function (data) {
            spanElem.text(data);
            btn.hide();
        }).fail(function () {
            console.log('error')
        });
    });

    function passwordRenderer(value, record) {
        return '<span id="user_password_' + record.id + '"></span><button class="btn btn-light showPassword" data-uid="' + record.id + '"><i class="fa fa-eye mr-2"></i>Show</button>';
    }

    function userButtonRenderer(value, record) {
        let userButtonHtml = '<button class="btn btn-outline-primary mr-2" data-bs-toggle="modal" data-bs-target="#user-modal" data-id="' + record.id + '" data-uname="' + record.userName + '"' +
            'data-udeleted="' + record.isDeleted + '" data-action="editUser"><i class="fa fa-pencil mr-2"></i>Edit</button>';

        if (record.isDeleted) {
            userButtonHtml += '<button class="btn btn-outline-success activateuser" data-user-id="' + record.id + '""> <i class="fa fa-check mr-2" aria-hidden="true"></i>Activate</button>';
        } else {
            userButtonHtml += '<button class="btn btn-outline-danger deleteuser" data-user-id="' + record.id + '""> <i class="fa fa-trash mr-2" aria-hidden="true"></i>Delete</button>';
        }

        return userButtonHtml;
    }

    $('#add_user').on('click', function () {
        $('#userId').val('');
        $('#userName').val('');
        $('#userName').prop('readonly', false);
        $('#userPassword').val('');
        $('#userConfirmPassword').val('');
        $('#user-modal').modal();
    });

    $('#user-modal').on('shown.bs.modal', function (event) {
        const button = $(event.relatedTarget);
        if (button.data('action') !== undefined &&
            button.data('action') === 'editUser') {
            $('#userId').val(button.data('id'));
            $('#userName').val(button.data('uname'))
            $('#userName').prop('readonly', true);
            $('#userPassword').val('');
            $('#userConfirmPassword').val('');
        }
    });

    function newUserIsValid() {
        const userName = $('#userName').val();
        const password = $('#userPassword').val();
        const confirmPassword = $('#userConfirmPassword').val();

        if (userName === '' || password === '' || confirmPassword === '') {
            $('#user-modal-validation').html('All fields are required').show().delay(2000).fadeOut();
            return false;
        }

        if (password !== confirmPassword) {
            $('#user-modal-validation').html('Passwords not matching').show().delay(2000).fadeOut();
            return false;
        }

        return true;
    }

    $('#btnSaveUser').on('click', function () {
        if (newUserIsValid()) {
            var data = {
                'id': $('#userId').val(),
                'userName': $('#userName').val(),
                'password': $('#userPassword').val()
            };
            $.ajax({
                url: '/Admin/Settings?handler=User',
                data: { record: data },
                type: 'POST',
                headers: { 'RequestVerificationToken': $('input[name="__RequestVerificationToken"]').val() },
            }).done(function (data) {
                if (!data.status) {
                    $('#user-modal-validation').html(data.message).show().delay(2000).fadeOut();
                } else {
                    $('#user-modal').modal('hide');
                    gridUsers.reload();
                    gridClientSiteAccess.reload();
                    showStatusNotification(true, 'Saved successfully');
                }
            }).fail(function () {
                console.log('error');
            }).always(function () {
            });
        }
    });


    $('#user_settings').on('click', '.deleteuser', function () {
        if (confirm('Are you sure want to delete this user?')) {
            toggleUserStatus($(this).attr('data-user-id'), true);
        }
    });

    $('#user_settings').on('click', '.activateuser', function () {
        toggleUserStatus($(this).attr('data-user-id'), false);
    });

    function toggleUserStatus(userId, deleted) {
        $.ajax({
            url: '/Admin/Settings?handler=UpdateUserStatus',
            data: { id: userId, deleted: deleted },
            type: 'POST',
            headers: { 'RequestVerificationToken': $('input[name="__RequestVerificationToken"]').val() },
        }).done(function () {
            gridUsers.reload();
            gridClientSiteAccess.reload();
        }).fail(function () {
            console.log('error')
        });
    }

    gridClientSiteAccess = $('#user_client_access').grid({
        dataSource: '/Admin/Settings?handler=UserClientAccess',
        uiLibrary: 'bootstrap4',
        iconsLibrary: 'fontawesome',
        primaryKey: 'id',
        columns: [
            { field: 'userName', title: 'User', width: 150 },
            { field: 'clientTypeCsv', title: 'Client Type Access', width: 250 },
            { field: 'clientSiteCsv', title: 'Client Site Access', width: 250 },
            { width: 100, tmpl: '<button class="btn btn-outline-primary" data-toggle="modal" data-target="#user-client-access-modal" data-id="{id}"><i class="fa fa-pencil mr-2"></i>Edit</button>', align: 'center' },
        ],
        initialized: function (e) {
            $(e.target).find('thead tr th:last').html('<i class="fa fa-cogs" aria-hidden="true"></i>');
        }
    });

    $('#user-client-access-modal').on('shown.bs.modal', function (event) {
        const button = $(event.relatedTarget);
        const userId = button.data('id');
        $(this).find('#user-access-for-id').val(userId);
        if (ucaTree === undefined) {
            ucaTree = $('#ucaTreeView').tree({
                uiLibrary: 'bootstrap4',
                checkboxes: true,
                primaryKey: 'id',
                dataSource: '/Admin/Settings?handler=ClientAccessByUserId',
                autoLoad: false,
                textField: 'name',
                childrenField: 'clientSites',
                checkedField: 'checked'
            });
        }
        ucaTree.uncheckAll();
        ucaTree.reload({ userId: userId });
    });

    $('#btnSaveUserAccess').on('click', function () {
        if (ucaTree) {
            const userId = $('#user-access-for-id').val();
            let selectedSites = ucaTree.getCheckedNodes().filter(function (item) {
                return item !== 'undefined';
            });
            $.ajax({
                url: '/Admin/Settings?handler=ClientAccessByUserId',
                data: {
                    userId: userId,
                    selectedSites: selectedSites
                },
                type: 'POST',
                headers: { 'RequestVerificationToken': $('input[name="__RequestVerificationToken"]').val() },
            }).done(function () {
                gridClientSiteAccess.reload();
                showStatusNotification(true, 'Saved successfully');
            }).fail(function () {
                console.log('error');
            });
        }
    });

    $('#grantAllUserAccess').on('click', function () {
        if (ucaTree !== undefined) {
            ucaTree.checkAll();
        }
    });

    $('#revokeAllUserAccess').on('click', function () {
        if (ucaTree !== undefined && confirm('Are you sure want to revoke all access?')) {
            ucaTree.uncheckAll();
        }
    });

    $('#expandAllUserAccess').on('click', function () {
        if (ucaTree !== undefined) {
            ucaTree.expandAll();
        }
    });

    $('#collapseAllUserAccess').on('click', function () {
        if (ucaTree !== undefined) {
            ucaTree.collapseAll();
        }
    });






});

/* V2 Changes start 12102023 */
let Dttbl_moduleOneData = $('#Dttbl_moduleOneData').DataTable({
    lengthMenu: [[10, 25, 50, 100, 1000], [10, 25, 50, 100, 1000]],
    paging: true,
    ordering: true,
    fixedHeader: true,
    order: [[1, "asc"]],
    info: false,
    searching: true,
    autoWidth: false,
    ajax: {
        url: '/ModuleOne?handler=ModuleOneData',
        dataSrc: ''
    },
    columns: [
        { data: 'id', visible: false },
        { data: 'fieldOne', width: '4%' },
        { data: 'fieldTwo', width: '12%', orderable: false },
        {
            targets: -1,
            orderable: false,
            width: '4%',
            data: null,
            defaultContent: '<button  class="btn btn-outline-primary mr-2" id="btn_edit_cs_key"><i class="fa fa-pencil mr-2"></i>Edit</button>' +
                '<button id="btn_delete_cs_key" class="btn btn-outline-danger mr-2 mt-1"><i class="fa fa-trash mr-2"></i>Delete</button>',
            className: "text-center"
        },
    ],
});
$('#add_ModuleOneData').on('click', function () {
    resetModuleOneModal();
    $('#module-One-data-modal').modal('show');
});

function resetModuleOneModal() {
    $('#ModuleOne_Id').val('');
    $('#ModuleOne_FieldOne').val('');
    $('#ModuleOne_FieldTwo').val('');
    $('#csKeyValidationSummary').html('');
    $('#module-One-data-modal').modal('hide');
}

$('#btn_save_cs_key').on('click', function () {
    var test = $('#frm_add_key').serialize();
    var check = test;
    $.ajax({
        url: '/ModuleOne?handler=SaveModuleOneData',
        data: $('#frm_add_key').serialize(),
        type: 'POST',
        headers: { 'RequestVerificationToken': $('input[name="__RequestVerificationToken"]').val() },
    }).done(function (result) {
        if (result.success) {
            resetModuleOneModal();
            Dttbl_moduleOneData.ajax.reload();
        } else {
            displaySiteKeyValidationSummary(result.message);
        }
    });
});

function displaySiteKeyValidationSummary(errors) {
    $('#csKeyValidationSummary').removeClass('validation-summary-valid').addClass('validation-summary-errors');
    $('#csKeyValidationSummary').html('');
    $('#csKeyValidationSummary').append('<ul></ul>');
    if (!Array.isArray(errors)) {
        $('#csKeyValidationSummary ul').append('<li>' + errors + '</li>');
    } else {
        errors.forEach(function (item) {
            if (item.indexOf(',') > 0) {
                item.split(',').forEach(function (itemInner) {
                    $('#csKeyValidationSummary ul').append('<li>' + itemInner + '</li>');
                });
            } else {
                $('#csKeyValidationSummary ul').append('<li>' + item + '</li>');
            }
        });
    }
}

$('#Dttbl_moduleOneData tbody').on('click', '#btn_delete_cs_key', function () {
    var data = Dttbl_moduleOneData.row($(this).parents('tr')).data();
    if (confirm('Are you sure want to delete this data?')) {
        $.ajax({
            type: 'POST',
            url: '/ModuleOne?handler=DeleteModuleOneDetails',
            data: { 'id': data.id },
            dataType: 'json',
            headers: { 'RequestVerificationToken': $('input[name="__RequestVerificationToken"]').val() },
        }).done(function () {
            Dttbl_moduleOneData.ajax.reload();
        });
    }
});

$('#Dttbl_moduleOneData tbody').on('click', '#btn_edit_cs_key', function () {
    var data = Dttbl_moduleOneData.row($(this).parents('tr')).data();
    loadmoduleOneDataModal(data);
});

function loadmoduleOneDataModal(data) {
    $('#ModuleOne_Id').val(data.id);
    $('#ModuleOne_FieldOne').val(data.fieldOne);
    $('#ModuleOne_FieldTwo').val(data.fieldTwo);
    $('#csKeyValidationSummary').html('');
    $('#module-One-data-modal').modal('show');
}
/*gridDepartmentMaster Start*/
let gridDepartmentMaster;

gridDepartmentMaster = $('#Department_settings').grid({
    dataSource: '/Admin/Settings?handler=DepartmentDetails',
    uiLibrary: 'bootstrap4',
    iconsLibrary: 'fontawesome',
    primaryKey: 'id',
    inlineEditing: { mode: 'command' },
    columns: [
        { field: 'departmentName', title: 'Department Name', width: 200, editor: true },
        { field: 'departmentDescription', title: 'Department Description', width: 350, editor: true }

    ],
    initialized: function (e) {
        $(e.target).find('thead tr th:last').html('<i class="fa fa-cogs" aria-hidden="true"></i>');
    }
});




let isdepartmentAdding = false;
$('#add_department_settings').on('click', function () {


    if (isdepartmentAdding) {
        alert('Unsaved changes in the grid. Refresh the page');
    } else {
        isdepartmentAdding = true;
        gridDepartmentMaster.addRow({
            'id': -1,
            'departmentName': '',
            'departmentDescription': ''
        }).edit(-1);
    }
});


if (gridDepartmentMaster) {
    gridDepartmentMaster.on('rowDataChanged', function (e, id, record) {
        const data = $.extend(true, {}, record);
        const token = $('input[name="__RequestVerificationToken"]').val();
        $.ajax({
            url: '/Admin/Settings?handler=DepartmentDetailsUpdate',
            data: { department: data },
            type: 'POST',
            headers: { 'RequestVerificationToken': token },
        }).done(function (response) {
            if (!response.status) {
                alert(response.message);
            }
            gridDepartmentMaster.clear();
            gridDepartmentMaster.reload();
        }).fail(function () {
            console.log('error');
        }).always(function () {
            if (isdepartmentAdding)
                isdepartmentAdding = false;
        });
    });

    gridDepartmentMaster.on('rowRemoving', function (e, id, record) {

        if (confirm('Are you sure want to delete this Department?')) {
            const token = $('input[name="__RequestVerificationToken"]').val();
            $.ajax({
                url: '/Admin/Settings?handler=DeleteDepartmentDetails',
                data: { id: record },
                type: 'POST',
                headers: { 'RequestVerificationToken': token },
            }).done(function () {
                gridDepartmentMaster.reload();
            }).fail(function () {
                console.log('error');
            }).always(function () {
                if (isdepartmentAdding)
                    isdepartmentAdding = false;
            });
        }
    });
}
/*gridDepartmentMaster end*/

/*gridApplicationMaster Start*/
let gridApplicationMaster;

gridApplicationMaster = $('#Application_settings').grid({
    dataSource: '/Admin/Settings?handler=ApplicationDetails',
    uiLibrary: 'bootstrap4',
    iconsLibrary: 'fontawesome',
    primaryKey: 'id',
    inlineEditing: { mode: 'command' },
    columns: [
        { field: 'applicationName', title: 'Application Name', width: 200, editor: true },
        { field: 'applicationDescription', title: 'Application Description', width: 350, editor: true }

    ],
    initialized: function (e) {
        $(e.target).find('thead tr th:last').html('<i class="fa fa-cogs" aria-hidden="true"></i>');
    }
});




let isapplicationAdding = false;
$('#add_application_settings').on('click', function () {


    if (isapplicationAdding) {
        alert('Unsaved changes in the grid. Refresh the page');
    } else {
        isapplicationAdding = true;
        gridApplicationMaster.addRow({
            'id': -1,
            'departmentName': '',
            'departmentDescription': ''
        }).edit(-1);
    }
});


if (gridApplicationMaster) {
    gridApplicationMaster.on('rowDataChanged', function (e, id, record) {
        const data = $.extend(true, {}, record);
        const token = $('input[name="__RequestVerificationToken"]').val();
        $.ajax({
            url: '/Admin/Settings?handler=ApplicationDetailsUpdate',
            data: { application: data },
            type: 'POST',
            headers: { 'RequestVerificationToken': token },
        }).done(function (response) {
            if (!response.status) {
                alert(response.message);
            }
            gridApplicationMaster.clear();
            gridApplicationMaster.reload();
        }).fail(function () {
            console.log('error');
        }).always(function () {
            if (isapplicationAdding)
                isapplicationAdding = false;
        });
    });

    gridApplicationMaster.on('rowRemoving', function (e, id, record) {

        if (confirm('Are you sure want to delete this Application?')) {
            const token = $('input[name="__RequestVerificationToken"]').val();
            $.ajax({
                url: '/Admin/Settings?handler=DeleteApplicationDetails',
                data: { id: record },
                type: 'POST',
                headers: { 'RequestVerificationToken': token },
            }).done(function () {
                gridApplicationMaster.reload();
            }).fail(function () {
                console.log('error');
            }).always(function () {
                if (isapplicationAdding)
                    isapplicationAdding = false;
            });
        }
    });
}
/*gridApplicationMaster end*/

/*gridSiteMaster Start*/
let gridSiteMaster;

gridSiteMaster = $('#Site_settings').grid({
    dataSource: '/Admin/Settings?handler=SiteDetails',
    uiLibrary: 'bootstrap4',
    iconsLibrary: 'fontawesome',
    primaryKey: 'id',
    inlineEditing: { mode: 'command' },
    columns: [
        { field: 'siteName', title: 'Site Name', width: 200, editor: true },
        { field: 'siteAddress', title: 'Site Address', width: 350, editor: true }

    ],
    initialized: function (e) {
        $(e.target).find('thead tr th:last').html('<i class="fa fa-cogs" aria-hidden="true"></i>');
    }
});




let issiteAdding = false;
$('#add_site_settings').on('click', function () {


    if (issiteAdding) {
        alert('Unsaved changes in the grid. Refresh the page');
    } else {
        issiteAdding = true;
        gridSiteMaster.addRow({
            'id': -1,
            'departmentName': '',
            'departmentDescription': ''
        }).edit(-1);
    }
});


if (gridSiteMaster) {
    gridSiteMaster.on('rowDataChanged', function (e, id, record) {
        const data = $.extend(true, {}, record);
        const token = $('input[name="__RequestVerificationToken"]').val();
        $.ajax({
            url: '/Admin/Settings?handler=SiteDetailsUpdate',
            data: { site: data },
            type: 'POST',
            headers: { 'RequestVerificationToken': token },
        }).done(function (response) {
            if (!response.status) {
                alert(response.message);
            }
            gridSiteMaster.clear();
            gridSiteMaster.reload();
        }).fail(function () {
            console.log('error');
        }).always(function () {
            if (issiteAdding)
                issiteAdding = false;
        });
    });

    gridSiteMaster.on('rowRemoving', function (e, id, record) {

        if (confirm('Are you sure want to delete this Site?')) {
            const token = $('input[name="__RequestVerificationToken"]').val();
            $.ajax({
                url: '/Admin/Settings?handler=DeleteSiteDetails',
                data: { id: record },
                type: 'POST',
                headers: { 'RequestVerificationToken': token },
            }).done(function () {
                gridSiteMaster.reload();
            }).fail(function () {
                console.log('error');
            }).always(function () {
                if (issiteAdding)
                    issiteAdding = false;
            });
        }
    });
}
/*gridSiteMaster end*/

/* Fields Start */

let gridFields;

gridFields = $('#fields_settings').grid({
    dataSource: '/Admin/Settings?handler=FieldDetails',
    uiLibrary: 'bootstrap4',
    iconsLibrary: 'fontawesome',
    primaryKey: 'id',
    inlineEditing: { mode: 'command' },
    columns: [
        { field: 'fieldName', title: 'Field Item Name', width: 200, editor: true }


    ],
    initialized: function (e) {
        $(e.target).find('thead tr th:last').html('<i class="fa fa-cogs" aria-hidden="true"></i>');
    }
});




let isReportTooolsAdding = false;
$('#add_fields_settings').on('click', function () {
    const selToolsTypeId = $('#report_fields_types').val();
    if (!selToolsTypeId) {
        alert('Please select a field type to update');
        return;
    }

    if (isReportTooolsAdding) {
        alert('Unsaved changes in the grid. Refresh the page');
    } else {
        isReportTooolsAdding = true;
        gridFields.addRow({
            'id': -1,
            'fieldTypeMasterId': selToolsTypeId,
            'fieldName': ''
           
        }).edit(-1);
    }
});

$('#report_fields_types').on('change', function () {
    const selToolsTypeId = $(this).val();

    if (!selToolsTypeId) { // None
        $('#filedsSettings').show();
        gridFields.clear();
        gridFields.reload({ typeId: selToolsTypeId });

    } else {
        $('#filedsSettings').show();
        gridFields.clear();
        gridFields.reload({ typeId: selToolsTypeId });
    }
});

if (gridFields) {
    gridFields.on('rowDataChanged', function (e, id, record) {
        const data = $.extend(true, {}, record);
        const token = $('input[name="__RequestVerificationToken"]').val();
        $.ajax({
            url: '/Admin/Settings?handler=FieldDetails',
            data: { reportfield: data },
            type: 'POST',
            headers: { 'RequestVerificationToken': token },
        }).done(function (response) {
            if (!response.status) {
                alert(response.message);
            }
            gridFields.clear();
            gridFields.reload({ typeId: $('#report_fields_types').val() });
        }).fail(function () {
            console.log('error');
        }).always(function () {
            if (isReportTooolsAdding)
                isReportTooolsAdding = false;
        });
    });

    gridFields.on('rowRemoving', function (e, id, record) {

        if (confirm('Are you sure want to delete this field?')) {
            const token = $('input[name="__RequestVerificationToken"]').val();
            $.ajax({
                url: '/Admin/Settings?handler=DeleteFieldDetails',
                data: { id: record },
                type: 'POST',
                headers: { 'RequestVerificationToken': token },
            }).done(function () {
                gridFields.reload({ typeId: $('#report_fields_types').val() });
            }).fail(function () {
                console.log('error');
            }).always(function () {
                if (isReportTooolsAdding)
                    isReportTooolsAdding = false;
            });
        }
    });
}

$('#add_fields_page').on('click', function () {
    $('#pageType').val('');
    $('#fields-modal').modal();
});



$('#btnSavePageType').on('click', function () {
    if (newpageTypeIsValid()) {
        var newItem = $("#pageType").val();
        var data = {
            'FieldTypeName': $('#pageType').val()
        };
        $.ajax({
            url: '/Admin/Settings?handler=FieldType',
            data: { ClientSiteLinksPageTyperecord: data },
            type: 'POST',
            headers: { 'RequestVerificationToken': $('input[name="__RequestVerificationToken"]').val() },
        }).done(function (data) {
            if (data.status == -1) {
                $('#pageType').val('');
                $('#pageType-modal-validation').html(data.message).show().delay(2000).fadeOut();
            } else {

                const button_id = 'attach_' + data.status;
                const li = document.createElement('li');
                li.id = button_id;
                li.className = 'list-group-item';
                li.dataset.index = data.status;
                li.style = "border-left: 0;border-right: 0;"
                let liText = document.createTextNode(newItem);

                const icon = document.createElement("i");
                icon.className = 'fa fa-trash-o ml-2 text-danger btn-delete-fields-type';
                icon.title = 'Delete';
                icon.style = 'cursor: pointer;float:right';

                li.appendChild(liText);
                li.appendChild(icon);
                document.getElementById('itemList_fields').append(li);

                $("#itemInput").val("");
                // Append the new item to the list

                $('#pageType').val('');
                refreshPageType();

            }
        }).fail(function () {
            console.log('error');
        }).always(function () {
        });
    }
});
function newpageTypeIsValid() {
    const pageType = $('#pageType').val();
    if (pageType === '') {
        $('#pageType-modal-validation').html('Button name is required').show().delay(2000).fadeOut();
        return false;
    }
    return true;
}



const refreshPageType = function () {
    $.ajax({
        url: '/Admin/Settings?handler=FieldTypeList',
        type: 'GET',
        success: function (data) {
            if (data) {
                $('#report_fields_types').html('');
                $('#report_fields_types').append('<option value="">None</option>');
                data.map(function (template) {
                    $('#report_fields_types').append('<option value="' + template.id + '">' + template.fieldTypeName + '</option>');
                });
            }
        }
    });
}

$('#itemList_fields').on('click', '.btn-delete-fields-type', function (event) {
    if (confirm('Are you sure want to delete this Category ?')) {
        var target = event.target;
        const fileName = target.parentNode.innerText.trim();
        var itemToDelete = target.parentNode.dataset.index;
        $.ajax({
            url: '/Admin/Settings?handler=DeleteFieldType',
            type: 'POST',
            dataType: 'json',
            data: { TypeId: itemToDelete },
            headers: { 'RequestVerificationToken': $('input[name="__RequestVerificationToken"]').val() },
        }).done(function (result) {
            if (result) {
                $("#itemInput").val("");
                refreshPageType();
                target.parentNode.parentNode.removeChild(target.parentNode);

            }
        });
    }
});

/* Fields end */



