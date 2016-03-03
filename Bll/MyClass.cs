using System;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;

namespace Bll
{
	public class DatabaseContext : DbContext
	{
		public DatabaseContext() : base("DB") { }
		public DbSet<TB_USER> Users { get; set; }
		public DbSet<TB_USERGROUP> Roles { get; set; }
		public DbSet<TB_MENU> Menus { get; set; }
		public DbSet<Tb_UsergroupRights> URights { get; set; }
		public DbSet<TB_PageRights> PageRights { get; set; } 
	}

	public class TB_USER
	{
		[Key]
		public int ID { get; set; }
		[MaxLength(32)]
		public string CNAME { get; set; }
		[MaxLength(32)]
		public string PASSWORD { get; set; }
		public short STATUS { get; set; }
		public string GROUPS { get; set; }
		public short TYPE { get; set; }
		public Nullable<DateTime> LastUpdateTime { get; set; }
	}

	public class TB_USERGROUP
	{
		[Key]
		public int ID { get; set; }
		[MaxLength(32)]
		public string CNAME { get; set; }
		[MaxLength(128)]
		public string DESCRIPTION { get; set; }
		public Nullable<DateTime> LastUpdateTime { get; set; }
	}

	public class TB_MENU
	{
		[Key]
		public int ID { get; set; }
		[MaxLength(50)]
		public string MenuName { get; set; }
		[MaxLength(200)]
		public string LinkName { get; set; }
		[MaxLength(50)]
		public string IconClass { get; set; }
		public int ParentID { get; set; }
		public int MenuClass { get; set; }
		public int MenuOrder { get; set; }
	}
	public class Tb_UsergroupRights
	{
		[Key]
		public int ID { get; set; }
		public int RoleId { get; set; }
		public int RightId { get; set; }
		public int RightType { get; set; }
	}
	public class TB_PageRights
	{
		[Key]
		public int Id { get; set; }
		[MaxLength(30)]
		public string OperName { get; set; }
		public int PageId { get; set; }
	}
}


