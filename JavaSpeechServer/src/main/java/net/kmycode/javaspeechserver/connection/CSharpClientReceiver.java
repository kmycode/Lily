/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */
package net.kmycode.javaspeechserver.connection;

import java.io.BufferedReader;
import java.io.IOException;

/**
 *
 * @author KMY
 */
public class CSharpClientReceiver {

	private BufferedReader server;
	private char[] buffer = new char[2048];

	private static CSharpClientReceiver defaultInstance;
	public static CSharpClientReceiver getDefault() {
		if (defaultInstance == null) {
			defaultInstance = new CSharpClientReceiver();
		}
		return defaultInstance;
	}

	private Thread loopThread;
	private ClientState state;

	public ClientState getState() {
		return this.state;
	}

	void setServer(BufferedReader server) {
		this.server = server;
	}

	public void close() {
		try {
			this.server.close();
		} catch (IOException ex) {
			ex.printStackTrace();
		}
	}

	public void finalize() throws Throwable {
		super.finalize();
		this.close();
	}

	public ClientState beginLoop() {
		if (this.state != null) {
			return this.state;
		}
		this.state = new ClientState();

		this.loopThread = new Thread(() -> {
			while (true) {
				try {
					String data = this.server.readLine();
					if (data != null && data.length() > 0) {
						this.readObject(data);
					}
					Thread.sleep(10);
				} catch (IOException ex) {
					ex.printStackTrace();
				} catch (InterruptedException ex) {
					ex.printStackTrace();
				}
			}
		});
		this.loopThread.start();

		return this.state;
	}

	private void clearBuffer() {
		int i = 0;
		while (this.buffer[i] != 0) {
			this.buffer[i] = 0;
			i++;
		}
	}

	private void readObject(String data) {
		char signature = data.charAt(0);
		String json = data.substring(1);

		switch (signature) {
		case 'V':
			this.state.setRecognize(true);
			break;
		case 'v':
			this.state.setRecognize(false);
			break;
		}
	}

}
