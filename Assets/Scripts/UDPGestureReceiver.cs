using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace SG
{
	public class UDPGestureReceiver : MonoBehaviour
	{
		public static UDPGestureReceiver instance;

		Thread receiveThread;
		UdpClient client;
		public int port = 11000;

		// Bi?n l?u tr? ID c? ch? m?i nh?t
		public int currentGestureID = -1;

		private void Awake()
		{
			if (instance == null) instance = this;
			else Destroy(gameObject);
		}

		private void Start()
		{
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
			client = new UdpClient(port);
			while (true)
			{
				try
				{
					IPEndPoint anyIP = new IPEndPoint(IPAddress.Any, 0);
					byte[] data = client.Receive(ref anyIP);
					
					string text = Encoding.UTF8.GetString(data).Trim();

					Debug.Log($"[RAW UDP DATA] Đã nhận được chuỗi: '{text}'");
					// C?p nh?t ID nh?n ???c t? Python
					if (int.TryParse(text, out int id))
					{
						currentGestureID = id;
					}
				}
				catch (System.Exception e) { Debug.Log(e.ToString()); }
			}
		}

		public void ResetGesture()
		{
			currentGestureID = -1;
		}

		private void OnApplicationQuit()
		{
			if (receiveThread != null) receiveThread.Abort();
			if (client != null) client.Close();
		}
	}
}