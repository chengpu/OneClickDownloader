using System;


namespace OneClickDownloader
{
	public class Program
	{
		static void Main(string[] args)
		{
			// TODO
			new CreateTask("GitHub", "http://github-windows.s3.amazonaws.com/GitHub.application").Do();
			return;

			//
			string command = "";
			if (args.Length >= 1)
			{
				command = args[0];
			}

			//
			if (command == "CreateTask")
			{
				//
				if (args.Length != 3)
				{
					System.Console.WriteLine("Usage:");
					System.Console.WriteLine("\tOneClickDownloader.exe CreateTask task url");
					return;
				}
				string task = args[1];
				string url = args[2];

				//
				new CreateTask(task, url).Do();
				return;
			}
			else if (command == "RunTask")
			{
				//
				if (args.Length != 2)
				{
					System.Console.WriteLine("Usage:");
					System.Console.WriteLine("\tOneClickDownloader.exe RunTask task");
					return;
				}
				string task = args[1];

				//
				new RunTask(task).Do();
				return;
			}
			else
			{
				System.Console.WriteLine("Usage:");
				System.Console.WriteLine("\tOneClickDownloader.exe CreateTask task url");
				System.Console.WriteLine("\tOneClickDownloader.exe RunTask task");
				return;
			}
		}
	}
}

