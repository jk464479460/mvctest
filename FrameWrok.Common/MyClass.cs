using System;
using System.Collections.Generic;
using System.Data.Entity;
//using System.Data.SqlTypes;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;

namespace FrameWrok.Common
{
	public class DataCommand<TContext> where TContext : DbContext, new()
	{
		private readonly string _cmdKey;
		private readonly string _server;
		private System.Collections.Generic.IDictionary<string, SqlCmdModel> _dictCmd = new Dictionary<string, SqlCmdModel>();

		/// <summary>
		/// xml中，sqlcmd 的name属性
		/// </summary>
		public string CmdKey
		{
			get
			{
				return _cmdKey;
			}
		}
		/// <summary>
		/// 数据库类型：支持sql server
		/// </summary>
		public string Server
		{
			get
			{
				return _server;
			}
		}

		/// <summary>
		/// 当前sql语句
		/// </summary>
		public string CurCmdLine => _dictCmd.Keys.Any() == false ? string.Empty : _dictCmd[_cmdKey].CmdLine;

		public DataCommand() { }
		public DataCommand(string server, string sqlCmdName)
		{
			_server = server;
			_cmdKey = sqlCmdName;
			ReadCmmdConfig();
		}
		/// <summary>
		/// sql语句参数
		/// </summary>
		/// <param name="param"></param>
		/// <param name="val"></param>
		public void SetParameters(string param, string val)
		{
			if(_dictCmd.Keys.Any()==false) throw new FileNotFoundException();
			var targetCmd = _dictCmd[_cmdKey].CmdLine;
			_dictCmd[_cmdKey].CmdLine = targetCmd.Replace(param, val);
		}
		/// <summary>
		/// 自定义sql
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns>可以返回null</returns>
		public System.Collections.Generic.List<T> ExecuteSql<T>()
		{
			if (string.IsNullOrEmpty(CurCmdLine)) return null;
			using (var db =  DataCommandContext<TContext>.GetContext())
			{
				var sql = CurCmdLine;
				var res = db.Database.SqlQuery<T>(sql).ToList();
				return res;
			}
		}
		public void ExecuteNoReturn()
		{
			if (string.IsNullOrEmpty(CurCmdLine)) return ;
			using (var db = DataCommandContext<TContext>.GetContext())
			{
				var sql = CurCmdLine;
				db.Database.ExecuteSqlCommand(sql);
			}
		}
		/// <summary>
		/// 添加基本表内容
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="obj"></param>
		/// <returns></returns>
		public static bool Add<T>(T obj) where T:class 
		{
			using (var db = DataCommandContext<TContext>.GetContext())
			{
				db.Entry(obj).State=EntityState.Added;
				db.SaveChanges();
				return true;
			}
		}
		public static System.Data.Entity.DbContext GetCurContext()
		{
			return DataCommandContext<TContext>.GetContext();
		}
		private void ReadCmmdConfig()
		{
			var dir=new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory+"config");
			if(dir==null) throw new InvalidOperationException();
			var fileList=dir.GetFiles("*.xml");
			foreach (var fileInfo in fileList)
			{
				ParseXml(fileInfo.FullName);
				if (_dictCmd.Keys.Count > 0) return;
			}
		}

		private void ParseXml(string path)
		{
			SqlRoot root;
			using (var stream = new StreamReader(path))
			{
				var xmlSeri = new XmlSerializer(typeof(SqlRoot));
				root = (SqlRoot)xmlSeri.Deserialize(stream);
			}
			if (!root.SqlCmdList.Any()) throw new ArgumentNullException();
			var serverFound =root.SqlCmdList.Where(x => x.Name.Equals(_server)).ToList();
			if(!serverFound.Any()) throw new ArgumentNullException();
			var cmdFound = serverFound[0].SqlCmd.Where(x => x.Name.Equals(_cmdKey)).ToList();
			if(!cmdFound.Any()) throw new ArgumentNullException();
			_dictCmd.Add(_cmdKey,cmdFound[0]);
		}
	}

	[XmlRoot("SqlRoot")]
	public class SqlRoot
	{
		[XmlElement("SqlList")]
		public List<SqlList> SqlCmdList;
	}
	[XmlRoot("SqlList")]
	public class SqlList
	{
		[XmlAttribute("name")]
		public string Name { get; set; }

		[XmlElement("SqlCmd")]
		public List<SqlCmdModel> SqlCmd { get; set; } 
	}
	[XmlRoot("SqlCmd")]
	public class SqlCmdModel
	{
		[XmlElement("param")]
		public List<Params> Param { get; set; }
		[XmlElement("CmdLine")]
		public string CmdLine { get; set; }
		[XmlAttribute("name")]
		public string Name { get; set; }
	}
	[XmlRoot]
	public class Params
	{
		[XmlAttribute("name")]
		public string Name { get; set;}
		[XmlAttribute("type")]
		public string Type { get; set; }
	}


	internal class DataCommandContext<TContext> where TContext :System.Data.Entity.DbContext,new()
	{
		public void Dispose()
		{

		}

		public static TContext GetContext()
		{
			return new TContext();
		}
	}
}

