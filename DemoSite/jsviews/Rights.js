$(function () {
    $('#tree').tree({
        checkbox: true,
        url: '../Right/GetMenuTree',
        onLoadSuccess: function () {
            var root = $('#tree').tree("getRoot");
            console.log(root.id)
            $('#tree').tree('select', root.target);
        },
        onSelect: function (node) {
            $("#pageId").val(node.id);
            $("#operList").datagrid('reload', { data: $("#pageId").val() });
        }
    });
    
    $('#roleList').combobox({
        url: '../UserGroup/GetRoleListById',
        valueField: 'ID',
        textField: 'CNAME',
        onChange: function (newVal, oldVal) {
            $("#roleID").val(newVal);
            $.ajax({
                url: '../Right/GetRightsByRoleId',
                dataType: "json",
                type: 'POST',
                data: { "inputs": JSON.stringify(newVal) },
                success: function (json) {
                    if (json.ActionResult.IsSuccess) {
                        var array = json.Data;
                        var nodes = $('#tree').tree('getChecked');
                        for (var i = 0; i < nodes.length; i++) {
                            $('#tree').tree('uncheck', nodes[i].target);
                        }
                        for (var i = 0; i < array.length; i++) {
                            var node = $('#tree').tree("find", array[i].MenuRights);
                            $('#tree').tree('check', node.target);
                        }
                    } else {

                        alert("读取失败：" + json.ErrMsg);
                    }
                },
                error: function () {

                }
            });
        }
    });
    $(document).on('click', '#saveBtn', function () {
        var data = {};
        data.Data = GetTreeSel();
        if (data.Data.length == 0) {
            alert('请选择节点');
            return;
        }
        data.RoleId = $('#roleID').val();
        if (data.RoleId == "") {
            alert('请选择角色');
            return;
        }
        data.Type = 0;
        $.ajax({
            url: '../Right/ModifyRights',
            dataType: "json",
            type:'POST',
            data: { "inputs": JSON.stringify(data) },
            success: function (json) {
                if (json.IsSuccess) {
                    alert('保存成功');
                } else {
                    alert("保存失败："+json.ErrMsg);
                }
            },
            error: function () {

            }
        });
    });

    $("#operList").datagrid({
        title: '操作列表  <input type="button" id="saveBtnOper" class="save2" value="保存" />',
        loadMsg: "数据加载中，请稍后...",
        nowrap: false,
        striped: true,
        //singleSelect: true,
        url: '../Right/GetOperListByMenu' ,
        queryParams: { data:$("#pageId").val()},
        pageList: [10, 15, 20, 25, 30, 40, 50],
        pageSize: 15,
        remoteSort: false,
        frozenColumns: [[
           { field: 'ck', checkbox: true },
            { title: '操作编码', field: 'OperId', width: 120, align: 'center'}
        ]],
        columns: [[
            { field: 'OperName', title: '操作名称', width: 120, align: 'center'}
        ]],
        pagination: true,
        rownumbers: true,
        onClickRow: function (rowIndex, rowData) {
            $(this).datagrid('unselectRow', rowIndex);
        },
        onSelect: function (index, row) {
          
        }
    });
   
    $(document).on('click', "#saveBtnOper", function () {
        var ids = getSelections();
        if (ids == "") {
            alert("无选择项");
            return;
        }
        var roleId = $('#roleList').combobox('getValue')
        var page = $('#tree').tree('getSelected')
        if (roleId == "" || page == null) {
            alert('选择角色或菜单');
            return;
        }
        var data = { "ids": ids, "roleId": roleId, "pageId": page.id };
        $.ajax({
            type: "POST",
            url: "../right/ModifyOperRights",
            dataType: "json",
            data: { "inputs": JSON.stringify(data) },
            success: function (json) {
                if (json.IsSuccess) {
                    alert('保存成功');
                    $("#operList").datagrid('reload');
                } else {
                    alert("错误: "+json.ErrMsg);
                }
            },
            error: function () {

            }
        });
    });
   
    function GetTreeSel() {
        var nodes = $('#tree').tree('getChecked');
        var s = [];
        for (var i = 0; i < nodes.length; i++) {

            s.push(nodes[i].id);
        }
        return s;
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

});