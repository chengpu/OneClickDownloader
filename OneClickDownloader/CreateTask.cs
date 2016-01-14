using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;


namespace OneClickDownloader
{
	public class CreateTask
	{
		//
		private string task;
		private string url;

		//
		private string appUrl;
		private string appPath;
		private string appName;
		private string appXml;

		//
		private string manifestUrl;
		private string manifestPath;
		private string manifestName;
		private string manifestXml;

		//
		private class Item
		{
			public string name;
			public int size;
		}

		public CreateTask(string task, string url)
		{
			this.task = task;
			this.url = url;
		}

		public void Do()
		{
			//
			appUrl = url;
			appPath = ".\\";
			appName = url.Substring(url.LastIndexOf('/') + 1);
			appXml = HttpHelper.GetText(appUrl);
			Console.WriteLine(string.Format("load {0}", appUrl));

			//
			string manifestCodebase = ParseAppXml(appXml);

			//
			manifestUrl = appUrl.Substring(0, url.LastIndexOf('/') + 1) + manifestCodebase.Replace('\\', '/');
			manifestPath = manifestCodebase.Substring(0, manifestCodebase.LastIndexOf('\\') + 1);
			manifestName = manifestCodebase.Substring(manifestCodebase.LastIndexOf('\\') + 1);
			manifestXml = HttpHelper.GetText(manifestUrl);
			Console.WriteLine(string.Format("load {0}", manifestUrl));

			//
			List<Item> items = ParseManifestXml(manifestXml);

			//
			string taskXml = "<?xml version=\"1.0\" encoding=\"utf-8\"?>\r\n";
			taskXml += "<files>\r\n";
			for (int i = 0; i < items.Count; i++)
			{
				string name = items[i].name;
				int size = items[i].size;
				string filePath = manifestPath;
				string fileName = name;
				if (name.LastIndexOf('\\') > 0)
				{
					filePath += name.Substring(0, name.LastIndexOf('\\') + 1);
					fileName = name.Substring(name.LastIndexOf('\\') + 1);
				}
				string fileUrl = manifestUrl.Substring(0, manifestUrl.LastIndexOf('/') + 1) + name.Replace('\\', '/');
				fileName += ".deploy";
				fileUrl += ".deploy";
				taskXml += string.Format("<file FilePath=\"{0}\" FileName=\"{1}\" FileSize=\"{2}\" Url=\"{3}\"/>\r\n", filePath, fileName, size, fileUrl);
			}
			taskXml += "</files>\r\n";

			//
			Directory.CreateDirectory(task);

			Directory.CreateDirectory(task + "\\" + appPath);
			File.WriteAllText(task + "\\" + appPath + appName, appXml);

			Directory.CreateDirectory(task + "\\" + manifestPath);
			File.WriteAllText(task + "\\" + manifestPath + manifestName, manifestXml);

			File.WriteAllText(task + ".xml", taskXml);

			//
			Console.WriteLine(string.Format("CreateTask [{0}] Complete!", task));
		}

		private static string ParseAppXml(string xml)
		{
			XmlDocument doc = new XmlDocument();
			doc.LoadXml(xml);

			XmlNamespaceManager mgr = new XmlNamespaceManager(doc.NameTable);
			mgr.AddNamespace("ns", "urn:schemas-microsoft-com:asm.v2");

			XmlNode node = doc.SelectSingleNode("//ns:dependentAssembly", mgr);
			return node.Attributes["codebase"].Value;
		}

		private List<Item> ParseManifestXml(string xml)
		{
			//
			XmlDocument doc = new XmlDocument();
			doc.LoadXml(xml);

			XmlNamespaceManager mgr = new XmlNamespaceManager(doc.NameTable);
			mgr.AddNamespace("ns", "urn:schemas-microsoft-com:asm.v2");

			//
			List<Item> items = new List<Item>();

			//
			XmlNodeList list1 = doc.SelectNodes("//ns:dependentAssembly", mgr);
			for (int i = 0; i < list1.Count; i++)
			{
				XmlNode node = list1[i];
				if (node.Attributes["dependencyType"].Value == "install")
				{
					Item item = new Item();
					item.name = node.Attributes["codebase"].Value;
					item.size = int.Parse(node.Attributes["size"].Value);
					items.Add(item);
				}
			}

			//
			XmlNodeList list2 = doc.SelectNodes("//ns:file", mgr);
			for (int i = 0; i < list2.Count; i++)
			{
				XmlNode node = list2[i];
				Item item = new Item();
				item.name = node.Attributes["name"].Value;
				item.size = int.Parse(node.Attributes["size"].Value);
				items.Add(item);
			}

			//
			return items;
		}
	}
}

