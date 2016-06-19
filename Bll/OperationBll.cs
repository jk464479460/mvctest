using FrameWrok.Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Bll
{
    public class OperationBll
    {
        public List<TB_USERGROUP> GetRoleList(int pageSize, int curPage, out int cnt, string whereStr)
        {
            try
            {
                if (string.IsNullOrEmpty(whereStr))
                {
                    var list = (from d in new DatabaseContext().Roles orderby d.ID select d).Skip((curPage - 1) * pageSize).Take(pageSize).ToList();
                    cnt = new DatabaseContext().Roles.Count();
                    return list;
                }
                var list2 = (from d in new DatabaseContext().Roles where d.CNAME.Contains(whereStr) orderby d.ID select d).Skip((curPage - 1) * pageSize).Take(pageSize).ToList();
                cnt = new DatabaseContext().Roles.Count(x => x.CNAME.Contains(whereStr));
                return list2;

            }
            catch (Exception ex)
            {
                cnt = 0;
                return null;
            }

        }
        public List<TB_USERGROUP> GetRoleById(int? roleId)
        {
            if (roleId == null) return (from d in new DatabaseContext().Roles select d).ToList();
            return (from d in new DatabaseContext().Roles where d.ID == roleId select d).ToList();
        }
        public ResultView AddRole(TB_USERGROUP role)
        {
            try
            {
                var isExist = (from d in new DatabaseContext().Roles where d.CNAME.Equals(role.CNAME) select d).FirstOrDefault();
                if (isExist != null)
                {
                    return new ResultView { IsSuccess = false, ErrMsg = "角色名称已经存在" };
                }
                role.LastUpdateTime = DateTime.Now;
                var cmd = DataCommand<DatabaseContext>.Add<TB_USERGROUP>(role);
                return new ResultView { IsSuccess = true };
            }
            catch (Exception ex)
            {
                return new ResultView { IsSuccess = false, ErrMsg = ex.Message };
            }
        }
        public ResultView DelRole(string rolesId)
        {
            try
            {
                var cmd = new DataCommand<DatabaseContext>("sqlServer", "DeleteRolesById");
                cmd.SetParameters("@roles", rolesId);
                cmd.ExecuteNoReturn();
                return new ResultView { IsSuccess = true };
            }
            catch (Exception ex)
            {
                return new ResultView { IsSuccess = false, ErrMsg = ex.Message };
            }
        }

        public List<TB_USER> GetUserList(int pageSize, int curPage, out int cnt, string whereStr)
        {
            try
            {
                if (string.IsNullOrEmpty(whereStr))
                {
                    var list = (from d in new DatabaseContext().Users orderby d.ID select d).Skip((curPage - 1) * pageSize).Take(pageSize).ToList();
                    cnt = new DatabaseContext().Roles.Count();
                    return list;
                }
                var list2 = (from d in new DatabaseContext().Users where d.CNAME.Contains(whereStr) orderby d.ID select d).Skip((curPage - 1) * pageSize).Take(pageSize).ToList();
                cnt = new DatabaseContext().Users.Count(x => x.CNAME.Contains(whereStr));
                return list2;

            }
            catch (Exception ex)
            {
                cnt = 0;
                return null;
            }
        }

        public ResultView AddUser(TB_USER user)
        {
            try
            {
                var isExist = (from d in new DatabaseContext().Users where d.CNAME.Equals(user.CNAME) select d).FirstOrDefault();
                if (isExist != null)
                {
                    return new ResultView { IsSuccess = false, ErrMsg = "用户名称已经存在" };
                }
                user.LastUpdateTime = DateTime.Now;
                var cmd = DataCommand<DatabaseContext>.Add<TB_USER>(user);
                return new ResultView { IsSuccess = true };
            }
            catch (Exception ex)
            {
                return new ResultView { IsSuccess = false, ErrMsg = ex.Message };
            }
        }

        public ResultView DelUser(string userId)
        {
            try
            {
                var cmd = new DataCommand<DatabaseContext>("sqlServer", "DeleteUserById");
                cmd.SetParameters("@roles", userId);
                cmd.ExecuteNoReturn();
                return new ResultView { IsSuccess = true };
            }
            catch (Exception ex)
            {
                return new ResultView { IsSuccess = false, ErrMsg = ex.Message };
            }
        }

        public List<TB_MENU> GetMenuList()
        {
            try
            {
                return new DatabaseContext().Menus.ToList();
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public List<TreeNodeBll> GetMenuTree()
        {
            try
            {
                var result = new List<TreeNodeBll>();
                var menuList = GetMenuList();
                var parentMenu = (from p in menuList where p.ParentID == 0 select p);
                foreach (var topNode in parentMenu)
                {
                    var node = new TreeNodeBll
                    {
                        id = topNode.ID,
                        text = topNode.MenuName,
                        children = new List<TreeNodeBll>(),
                        link=topNode.LinkName
                    };
                    result.Add(node);
                    CreateTree(menuList, node.id, node);
                }
                return result;

            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public ResultRoleRights GetURights(int roleId)
        {
            try
            {
                var result = new ResultRoleRights { Data=new List<ReusltRoleType>()};
                result.RoleId = roleId;
                var rightsList = new DatabaseContext().URights.Where(x => x.RoleId == roleId).ToList();
                foreach(var item in rightsList.Where(x=>x.RightType==1))
                {
                    var pTmp = new ReusltRoleType { MenuRights = item.RightId,OperRights=new List<int>()};
                    pTmp.OperRights=(from p in rightsList where p.RightId == item.RightId && p.RightType == 2 select p.RightId).ToList();
                    result.Data.Add(pTmp);
                }
                result.ActionResult = new ResultView { IsSuccess = true };
                return result;
            }
            catch (Exception ex)
            {
                return new ResultRoleRights { ActionResult = new ResultView { IsSuccess = false, ErrMsg = ex.Message } };
            }

        }
        public ResultView ModifyRights(RoleRightsBll query)
        {
            try
            {
                foreach (var item in query.Data)
                {
                    var isExists = new DatabaseContext().URights.Where(x => x.RightType == query.RightsType && x.RoleId == query.RoleId
                                    && x.RightId == item);
                    if (!isExists.Any())
                    {
                        var obj = new Tb_UsergroupRights
                        {
                            RightId = item,
                            RightType = query.RightsType,
                            RoleId = query.RoleId
                        };
                        DataCommand<DatabaseContext>.Add<Tb_UsergroupRights>(obj);
                    }
                }
                return new ResultView { IsSuccess = true };
            }
            catch (Exception ex)
            {
                return new ResultView { IsSuccess = false, ErrMsg = ex.Message };
            }
        }

        public ResultOperList GetPageOperList(int pageId)
        {
            try
            {
                var context = new DatabaseContext();
                var result = new ResultOperList { ActionResult = new ResultView() };
                var list1 = (from p in context.PageRights
                             join d in context.URights on p.Id+10000 equals d.RightId into tempw
                             from tt in tempw.DefaultIfEmpty()
                             where (tt.RightType == 2 || tt == null) && p.PageId==pageId

                             select new ResultOper
                             {
                                 @checked = tt == null ? false : true,
                                 OperId = p.Id,
                                 OperName = p.OperName
                             }
                            ).ToList();
                result.OperList = list1;
                result.ActionResult.IsSuccess = true;
                return result;
            }catch(Exception ex)
            {
                return new ResultOperList { ActionResult = new ResultView { IsSuccess = false, ErrMsg = ex.Message } };
            }
        }

        public ResultView ModifyOperRights(int roleId, int pageId,IList<int> ids)
        {
            try
            {
                foreach (var id in ids)
                {
                    var isExists = new DatabaseContext().URights.Where(x => x.RightType == 2 && x.RoleId == roleId
                                  && x.RightId == id+10000);
                    if (!isExists.Any()) {
                        var obj = new Tb_UsergroupRights
                        {
                            RightId = 10000 +id,
                            RightType = 2,
                            RoleId = roleId
                        };
                        DataCommand<DatabaseContext>.Add<Tb_UsergroupRights>(obj);
                    }
                }
                return new ResultView { IsSuccess = true };
            }catch(Exception ex)
            {
                return new ResultView { IsSuccess = false, ErrMsg = ex.Message };
            }
        }

        private void CreateTree(IList<TB_MENU> list, int curId, TreeNodeBll result)
        {
            var children = (from p in list where p.ParentID == curId select p);
            if (children == null) return;

            foreach (var node in children)
            {
                var treeNode = new TreeNodeBll
                {
                    id = node.ID,
                    text = node.MenuName,
                    children = new List<TreeNodeBll>(),
                    link = node.LinkName
                };
                result.children.Add(treeNode);
                CreateTree(list, treeNode.id, treeNode);
            }
        }
    }
}
