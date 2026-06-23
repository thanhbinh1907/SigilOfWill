using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace SG
{
	public class UDPReceiver : MonoBehaviour
	{
		public static UDPReceiver instance;

		Thread receiveThread;
		UdpClient client;
		public int port = 11000;
		private volatile bool isRunning = true;

		[Header("Data Received From Python")]
		public int currentGestureID = -1;
		public string currentVoiceWord = "";

		private void Awake()
		{
			if (instance == null) instance = this;
			else Destroy(gameObject);
		}

		private void Start()
		{
			if (instance != this)
			{
				return;
			}
			InitUDP();
			DontDestroyOnLoad(gameObject);
		}

		private void InitUDP()
		{
			receiveThread = new Thread(new ThreadStart(ReceiveData));
			receiveThread.IsBackground = true;
			receiveThread.Start();
		}

		private void ReceiveData()
		{
			try
			{
				client = new UdpClient(port);
				while (isRunning)
				{
					try
					{
						IPEndPoint anyIP = new IPEndPoint(IPAddress.Any, 0);
						byte[] data = client.Receive(ref anyIP);

						if (!isRunning) break;

						string text = Encoding.UTF8.GetString(data).Trim();
						System.Console.WriteLine($">> [UDP RECEIVER] Nhận dữ liệu thô từ Python: '{text}'");


						if (text.Contains(","))
						{
							string[] splitData = text.Split(',');
							if (splitData.Length >= 2)
							{
								if (int.TryParse(splitData[0], out int id))
								{
									currentGestureID = id;
								}
								currentVoiceWord = splitData[1].ToLower().Trim();
							}
						}
						else
						{

							if (int.TryParse(text, out int id))
							{
								currentGestureID = id;
								currentVoiceWord = "";
							}
						}
						System.Console.WriteLine($">> [UDP RECEIVER] Phân tích thành công: GestureID = {currentGestureID}, VoiceWord = '{currentVoiceWord}'");
					}
					catch (System.Exception e)
					{
						if (!isRunning) break;
						System.Console.WriteLine(e.ToString());
					}
				}
			}
			catch (System.Exception e)
			{
				System.Console.WriteLine("UDP Receiver Thread Error: " + e.Message);
			}
		}


		public void ResetUDPData()
		{
			currentGestureID = -1;
			currentVoiceWord = "";
		}

		private void OnDestroy()
		{
			if (instance == this) instance = null;
			isRunning = false;
			if (client != null) client.Close();
		}
	}
}