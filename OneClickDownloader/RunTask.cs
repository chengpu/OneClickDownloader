using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Threading;


namespace OneClickDownloader
{
	public class RunTask
	{
		//
		private string task;

		//
		private class Downloader
		{
			public string FilePath;
			public string FileName;
			public int FileSize;
			public string Url;

			public void Do(string basePath)
			{
				//
				byte[] data = HttpHelper.GetData(Url);
				if (data == null)
				{
					Console.WriteLine(string.Format("GetError {0}", Url));
					return;
				}
				if (data.Length != FileSize)
				{
					Console.WriteLine(string.Format("SizeError {0}", Url));
					return;
				}

				//
				Directory.CreateDirectory(basePath + "\\" + FilePath);
				File.WriteAllBytes(basePath + "\\" + FilePath + FileName, data);
				Console.WriteLine(string.Format("Get {0}", Url));
			}
		}

		//
		private class DownloaderQueue
		{
			private Queue<Downloader> queue;

			public DownloaderQueue()
			{
				queue = new Queue<Downloader>();
			}

			public void Add(Downloader downloader)
			{
				lock (this)
				{
					queue.Enqueue(downloader);
				}
			}

			public Downloader Get()
			{
				lock (this)
				{
					if (queue.Count <= 0)
					{
						return null;
					}
					return queue.Dequeue();
				}
			}
		}

		public RunTask(string task)
		{
			this.task = task;
		}

		public void Do()
		{
			//
			DownloaderQueue downloaderQueue = new DownloaderQueue();
			int downloadCount = 0;
			int ingoreCount = 0;

			//
			List<Downloader> downloaders = ParseTaskXml();
			for (int i = 0; i < downloaders.Count; i++)
			{
				//
				Downloader downloader = downloaders[i];

				//
				FileInfo fi = new FileInfo(task + "\\" + downloader.FilePath + downloader.FileName);
				if (fi.Exists && (fi.Length == downloader.FileSize))
				{
					ingoreCount++;
					continue;
				}
				
				//
				downloaderQueue.Add(downloader);
				downloadCount++;
			}
			System.Console.WriteLine(string.Format("Load task:{0} ingore:{1}", downloadCount, ingoreCount));

			//
			List<Thread> threads = new List<Thread>();
			for (int i = 0; i < 10; i++)
			{
				Thread thread = new Thread(delegate()
					{
						while (true)
						{
							Downloader downloader = downloaderQueue.Get();
							if (downloader == null)
							{
								break;
							}
							downloader.Do(task);
						}
					}
				);
				thread.Start();
				threads.Add(thread);
			}

			//
			for (int i = 0; i < threads.Count; i++)
			{
				threads[i].Join();
			}

			//
			int missingCount = 0;
			for (int i = 0; i < downloaders.Count; i++)
			{
				//
				Downloader downloader = downloaders[i];

				//
				FileInfo fi = new FileInfo(task + "\\" + downloader.FilePath + downloader.FileName);
				if (!fi.Exists || (fi.Length != downloader.FileSize))
				{
					missingCount++;
					System.Console.WriteLine(string.Format("Missing: {0}", downloader.Url));
				}
			}

			//
			if (missingCount > 0)
			{
				Console.WriteLine(string.Format("RunTask [{0}] Complete!", task));
				Console.WriteLine(string.Format("missing {0} file{1}, Please retry.", missingCount, (missingCount > 1 ? "s" : "")));
				return;
			}

			//
			Console.WriteLine(string.Format("RunTask [{0}] Complete!", task));
		}

		private List<Downloader> ParseTaskXml()
		{
			//
			XmlDocument doc = new XmlDocument();
			doc.Load(task + ".xml");

			//
			List<Downloader> downloaders = new List<Downloader>();

			//
			XmlNodeList list = doc.SelectNodes("//file");
			for (int i = 0; i < list.Count; i++)
			{
				XmlNode node = list[i];
				Downloader downloader = new Downloader();
				downloader.FilePath = node.Attributes["FilePath"].Value;
				downloader.FileName = node.Attributes["FileName"].Value;
				downloader.FileSize = int.Parse(node.Attributes["FileSize"].Value);
				downloader.Url = node.Attributes["Url"].Value;
				downloaders.Add(downloader);
			}

			//
			return downloaders;
		}
	}
}

