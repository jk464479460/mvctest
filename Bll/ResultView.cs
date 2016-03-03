using System.Collections.Generic;

namespace Bll
{
	public class ResultView
	{
		public bool IsSuccess { get; set; }
		public string ErrMsg { get; set; }
	}
	public class TreeNodeBll
	{
		public int id { get; set; }
		public string text { get; set; }
		public string link { get; set; }
		public List<TreeNodeBll> children { get; set; }
	}
	public class RoleRightsBll
	{
		public int RightsType { get; set; }
		public int RoleId { get; set; }
		public List<int> Data { get; set; }
	}
	public class ResultRoleRights
	{
		public ResultView ActionResult { get; set; }
		public int RoleId { get; set; }
		public List<ReusltRoleType> Data { get; set; }
	}
	public class ReusltRoleType
	{
		public int MenuRights { get; set; }
		public List<int> OperRights { get; set; }
	}
	public class ResultOperList
	{
		public ResultView ActionResult { get; set; }
		public List<ResultOper> OperList { get; set; }
	}
	public class ResultOper
	{
		public int OperId { get; set; }
		public string OperName { get; set; }
		public bool @checked { get; set; }
	}
}
