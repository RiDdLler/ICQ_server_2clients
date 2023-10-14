using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

class Server
{
	private TcpListener tcpListener;
	private List<TcpClient> clients = new List<TcpClient>();

	public void Start()
	{
		tcpListener = new TcpListener(IPAddress.Any, 8888);
		tcpListener.Start();

		Console.WriteLine("Сервер запущен.");

		while (true)
		{
			TcpClient client = tcpListener.AcceptTcpClient();
			clients.Add(client);

			Thread clientThread = new Thread(new ParameterizedThreadStart(HandleClientComm));
			clientThread.Start(client);
		}
	}

	private void HandleClientComm(object clientObj)
	{
		TcpClient tcpClient = (TcpClient)clientObj;
		NetworkStream clientStream = tcpClient.GetStream();

		byte[] message = new byte[4096];
		int bytesRead;

		while (true)
		{
			bytesRead = 0;

			try
			{
				bytesRead = clientStream.Read(message, 0, 4096);
			}
			catch
			{
				break;
			}

			if (bytesRead == 0)
				break;

			string receivedMessage = Encoding.ASCII.GetString(message, 0, bytesRead);
			Console.WriteLine($"Получено сообщение: {receivedMessage}");

			BroadcastMessage(receivedMessage, tcpClient);
		}

		clients.Remove(tcpClient);
		tcpClient.Close();
	}

	private void BroadcastMessage(string message, TcpClient excludeClient)
	{
		foreach (var client in clients)
		{
			if (client != excludeClient)
			{
				NetworkStream clientStream = client.GetStream();
				byte[] broadcastMessage = Encoding.ASCII.GetBytes(message);
				clientStream.Write(broadcastMessage, 0, broadcastMessage.Length);
				clientStream.Flush();
			}
		}
	}
}

class Program
{
	static void Main(string[] args)
	{
		Server server = new Server();
		server.Start();
	}
}
