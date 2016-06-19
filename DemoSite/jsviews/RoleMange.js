$(function () {

    function addHandler() {
        $('#dlg').dialog('open');

    }
    $(document).on('click', '#submitForm', submitForm);

    function submitForm() {
        $('#ff').form('submit',{
            url: '../UserGroup/AddRole',
            onSubmit: function (param) {
                param.roleName = $('#roleName').val();
                param.roleDescri = $('#roleDescri').val();
                return $('#ff').form('validate');
            },
            success: function (data) {
                data = $.parseJSON(data);
                if (data.IsSuccess){
                    $('#dlg').dialog('close');
                    $('#roleroleList').datagrid("reload");
                }
                   
                else alert("保存失败: " + data.ErrMsg);
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
                var index=-1;
                for (var i = 0; i < ids.length; i++)
                    if (ids[i] == parseInt($($(this).parent().parent().next().children().get(0)).html()))
                        index = i;
                if(index!=-1) ids.remove(index);
            }
               
        });
        return ids.join(',');
    }

    $(document).on('click', '.edite', function () {
        var title = $(this).attr('roleName');
        $('#dlg').dialog({title:title});
        $('#dlg').dialog('open');
    });
    $("#roleroleList").datagrid({
        title: "角色列表",
        loadMsg: "数据加载中，请稍后... ...",
        nowrap: false,
        striped: true,
        //collapsible: true,
        url: "../UserGroup/GetRoleList",
        pageList: [10, 15, 20, 25, 30, 40, 50],
        pageSize: 15,
        //sortName: "RoleSort",
        sortOrder: "asc",
        remoteSort: false,
        idField: "RoleId",
        frozenColumns: [[
            { field: 'ck', checkbox: true },
             { title: '角色编码', field: 'RoleID', width: 120, align: 'center', sortable: true }
        ]],
        columns: [[
            { field: 'RoleName', title: '角色名称', width: 120, align: 'center', sortable: true },
            { field: 'Description', title: '描述', width: 80, align: 'center' },
            { field: 'LastUpdateTime', title: '更新时间', width: 80, align: 'center', sortable: true },
            {
                field: 'opt', title: '操作', width: 100, align: 'center',
                formatter: function (vale, rec) {
                    return '<a href="#" roleName="编辑角色 '+rec.RoleName +'" class="edite"><span style="color:red">编辑</span></a>';
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
                 '<input type="text" id="txtSearch" title="请输入角色名称" />' +
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
                                    url: '../UserGroup/DelRole',
                                    type: 'post',
                                    datatype: 'text',
                                    data: { "inputs": codes },
                                    success: function (returnValue) {
                                        if (returnValue) {
                                            $('#processWindow').window('close');
                                            $('#roleroleList').datagrid('reload');
                                            $('#roleroleList').datagrid('clearSelections');
                                        }
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
                     $('#roleroleList').datagrid('load', {
                         "inputs": $('#txtSearch').val()
                     });
                        
                     //$('#roleroleList').datagrid("reload");
                 }
             }
        ]
    });

});