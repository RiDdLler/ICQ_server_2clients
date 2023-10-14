using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;

class Client
{
	private TcpClient tcpClient;
	private NetworkStream clientStream;

	public void Start()
	{
		tcpClient = new TcpClient("127.0.0.1", 8888);
		clientStream = tcpClient.GetStream();

		Thread listenThread = new Thread(new ThreadStart(ListenForMessages));
		listenThread.Start();

		while (true)
		{
			string message = Console.ReadLine();
			SendMessage(message);
		}
	}

	private void ListenForMessages()
	{
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
		}
	}

	private void SendMessage(string message)
	{
		byte[] messageBytes = Encoding.ASCII.GetBytes(message);
		clientStream.Write(messageBytes, 0, messageBytes.Length);
		clientStream.Flush();
	}
}

class Program
{
	static void Main(string[] args)
	{
		Client client = new Client();
		client.Start();
	}
}
