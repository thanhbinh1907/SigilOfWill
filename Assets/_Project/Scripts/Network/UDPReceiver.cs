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

		[Header("Data Received From Python")]
		public int currentGestureID = -1;
		public string currentVoiceWord = ""; // BIẾN MỚI: Lưu từ khóa nhận từ Python

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
					Debug.Log($">> [UDP RECEIVER] Nhận dữ liệu thô từ Python: '{text}'");

					// Phân tích định dạng mới từ Python: "ID,TừKhóa" (Ví dụ: "2,thunderbolt")
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
						// Tương thích ngược: Nếu Python chỉ gửi mỗi số ID
						if (int.TryParse(text, out int id))
						{
							currentGestureID = id;
							currentVoiceWord = "";
						}
					}
					Debug.Log($">> [UDP RECEIVER] Phân tích thành công: GestureID = {currentGestureID}, VoiceWord = '{currentVoiceWord}'");
				}
				catch (System.Exception e) { Debug.Log(e.ToString()); }
			}
		}

		// Hàm dọn dẹp dữ liệu cũ
		public void ResetUDPData()
		{
			currentGestureID = -1;
			currentVoiceWord = "";
		}

		private void OnDestroy()
		{
			if (instance == this) instance = null;
			if (receiveThread != null) receiveThread.Abort();
			if (client != null) client.Close();
		}
	}
}