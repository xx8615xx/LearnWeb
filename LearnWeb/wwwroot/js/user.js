var dataTable;
$(document).ready(function () {
    loadDataTable();
});

function loadDataTable() {
    dataTable = $('#tblData').DataTable({
        "ajax": { url: '/admin/user/getall'},
        "columns": [
            { data: 'name', "width":"15%" },
            { data: 'email', "width": "25%" },
            { data: 'phoneNumber', "width": "10%" },
            { data: 'company.name', "width": "15%" },
            { data: 'role', "defaultContent": "" ,"width":"10%"},
            {
                data: {id:'id', lockoutEnd:'lockoutEnd'},
                "render": function (data) {
                    var today = new Date().getTime();
                    var lockout = new Date(data.lockoutEnd).getTime();
                    if (lockout > today) {
                        return `
                         <div class="text-center">
                            <a onclick=LockAndUnlock('${data.id}') class="btn btn-danger text-white" style="cursor:pointer; width:100px;">
                                <i class="bi bi-unlock-fill"></i>Lock
                            </a>
                            <a href="/admin/user/RoleManagement?userID=${data.id}" class="btn btn-danger text-white" style="cursor:pointer; width:150px;">
                                <i class="bi bi-pencil-fill"></i>Permission
                            </a>
                         </div>`
                    }
                    else {
                        return `
                         <div class="text-center">
                            <a onclick=LockAndUnlock('${data.id}') class="btn btn-success text-white" style="cursor:pointer; width:100px;">
                                <i class="bi bi-unlock-fill"></i>Unlock
                            </a>
                            <a href="/admin/user/RoleManagement?userID=${data.id}" class="btn btn-danger text-white" style="cursor:pointer; width:150px;">
                                <i class="bi bi-pencil-fill"></i>Permission
                            </a>
                         </div>`
                    }
                },
                width:"25%"
            }
        ]
    });
}

function LockAndUnlock(id) {
    $.ajax({
        type: "POST",
        url: '/Admin/User/LockAndUnlock',
        data: JSON.stringify(id),
        contentType: "application/json",
        success: function (data) {
            if (data.success) {
                dataTable.ajax.reload();
                //toastr.success(data.message);
            }
        }
    });
}
