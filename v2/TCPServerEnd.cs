// Source: https://msdn.microsoft.com/en-us/library/system.net.sockets.socket(v=vs.80).aspx

using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Net;
using UnityEngine;
using System.Net.Sockets;
using System.Threading;


public class TCPServerEnd : MonoBehaviour {
	
	public int port;
	Thread tcpListenerThread;
	private Queue<DataPointViewModel> dataPointsQueue= new Queue<DataPointViewModel>();
	private object queueLock= new System.Object();

	void Start()
	{
		tcpListenerThread = new Thread (() => ListenForMessages (port));
		tcpListenerThread.Start();
	}
	void Update()
	{
		Queue<DataPointViewModel> tempqueue;

		lock (queueLock) {
			tempqueue = dataPointsQueue;
			dataPointsQueue = new Queue<DataPointViewModel> ();
		}

		foreach (var point in dataPointsQueue) {
//			var viewModel = new DataPointViewModel ();
//			viewModel.rightAscendsion = UnityEngine.Random.value;
//			viewModel.declination = UnityEngine.Random.Range (-1f, 1f);
//			viewModel.color = Color.cyan;
//			SendMessage ("Data received", viewModel);
			Debug.Log("Read Message");
			SendMessage ("Data Received", point);
		}
		dataPointsQueue.Clear();
	}
	public void ListenForMessages(int port)
	    {
	        TcpListener server = null;
	        try
	        {
	            // Set the TcpListener on port 13000.
	           
	            IPAddress localAddr = IPAddress.Parse("127.0.0.1");

	            // TcpListener server = new TcpListener(port);
	            server = new TcpListener(localAddr, port);

	            // Start listening for client requests.
	            server.Start();

	            // Buffer for reading data
	            Byte[] bytes = new Byte[256];
	            String data = null;

	            // Enter the listening loop.
	            while (true)
	            {
				Debug.Log("Waiting for a connection... ");
	                //Console.Write("Waiting for a connection... ");

	                // Perform a blocking call to accept requests.
	                // You could also user server.AcceptSocket() here.
	                TcpClient client = server.AcceptTcpClient();
				Debug.Log("Connected!");

	                data = null;

	                // Get a stream object for reading and writing
	                NetworkStream stream = client.GetStream();

	                int i;

	                // Loop to receive all the data sent by the client.
	                while ((i = stream.Read(bytes, 0, bytes.Length)) != 0)
	                {
	                    // Translate data bytes to a ASCII string.
	                    data = System.Text.Encoding.ASCII.GetString(bytes, 0, i);
					Debug.Log(String.Format("Received: {0}", data));

//					var viewModel=new DataPointViewModel();
//					viewModel.rightAscendsion = UnityEngine.Random.value;
//					viewModel.declination= UnityEngine.Random.Range(-1f,1f);
//					viewModel.color= Color.cyan;
					var viewModel=JsonUtility.FromJson<DataPointViewModel>(data);
					lock(queueLock)
					{
					dataPointsQueue.Enqueue(viewModel);
					}

	                    // Process the data sent by the client.

	                    data = data.ToUpper();

	                    byte[] msg = System.Text.Encoding.ASCII.GetBytes(data);

	                    // Send back a response.
	                    stream.Write(msg, 0, msg.Length);
					Debug.Log(String.Format("Sent: {0}", data));
	                }

	                // Shutdown and end connection
	                client.Close();
	            }
	        }
	        catch (SocketException e)
	        {
			Debug.LogError(String.Format("SocketException: {0}", e));
	        }
	        finally
	        {
	            // Stop listening for new clients.
	            server.Stop();
	        }
			        
	    }
}
