$(function () {

    function addHandler() {
        $('#dlg').dialog({ title: '新增用户' });
        $('#dlg').dialog('open');
    }
    $(document).on('click', '#submitForm', submitForm);

    function submitForm() {
        $('#ff').form('submit',{
            url: '../User/AddUser',
            onSubmit: function (param) {
                param.userName = $('#userName').val();
                param.roleID = $('#roleID').val();
                param.password = $('#password').val();
                param.status = $('#status').val();
                return $('#ff').form('validate');
            },
            success: function (data) {
                alert("保存成功");
                $('#dlg').dialog('close');
                $('#userList').datagrid("reload");
            }
        });
    }

    $(document).on('click', '#clearForm', clearForm);
    function clearForm() {
        $('#ff').form('clear');
    }

    function getSelections() {
        var ids = [];
        $("input[name='ck']").each(function (index) {
            if (this.checked == true)
                ids.push(parseInt($($(this).parent().parent().next().children().get(0)).html()));
            else {
                var index = -1;
                for (var i = 0; i < ids.length; i++)
                    if (ids[i] == parseInt($($(this).parent().parent().next().children().get(0)).html()))
                        index = i;
                if (index != -1) ids.remove(index);
            }

        });
        return ids.join(',');
    }

    $('#roleList').combobox({
        url: '../UserGroup/GetRoleListById',
        valueField: 'ID',
        textField: 'CNAME'
    });
    $('#roleList').combobox({
        onChange: function (newVal, oldVal) {
            $("#roleID").val(newVal);
        }
    });
    $('#statusSel').change(function () {
        $('#status').val($('#statusSel').val());
    });
    $('#status').val($('#statusSel').val());
    
    $(document).on('click', '.edite', function () {
        var title = $(this).attr('userName');
        $('#dlg').dialog({title:title});
        $('#dlg').dialog('open');
    });

   

    $("#userList").datagrid({
        title: "用户列表",
        loadMsg: "数据加载中，请稍后... ...",
        nowrap: false,
        striped: true,
        //collapsible: true,
        url: "../User/GetUserList",
        pageList: [10, 15, 20, 25, 30, 40, 50],
        pageSize: 15,
        //sortName: "RoleSort",
        sortOrder: "asc",
        remoteSort: false,
        idField: "UserId",
        frozenColumns: [[
            { field: 'ck', checkbox: true },
             { title: '用户编码', field: 'UserId', width: 120, align: 'center', sortable: true }
        ]],
        columns: [[
            { field: 'UserName', title: '用户名称', width: 120, align: 'center', sortable: true },
            { field: 'RoleId', title: '角色编码', width: 120, align: 'center', sortable: true },
            { field: 'RoleName', title: '角色名称', width: 120, align: 'center', sortable: true },
            {
                field: 'Status', title: '状态', width: 120, align: 'center', sortable: true,
                formatter: function (val, rec) {
                    if (val == 1) return "启用";
                    return "禁用";
                }
            },
            { field: 'LastUpdateTime', title: '更新时间', width: 80, align: 'center', sortable: true },
            {
                field: 'opt', title: '操作', width: 100, align: 'center',
                formatter: function (vale, rec) {
                    return '<a href="#" RoleId="' + rec.RoleId+ '" userName="编辑用户 ' + rec.UserName + '" class="edite"><span style="color:red">编辑</span></a>';
                }
            }
        ]],
        pagination: true,
        rownumbers: true,
        onClickRow: function (rowIndex, rowData) {
            $(this).datagrid('unselectRow', rowIndex);
        },
        onLoadSuccess: function () {
            if ($('#txtSearch').length>0) return;

            $('.datagrid-toolbar>table tr:last').append("<td>" +
                 '<input type="text" id="txtSearch" title="请输入用户名称" />' +
                "</td>");
            $('#txtSearch').show();
        },
        toolbar: [
            {
                id: 'btnadd',
                text: '添加',
                iconCls: 'icon-add',
                handler: addHandler
            },
            {
                id: 'btncut',
                text: '删除',
                iconCls: 'icon-cut',
                handler: function () {
                    var codes = getSelections();
                    if (codes == '') {
                        $.messager.alert('提示消息', '请选择要删除的数据！', 'info');
                    } else {
                        $.messager.confirm('提示消息', '确定要删除所选数据吗？', function (r) {
                            if (r) {
                                $('#processWindow').window('open', 'aadasdsads');
                                $.ajax({
                                    url: '../User/DelUser',
                                    type: 'post',
                                    datatype: 'text',
                                    data:{"inputs":codes},
                                    success: function (returnValue) {
                                        var json = returnValue;//$.parseJSON(returnValue);
                                        if (json.IsSuccess) {
                                            $('#processWindow').window('close');
                                            $('#userList').datagrid('reload');
                                            $('#userList').datagrid('clearSelections');
                                        } else alert("错误: " + json.ErrMsg);
                                    }
                                });
                            }
                        });
                    }
                }
            }, '-',
             {
                 id: 'btnSearch',
                 text: '搜索',
                 disabled: false,
                 iconCls: 'icon-search',
                 handler: function () {
                     $('#userList').datagrid('load', {
                         "inputs": $('#txtSearch').val()
                     });
                 }
             }
        ]
    });

});