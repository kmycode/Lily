/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */
package net.kmycode.javaspeechserver.connection;

import com.google.gson.Gson;
import java.io.BufferedWriter;
import java.io.IOException;

/**
 *
 * @author KMY
 */
public class CSharpClientSender {

	private BufferedWriter server;

	private static CSharpClientSender defaultInstance;
	public static CSharpClientSender getDefault() {
		if (defaultInstance == null) {
			defaultInstance = new CSharpClientSender();
		}
		return defaultInstance;
	}

	void setServer(BufferedWriter server) {
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

	public void send(RecognitionResult result) {
		Gson gson = new Gson();
		String data = gson.toJson(result);
		this.send('r', data);
	}

	public void send(SoundInformation info) {
		Gson gson = new Gson();
		String data = gson.toJson(info);
		this.send('v', data);
	}

	private void send(char signature, String data) {
		try {
			this.server.write(signature + data, 0, data.length() + 1);
			this.server.flush();
		} catch (IOException ex) {
			ex.printStackTrace();
		}
	}
}
